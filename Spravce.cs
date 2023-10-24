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
			string telo = " Správce:"
				   + Environment.NewLine
				   + "\t 1. Editace souboru s daty"
				   + Environment.NewLine
				   + "\t 2. Editace šablon"
				   + Environment.NewLine
				   + "\t 3. Editace konfiguračního souboru";
			int volba = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", telo, " Vyber akci zadáním čísla: 1 - 5: ", 1, 5, 0);
			if (volba == 2) EditujSablony();
			else if (volba == 3) EditujKonfSoubor();
			else if (volba == 1) EditujData();
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
			externiProces.WaitForExit(); //u exploreru nafunguje
										 //UzivRozhrani.Oznameni(" Tisk štítků na EPL tiskárně", " Editace šablon.", " Pro pokračování stiskni libovolnou klávesu...");
			Restart();
		}
		public void EditujKonfSoubor()
		{
			Process externiProces = new Process();
			externiProces.StartInfo.FileName = "Notepad.exe";
			//externiProces.StartInfo.FileName = "mousepad";
			//externiProces.StartInfo.FileName = "leafpad";
			externiProces.StartInfo.Arguments = Path.GetFullPath(Path.Combine(Konfigurace.KonfiguracniSoubor));
			externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			externiProces.Start();
			externiProces.WaitForExit();
			Restart();
		}
		public void EditujData()
		{
			Process externiProces = new Process();
			externiProces.StartInfo.FileName = "Notepad.exe";
			//externiProces.StartInfo.FileName = "mousepad";
			//externiProces.StartInfo.FileName = "leafpad";
			externiProces.StartInfo.Arguments = Path.GetFullPath(Konfigurace.AdresaDat);
			externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			externiProces.Start();
			externiProces.WaitForExit();
			//if (!string.IsNullOrEmpty(Konfigurace.AdresaDat)) SouborUloh.NactiData();
			Restart();

		}
		private void Restart()
		{
			Program.Run(new string[] { Konfigurace.KonfiguracniSoubor });
			Environment.Exit(0);
		}
	}
}