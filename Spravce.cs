using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using Form;

namespace TiskStitku
{
	public class Spravce
	{
		public void Rozhrani()
		{
			string telo = " Správce nastavení:"
				   + Environment.NewLine
				   + "\t 1. Editace souboru s daty"
				   + Environment.NewLine
				   + "\t 2. Editace šablon"
				   + Environment.NewLine
				   + "\t 3. Editace konfiguračního souboru"
				   + Environment.NewLine
				   + "\t 4. Zobrazení nápovědy";
			int volba;
			do
			{
				volba = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", telo, " Vyber akci zadáním čísla 1 - 4 (prázdný vstup = návrat): ", 1, 4, 0);
				if (volba == 1) EditujData();
				else if (volba == 2) EditujSablony();
				else if (volba == 3) EditujKonfSoubor();
				else if (volba == 4) ZobrazReadme();
			} while (volba != 0);
			Restart();
		}
		public void RozhraniNew()
		{
			List<string> seznamVoleb = new List<string>() { "Editace souboru s daty", "Editace šablon", "Editace konfiguračního souboru", "Nápověda" };
			string volba;
			SelectFromList<string> selecList = new SelectFromList<string>();
			do
			{
				volba = selecList.Select(seznamVoleb);
				switch (seznamVoleb.IndexOf(volba))
				{
					case 1:
						EditujData();
						break;
					case 2:
						EditujSablony();
						break;
					case 0:
						EditujKonfSoubor();
						break;
					case 3:
						ZobrazReadme();
						break;
					default:
						break;
				}

			} while (volba != null);
			Restart();
		}
		public void EditujSablony()
		{
			//RunExternalProcess("explorer",Path.GetFullPath(Configuration.TemplatesDirectory));
			RunExternalProcess(@"C:\Program Files (x86)\FreeCommander\FreeCommander.exe", Path.GetFullPath(Configuration.TemplatesDirectory));
		}
		public void EditujKonfSoubor(bool waitForExit = false)
		{
			RunExternalProcess(path: Path.GetFullPath(Path.Combine(Configuration.ConfigPath, Configuration.ConfigFile)), waitForExit: waitForExit);
		}
		public void EditujData()
		{
			RunExternalProcess(path: Configuration.PrimaryDataAdress);
		}
		public void ZobrazReadme()
		{
			//string adrReadMe = @".\Readme.txt";
			string adrReadMe = @".\zdrojak\Readme.txt";
			if (File.Exists(adrReadMe))
			{
				RunExternalProcess(path: Path.GetFullPath(adrReadMe));
			}
			else ErrorHandler.HandleError(this, new FileNotFoundException(" Nenašel jsem soubor s nápovědou:" + Path.GetFullPath(adrReadMe)));
		}
		public void Restart()
		{
			//novy beh programu
			Program.Run(new string[] { Configuration.ConfigFile });
			//aby stary beh nepokracoval
			Environment.Exit(0);
		}
		// private void RunExternalProcess(string command = "notepad.exe", string path = "", bool waitForExit = false)
		// {
		// 	Process process = new Process();
		// 	process.StartInfo.FileName = command;
		// 	process.StartInfo.Arguments = Path.GetFullPath(path);
		// 	process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
		// 	if (waitForExit) process.WaitForExit();
		// 	process.Start();
		// }
		private void RunExternalProcess(string command = "notepad.exe", string path = "", bool waitForExit = false)
		{
			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.FileName = command;
					process.StartInfo.Arguments = Path.GetFullPath(path);
					process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

					bool isStarted = process.Start();

					if (isStarted)
					{
						if (waitForExit)
						{
							process.WaitForExit(6000000); // Wait for the process to exit with a timeout
						}
					}
					else
					{
						Console.WriteLine("The process could not be started.");
					}
				}
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleError(this, ex);
			}
		}

	}
}