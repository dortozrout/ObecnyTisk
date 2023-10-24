using System;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace TiskStitku
{
	class Konfigurace
	{
		//Adresa konfig souboru
		public static string KonfiguracniSoubor { get; private set; }

		//Adresář se soubory obsahujícími EPL příkazy
		public static string Adresar { get; private set; }
		//Vyraz pro vyhledani jednoho nebo nekolika epl prikazu v adresari
		public static string HledanyText { get; private set; }
		//Tisk pouze jednoho souboru?
		public static bool JedenSoubor { get; private set; }
		//Opakovany tisk 1 souboru?
		public static bool OpakovanyTisk { get; private set; }
		//Kodovani souborů
		public static string Kodovani { get; private set; }
		//ip adresa nebo jméno tiskárny
		public static string AdresaTiskarny { get; private set; }
		//typ tiskarny - lokalni, sdilena nebo sitova
		public static int TypTiskarny { get; private set; }
		public static string TypTiskarnySlovy { get; private set; }
		//jestli vyzadovat zadani login
		public static bool Prihlasit { get; private set; }
		public static string Uzivatel { get; private set; }
		public static string AdresaDat { get; private set; }
		public const string Editace = "edit";
		public static int Nacti(string konfiguracniSoubor) //jmeno konfig souboru v adresari %appdata%/TiskStitku
		{
			HledanyText = "";
			int navratovaHodnota = 0;
			string cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TiskStitku");
			if (!Directory.Exists(cesta))
				Directory.CreateDirectory(cesta);
			if (File.Exists(Path.Combine(cesta, konfiguracniSoubor))) //pokud konfig soubor existuje
			{
				string[] konfArray = File.ReadAllLines(Path.Combine(cesta, konfiguracniSoubor));
				bool jedenSoubor = false;
				bool opakovanyTisk = false;
				bool prihlasit = false;
				foreach (string s in konfArray)
				{
					if (!s.StartsWith("#")) //ignoruje radky zakomentovane pomoci #
					{
						if (s.ToLower().Contains("iptiskarny:")) //tiskarna
							AdresaTiskarny = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("typtiskarny:")) //tiskarna
							TypTiskarny = int.Parse(s.Substring(s.IndexOf(':') + 1).Trim());
						if (s.ToLower().Contains("adresar:")) //adresar se soubory epl prikazu
							Adresar = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("hledanytext:")) //hledany text v nazvu souboru
							HledanyText = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("jedensoubor:")) //true pokud se ma vytisknout pouze jeden soubor
							bool.TryParse(s.Substring(s.IndexOf(':') + 1).Trim(), out jedenSoubor);
						if (s.ToLower().Contains("opakovanytisk:")) //true pkud se jeden soubor ma tisknout opakovane
							bool.TryParse(s.Substring(s.IndexOf(':') + 1).Trim(), out opakovanyTisk);
						if (s.ToLower().Contains("kodovani:")) //kodovani souboru sablon epl prikazu
							Kodovani = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("prihlasit:")) //kodovani souboru sablon epl prikazu
							bool.TryParse(s.Substring(s.IndexOf(':') + 1).Trim(), out prihlasit);
						if (s.ToLower().Contains("data:")) //dvojice globalnich dat otazka-odpoved
							AdresaDat = s.Substring(s.IndexOf(':') + 1).Trim();
					}
				}
				JedenSoubor = jedenSoubor;
				OpakovanyTisk = opakovanyTisk;
				try
				{
					Encoding.GetEncoding(Kodovani);
				}
				catch (Exception ex)
				{
					UzivRozhrani.OznameniChyby("  Tisk štítků na EPL tiskárně",
					" Kódování \"" + Kodovani + "\" uvedené v konfiguračním souboru není platné." + Environment.NewLine +
					" Popis chyby: " + Environment.NewLine +
					ex.Message + Environment.NewLine +
					" Program bude ukončen.",
					" Pokračuj stisknutím libovolné klávesy.");
					Environment.Exit(-1);
				}
				Prihlasit = prihlasit;
				if (prihlasit)
				{
					do
					{
						Uzivatel = UzivRozhrani.VratText("  Tisk štítků na EPL tiskárně", " Je vyžadována identifikace uživatele.", " Zadej login: ", "");
					} while (string.IsNullOrWhiteSpace(Uzivatel));
				}
				if (!Directory.Exists(Adresar))
				{
					UzivRozhrani.Oznameni("  Tisk štítků na EPL tiskárně",
					" Adresář se šablonami uvedený v konfiguračním souboru neexistuje " + Environment.NewLine +
					//" " + Path.GetFullPath(Path.Combine(cesta, konfiguracniSoubor)) + Environment.NewLine +
					" Konfigurační soubor bude otevřen v editoru, uprav ho podle svých potřeb.",
					" Pokračuj stisknutím libovolné klávesy.");
					Process externiProces = new Process();
					//externiProces.StartInfo.FileName = "Notepad.exe";
					externiProces.StartInfo.FileName = "mousepad";
					externiProces.StartInfo.Arguments = Path.GetFullPath(Path.Combine(cesta, konfiguracniSoubor));
					externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
					externiProces.Start();
					externiProces.WaitForExit();
					navratovaHodnota = 1;
				}
				if (TypTiskarny == 0)
					TypTiskarnySlovy = "sdílená tiskárna";
				if (TypTiskarny == 1)
					TypTiskarnySlovy = "lokální tiskárna";
				if (TypTiskarny == 2)
					TypTiskarnySlovy = "síťová tiskárna";
				if (TypTiskarny == 3)
					TypTiskarnySlovy = "výstup na obrazovku";
			}
			else //pokud konfig soubor neexistuje, vytvori novy
			{
				File.WriteAllText(Path.Combine(cesta, konfiguracniSoubor),
					"# IP adresa nebo jmeno tiskarny" + Environment.NewLine +
					"IPtiskarny:" + Environment.NewLine +
					"# typ tiskarny 0 - sdilena, 1 - mistni, 2 - sitova, 3 - výstup na obrazovku" + Environment.NewLine +
					"TypTiskarny:3" + Environment.NewLine +
					"# adresar souboru s epl prikazy" + Environment.NewLine +
					"adresar:." + Environment.NewLine +
					"# text ktery se hleda v nazvu souboru" + Environment.NewLine +
					"hledanyText:" + Environment.NewLine +
					"# jestli se ma tisknout jenom jeden soubor" + Environment.NewLine +
					"jedenSoubor:false" + Environment.NewLine +
					"# jeden soubor se tiskne opakovane" + Environment.NewLine +
					"opakovanytisk:false" + Environment.NewLine +
					"# kodovani ulozenych souboru (UTF-8 nebo windows-1250)" + Environment.NewLine +
					"kodovani:UTF-8" + Environment.NewLine +
					"# zda vyzadovat login" + Environment.NewLine +
					"prihlasit:false" + Environment.NewLine +
					"# adresa souboru s daty" + Environment.NewLine +
					"data:" + Environment.NewLine);
				UzivRozhrani.Oznameni("  Tisk štítků na EPL tiskárně",
					" První spuštění programu s konfiguračním souborem " + Environment.NewLine +
					" " + Path.GetFullPath(Path.Combine(cesta, konfiguracniSoubor)) + Environment.NewLine +
					" Konfigurační soubor bude otevřen v editoru, uprav ho podle svých potřeb.",
					" Pokračuj stisknutím libovolné klávesy.");
				Process externiProces = new Process();
				externiProces.StartInfo.FileName = "Notepad.exe";
				//externiProces.StartInfo.FileName = "mousepad";
				//externiProces.StartInfo.FileName = "leafpad";
				externiProces.StartInfo.Arguments = Path.GetFullPath(Path.Combine(cesta, konfiguracniSoubor));
				externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
				externiProces.Start();
				externiProces.WaitForExit();
				navratovaHodnota = 1;
			}
			KonfiguracniSoubor = Path.GetFullPath(Path.Combine(cesta, konfiguracniSoubor));
			//nacteni konfigurace pokud byl konfig soubor nove vytvoren 
			if (navratovaHodnota == 1) Nacti(KonfiguracniSoubor);
			return navratovaHodnota;
		}
	}
}