using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TiskStitku
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.Unicode;
			int nh;
			if (args.Length == 0)
			{
				nh=Konfigurace.Nacti("conf.txt");
			}
			else
			{
				nh=Konfigurace.Nacti(args[0]);
			}
			if (nh == 1) Konfigurace.Nacti(Konfigurace.KonfiguracniSoubor);
			DatabazePrikazu databazePrikazu = new DatabazePrikazu(Konfigurace.Adresar);
			List<EplPrikaz> list;
			string hledanyText;
			do
			{
				string telo = " Konfigurační soubor: " + Konfigurace.KonfiguracniSoubor + Environment.NewLine
					+ " Adresa tiskárny: " + Konfigurace.AdresaTiskarny + Environment.NewLine
					+ " Typ tiskárny: " + Konfigurace.TypTiskarnySlovy + Environment.NewLine
					+ " Adresář se soubory: " + Path.GetFullPath(Konfigurace.Adresar) + Environment.NewLine
					+ " Kódování souborů: " + Konfigurace.Kodovani;
				hledanyText = UzivRozhrani.VratText(" Tisk štítků na EPL tiskárně", telo, " Zadej část názvu hledaného souboru" + Environment.NewLine + " nebo * pro zobrazení všech souborů " + Environment.NewLine + " (prázdný vstup ukončí program): ", "");
				if (!string.IsNullOrEmpty(hledanyText))
				{
					if (hledanyText == "*")
					{
						list = databazePrikazu.VratSeznam();
					}
					else
					{
						list = databazePrikazu.VratSeznam(hledanyText);
					}
					telo = "";
					int cislo = 0;
					if (list.Count != 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							telo += string.Format(("{0}.").PadLeft(5) + "\t{1}" + Environment.NewLine, i + 1, Path.GetFileName(list[i].NazevSouboru));
						}
						telo = (telo).TrimEnd('\n');
						cislo = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", telo, " Vyber soubor zadáním čísla (1 - " + list.Count + "): ", 1, list.Count, 0);
					}
					else
					{
						telo = " Nenalezen žádný výskyt \"" + hledanyText + "\"";
						UzivRozhrani.Oznameni(" Tisk štítků na EPL tiskárně", telo, " Pokračuj stisknutím libovolné klávesy.");
					}
					cislo--;
					if (cislo != -1)
					{
						EplPrikaz EplPrikaz1 = list[cislo];
						string prikaz = EplPrikaz1.Telo;
						int pocetStitku = 1;
						if (prikaz.EndsWith("P"))
						{
							pocetStitku = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " vytisknu soubor " + Path.GetFileName(EplPrikaz1.NazevSouboru), " Zadej počet štítků od 1 do 20 (přednastaveno 1): ", 0, 20, 1);
							prikaz += pocetStitku + Environment.NewLine;
						}
						if (pocetStitku != 0)
						{
							if (Konfigurace.TypTiskarny == 0)
								TiskSdilenaTiskarna.Print(Konfigurace.AdresaTiskarny, prikaz);
							if (Konfigurace.TypTiskarny == 1)
								TiskLokalniTiskarna.SendStringToPrinter(Konfigurace.AdresaTiskarny, prikaz);
							if (Konfigurace.TypTiskarny == 2)
								TiskIPTiskarna.TiskniStitek(Konfigurace.AdresaTiskarny, prikaz);
						}
					}
				}
			} while (!string.IsNullOrEmpty(hledanyText));
		}
	}
}