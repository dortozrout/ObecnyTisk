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
			Process externiProces = new Process();
			//externiProces.StartInfo.FileName = "pcmanfm";
			externiProces.StartInfo.FileName = "explorer";
			//externiProces.StartInfo.FileName = @"C:\Program Files (x86)\FreeCommander\FreeCommander.exe";
			externiProces.StartInfo.Arguments = Path.GetFullPath(Configuration.TemplatesDirectory);
			externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			externiProces.Start();
			//externiProces.WaitForExit();
			//u exploreru nafunguje
			//UzivRozhrani.Oznameni(" Tisk štítků na EPL tiskárně", " Editace šablon.", " Pro pokračování stiskni libovolnou klávesu...");
		}
		public void EditujKonfSoubor(bool waitForExit = false)
		{
			Process externiProces = new Process();
			externiProces.StartInfo.FileName = "Notepad.exe";
			//externiProces.StartInfo.FileName = "mousepad";
			externiProces.StartInfo.Arguments = Path.GetFullPath(Path.Combine(Configuration.ConfigPath, Configuration.ConfigFile));
			externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			externiProces.Start();
			if (waitForExit) externiProces.WaitForExit();
		}
		public void EditujData()
		{
			try
			{
				Process externiProces = new Process();
				externiProces.StartInfo.FileName = "Notepad.exe";
				//externiProces.StartInfo.FileName = "mousepad";
				//externiProces.StartInfo.FileName = "leafpad";
				externiProces.StartInfo.Arguments = Path.GetFullPath(Configuration.PrimaryDataAdress);
				externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
				externiProces.Start();
				//externiProces.WaitForExit();
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleError(this, ex);
				// UzivRozhrani.OznameniChyby(" Tisk štítků na EPL tiskárně",
				// 			   " Při pokusu o otevření souboru dat došlo k chybě:" + Environment.NewLine + ex.Message,
				// 			   " Pokračuj stisknutím libovolné klávesy");
			}
		}
		public void ZobrazReadme()
		{
			//string adrReadMe = @".\Readme.txt";
			string adrReadMe = @".\zdrojak\Readme.txt";
			if (File.Exists(adrReadMe))
			{
				Process externiProces = new Process();
				externiProces.StartInfo.FileName = "Notepad.exe";
				//externiProces.StartInfo.FileName = "mousepad";
				externiProces.StartInfo.Arguments = Path.GetFullPath(adrReadMe);
				externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
				externiProces.Start();
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
	}
}