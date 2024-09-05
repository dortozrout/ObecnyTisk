using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Form;

namespace Labels
{

	class Program
	{
		static void Main(string[] args)
		{
			Run(args);
		}
		//rozdeleno kvuli "restartovani" programu (objekt spravce)
		public static void Run(string[] args)
		{
#if NETCOREAPP
			//Je treba pro .NET CORE
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
			Console.OutputEncoding = Encoding.UTF8;
			if (args.Length == 0) //pokud je program spusten bez parametru
			{
				Configuration.Load("conf.txt"); //pouzije se vychozi konfigurak
			}
			else //jinak se prvni parametr bere jako jmeno konfiguraku v %appdata%\TiskStitku
			{
				Configuration.Load(args[0]);
			}
			if (Configuration.Login)
			{
				LoginForm loginForm = new LoginForm();
				string user;
				do
				{
					user = loginForm.Fill();
					if (loginForm.Quit) return;
				} while (user == string.Empty);
				Configuration.User = user;
			}
			//deklarace promennych
			List<EplFile> selectedEplFiles;
			List<EplFile> eplFiles;
			//nacteni epl prikazu
			if (string.IsNullOrEmpty(Configuration.MasterTemplateAddress))
			{
				eplFiles = Configuration.SearchedText == "" ?
				   new EplFileLoader().LoadFiles(Configuration.TemplatesDirectory) :
				   new EplFileLoader().LoadFiles(Configuration.TemplatesDirectory, Configuration.SearchedText);
			}
			else
				eplFiles = new EplFileLoader().ReadFromFile(Configuration.PrimaryDataAdress, Configuration.MasterTemplateAddress);
			//Program muze bezet ve 3 modech - tisk pouze jednoho souboru podle konfiguraku
			//                               - vyber ze souboru vyfiltrovanych jiz v konfiguraku
			//                               - vyber ze vsech souboru v zadanem adresari
			//Tisk pouze jednoho souboru
			if (Configuration.PrintOneFile)
			{
				try
				{
					EplFile eplFile = eplFiles[0];
					Parser parser = new Parser();
					//new Parser1().Process(ref eplFile);
					if (Configuration.Repeate)
					{
						int i = 0;
						while (i < 20)
						{
							parser.Process(ref eplFile);
							if (eplFile.print)
								Printer.PrintLabel(eplFile.Body);
							else return;
							i++;
						}
					}
					else
					{
						parser.Process(ref eplFile);
						if (eplFile.print)
							Printer.PrintLabel(eplFile.Body);
					}
				}
				catch (Exception ex)
				{
					if (ex is ArgumentOutOfRangeException)
						ErrorHandler.HandleError("Program",
						 new IndexOutOfRangeException(string.Format("File not found \"{0}\"!",
						  Path.GetFullPath(Path.Combine(Configuration.TemplatesDirectory, Configuration.SearchedText)))));
					else
						ErrorHandler.HandleError("Program", ex);
				}
			}
			//vyber z eplFiles definovanych pomoci souboru primaryData
			else if (!string.IsNullOrEmpty(Configuration.MasterTemplateAddress))
			{
				if (eplFiles.Count == 0)
				{
					string message = $"No epl file can be loaded from {Configuration.PrimaryDataAdress}";
					ErrorHandler.HandleError("Program", new FileNotFoundException(message));
				}
				var eplFilesDistincted = eplFiles.Distinct().ToList();
				Parser parser = new Parser();
				selectedEplFiles = new SelectFromList<EplFile>().Select(eplFilesDistincted);
				while (selectedEplFiles != null)
				{
					var selectedEplFile = selectedEplFiles[0];
					selectedEplFiles = eplFiles.FindAll(e => e.Equals(selectedEplFile)).Select(e => e.Copy()).ToList();
					foreach (var eplFile in selectedEplFiles)
					{
						string firstValue = eplFile.KeyValuePairs.Values.ElementAt(0);
						string secondValue = eplFile.KeyValuePairs.Values.ElementAt(1);
						eplFile.FileName = $"{firstValue} {secondValue}";
					}
					selectedEplFiles = new SelectFromList<EplFile>().Select(selectedEplFiles);
					if (selectedEplFiles != null)
					{
						for (int i = 0; i < selectedEplFiles.Count; i++)
						{
							EplFile currentEplFile = selectedEplFiles[i];
							parser.Process(ref currentEplFile);
							if (currentEplFile.print)
								Printer.PrintLabel(currentEplFile.Body);
							else currentEplFile.print = true; //reset
						}
					}
					selectedEplFiles = new SelectFromList<EplFile>().Select(eplFilesDistincted);
				}
			}
			else //vyber ze vsech sablon pomoci tridy SelectList
			{
				if (eplFiles.Count == 0)
				{
					string message = string.IsNullOrEmpty(Configuration.SearchedText) ? string.Format("No file found in directory \"{0}\"!", Configuration.TemplatesDirectory)
					 : string.Format("No file containing \"{0}\" in directory \"{1}\"!", Configuration.SearchedText, Configuration.TemplatesDirectory);
					ErrorHandler.HandleError("Program", new FileNotFoundException(message));
				}
				SelectFromList<EplFile> selectList = new SelectFromList<EplFile>();
				Parser parser = new Parser();
				selectedEplFiles = selectList.Select(eplFiles);
				while (selectedEplFiles != null)
				{
					for (int i = 0; i < selectedEplFiles.Count; i++)
					{
						EplFile currentEplFile = selectedEplFiles[i];
						parser.Process(ref currentEplFile);
						if (currentEplFile.print)
							Printer.PrintLabel(currentEplFile.Body);
						else currentEplFile.print = true; //reset
					}
					selectedEplFiles = selectList.Select(eplFiles);
				}
			}
		}
	}
}

