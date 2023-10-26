using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

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
		public void EditujSablony()
		{
			Process externiProces = new Process();
			//externiProces.StartInfo.FileName = "pcmanfm";
			//externiProces.StartInfo.FileName = "explorer";
			externiProces.StartInfo.FileName = @"C:\Program Files (x86)\FreeCommander\FreeCommander.exe";
			externiProces.StartInfo.Arguments = Path.GetFullPath(Konfigurace.Adresar);
			externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			externiProces.Start();
			//externiProces.WaitForExit();
			//u exploreru nafunguje
			//UzivRozhrani.Oznameni(" Tisk štítků na EPL tiskárně", " Editace šablon.", " Pro pokračování stiskni libovolnou klávesu...");
		}
		public void EditujKonfSoubor()
		{
			Process externiProces = new Process();
			externiProces.StartInfo.FileName = "Notepad.exe";
			//externiProces.StartInfo.FileName = "mousepad";
			externiProces.StartInfo.Arguments = Path.GetFullPath(Path.Combine(Konfigurace.KonfiguracniSoubor));
			externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			externiProces.Start();
			//externiProces.WaitForExit();
		}
		public void EditujData()
		{
			try
			{
				Process externiProces = new Process();
				externiProces.StartInfo.FileName = "Notepad.exe";
				//externiProces.StartInfo.FileName = "mousepad";
				//externiProces.StartInfo.FileName = "leafpad";
				externiProces.StartInfo.Arguments = Path.GetFullPath(Konfigurace.AdresaDat);
				externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
				externiProces.Start();
				//externiProces.WaitForExit();
			}
			catch (Exception ex)
			{
				UzivRozhrani.OznameniChyby(" Tisk štítků na EPL tiskárně",
							   " Při pokusu o otevření souboru dat došlo k chybě:" + Environment.NewLine + ex.Message,
							   " Pokračuj stisknutím libovolné klávesy");
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
			else UzivRozhrani.OznameniChyby(" Tisk štítků na EPL tiskárně", " Nenašel jsem soubor s nápovědou:" + Path.GetFullPath(adrReadMe), " Pokračuj stisknutím libovolné klávesy...");
		}
		private void Restart()
		{
			//vyresetovani listu dat pri restartu
			SouborUloh.Data = null;
			//novy beh programu
			Program.Run(new string[] { Konfigurace.KonfiguracniSoubor });
			//aby stary beh nepokracoval
			Environment.Exit(0);
		}
	}
}