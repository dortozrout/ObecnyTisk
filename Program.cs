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
			//nacteni epl prikazu z adresare
			if (string.IsNullOrEmpty(Configuration.MasterTemplateAddress))
			{
				eplFiles = Configuration.SearchedText == "" ?
				   new EplFileLoader().LoadFiles(Configuration.TemplatesDirectory) :
				   new EplFileLoader().LoadFiles(Configuration.TemplatesDirectory, Configuration.SearchedText);
			}
			else //ze souboru definovaneho v Configuration.MasterTemplateInputAddress
				eplFiles = new EplFileLoader().ReadFromFile(Configuration.MasterTemplateInputAddress, Configuration.MasterTemplateAddress);
			//Program muze bezet ve 3 modech - tisk pouze jednoho souboru podle konfiguraku
			//                               - vyber ze vsech souboru v zadanem adresari (mozny filtr)
			//                               - tisk vice souboru podle jedne sablony (hlavniSablona, hlaviSablonaData)
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
			else //vyber ze vsech sablon pomoci tridy SelectList
			{
				if (eplFiles.Count == 0)
				{
					string message = "No EPL files or EPL data found!";
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

