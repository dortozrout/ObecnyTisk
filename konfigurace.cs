using System;
using System.IO;

namespace TiskStitku
{
	class Konfigurace
	{
		//Adresa konfig souboru
		public static string KonfiguracniSoubor { get; private set; }

		//Adresář se soubory obsahujícími EPL příkazy
		public static string Adresar { get; private set; }

		//Kodovani souborů
		public static string Kodovani { get; private set; }

		//ip adresa nebo jméno tiskárny
		public static string AdresaTiskarny { get; private set; }
		public static int TypTiskarny { get; private set; }
		public static string TypTiskarnySlovy { get; private set; }
		public static void Nacti(string konfiguracniSoubor) //jmeno konfig souboru v adresari %appdata%/TiskStitku
		{
			string cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TiskStitku");
			if (!Directory.Exists(cesta))
				Directory.CreateDirectory(cesta);
			if (File.Exists(Path.Combine(cesta, konfiguracniSoubor))) //pokud konfig soubor existuje
			{
				string[] konfArray = File.ReadAllLines(Path.Combine(cesta, konfiguracniSoubor));
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
						if (s.ToLower().Contains("kodovani:")) //adresar se soubory epl prikazu
							Kodovani = s.Substring(s.IndexOf(':') + 1).Trim();
					}
				}
				if (TypTiskarny == 0)
					TypTiskarnySlovy = "sdílená tiskárna";
				if (TypTiskarny == 1)
					TypTiskarnySlovy = "lokální tiskárna";
				if (TypTiskarny == 2)
					TypTiskarnySlovy = "síťová tiskárna";
			}
			else //pokud konfig soubor neexistuje, vytvori novy
			{
				File.WriteAllText(Path.Combine(cesta, konfiguracniSoubor),
					"# IP adresa nebo jmeno tiskarny" + Environment.NewLine +
					"IPtiskarny:" + Environment.NewLine +
					"# typ tiskarny 0 - sdilena, 1 - mistni, 2 - sitova" + Environment.NewLine +
					"TypTiskarny:2" + Environment.NewLine +
					"# adresar souboru s epl prikazy" + Environment.NewLine +
					"adresar:." + Environment.NewLine +
					"# kodovani ulozenych souboru (UTF-8 nebo windows-1250)" + Environment.NewLine +
					"kodovani:UTF-8" + Environment.NewLine);
				AdresaTiskarny = "";
				TypTiskarny = 2;
				Adresar = ".";
				Kodovani = "UTF-8";
			}
			KonfiguracniSoubor = Path.GetFullPath(Path.Combine(cesta, konfiguracniSoubor));
		}
	}
}