using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Form;

namespace TiskStitku
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
			if (Configuration.Prihlasit)
			{
				LoginForm loginForm = new LoginForm();
				string user;
				do
				{
					user = loginForm.Fill();
					if (loginForm.Quit) return;
				} while (user == string.Empty);
				Configuration.Uzivatel = user;
			}
			//deklarace promennych
			List<EplFile> eplPrikazy;
			//nacteni epl prikazu
			List<EplFile> eplFiles = Configuration.HledanyText == "" ? new EplFileLoader().LoadFiles(Configuration.TemplatesDirectory) : new EplFileLoader().LoadFiles(Configuration.TemplatesDirectory, Configuration.HledanyText);

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
								Tisk.TiskniStitek(eplFile.Telo);
							else return;
							i++;
						}
					}
					else
					{
						parser.Process(ref eplFile);
						if (eplFile.print)
							Tisk.TiskniStitek(eplFile.Telo);
					}
				}
				catch (Exception ex)
				{
					if (ex is ArgumentOutOfRangeException)
						ErrorHandler.HandleError("Program",
						 new IndexOutOfRangeException(string.Format("File not found \"{0}\"!",
						  Path.GetFullPath(Path.Combine(Configuration.TemplatesDirectory, Configuration.HledanyText)))));
					else
						ErrorHandler.HandleError("Program", ex);
				}
			}
			else //vyber pomoci tridy SelectList
			{
				if (eplFiles.Count == 0)
				{
					ErrorHandler.HandleError("Program",
					 new FileNotFoundException(string.Format("No file containing \"{0}\" in directory \"{1}\"!",
					  Configuration.HledanyText, Configuration.TemplatesDirectory)));
				}
				SelectFromList<EplFile> selectList = new SelectFromList<EplFile>();
				//Parser parser=new Parser();
				Parser parser = new Parser();
				eplPrikazy = selectList.Select(eplFiles);
				while (eplPrikazy != null)
				{
					for (int i = 0; i < eplPrikazy.Count; i++)
					{
						EplFile currentEplFile=eplPrikazy[i];
						parser.Process(ref currentEplFile);
						if (currentEplFile.print)
							Tisk.TiskniStitek(currentEplFile.Telo);
						else currentEplFile.print = true; //reset
					}
					eplPrikazy = selectList.Select(eplFiles);
				}
			}
		}
	}
}

