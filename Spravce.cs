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
			string telo = " Konfigurační soubor: " + Konfigurace.KonfiguracniSoubor + Environment.NewLine
						+ " Adresa tiskárny: " + Konfigurace.AdresaTiskarny + Environment.NewLine
						+ " Typ tiskárny: " + Konfigurace.TypTiskarnySlovy + Environment.NewLine
						+ " Adresář se soubory: " + Path.GetFullPath(Konfigurace.Adresar) + Environment.NewLine
						+ " Kódování souborů: " + Konfigurace.Kodovani + Environment.NewLine
						+ " " + RuntimeInformation.FrameworkDescription;
			//hledanyText = UzivRozhrani.VratText(" Tisk štítků na EPL tiskárně", telo, " Zadej část názvu hledaného souboru" + Environment.NewLine + " nebo * pro zobrazení všech souborů " + Environment.NewLine + " (prázdný vstup ukončí program): ", "");
			int volba = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", telo, " Správce:"
				   + Environment.NewLine
				   + "\t 1. Editace šablon"
				   + Environment.NewLine
				   + "\t 2. Editace konfiguračního souboru"
				   + Environment.NewLine
				   + " Vyber akci zadáním čísla: 1 - 5: ", 1, 5, 0);
			if (volba == 1) EditujSablony();
			else if (volba == 2) EditujKonfSoubor();
		}
		public void EditujSablony()
		{
			Process externiProces = new Process();
			externiProces.StartInfo.FileName = "pcmanfm";
			//externiProces.StartInfo.FileName = "mousepad";
			//externiProces.StartInfo.FileName = "leafpad";
			externiProces.StartInfo.Arguments = Path.GetFullPath(Konfigurace.Adresar);
			externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			externiProces.Start();
			externiProces.WaitForExit();
			SouborEplPrikazu.NactiEplSablony(Konfigurace.Adresar);
		}
		public void EditujKonfSoubor()
		{
			Process externiProces = new Process();
			//externiProces.StartInfo.FileName = "Notepad.exe";
			externiProces.StartInfo.FileName = "mousepad";
			//externiProces.StartInfo.FileName = "leafpad";
			externiProces.StartInfo.Arguments = Path.GetFullPath(Path.Combine(Konfigurace.KonfiguracniSoubor));
			externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			externiProces.Start();
			externiProces.WaitForExit();
			Konfigurace.Nacti(Konfigurace.KonfiguracniSoubor);
		}
		public void EditujData()
		{
			Process externiProces = new Process();
			externiProces.StartInfo.FileName = "Notepad.exe";
			//externiProces.StartInfo.FileName = "mousepad";
			//externiProces.StartInfo.FileName = "leafpad";
			//externiProces.StartInfo.Arguments = Path.GetFullPath(Path.Combine(cesta, konfiguracniSoubor));
			externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			externiProces.Start();
			externiProces.WaitForExit();
		}
	}
}