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
				nh = Konfigurace.Nacti("conf.txt");
			}
			else
			{
				nh = Konfigurace.Nacti(args[0]);
			}
			if (nh == 1) Konfigurace.Nacti(Konfigurace.KonfiguracniSoubor);
			DatabazePrikazu databazePrikazu = new DatabazePrikazu(Konfigurace.Adresar);
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
					EplPrikaz eplPrikaz = databazePrikazu.Vyber(hledanyText);
					if (eplPrikaz != null)
					{
						Dotazy dotazy = new Dotazy();
						List<Dotaz> listDotazu = dotazy.GenerujListDotazu(eplPrikaz.Telo);
						int pocetStitku = 1;
						foreach (Dotaz dotaz in listDotazu)
						{
							if (dotaz.Otazka == "počet štítků")
							{
								pocetStitku = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(eplPrikaz.NazevSouboru), " Zadej počet štítků od 1 do 20 (přednastaveno 1): ", 0, 20, 1);
								dotaz.Odpoved = pocetStitku.ToString();
							}
							else
							{
								dotaz.Odpoved = UzivRozhrani.VratText(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(eplPrikaz.NazevSouboru), " Zadej " + dotaz.Otazka + ": ", "");
							}
						}
						eplPrikaz.UpravTelo(listDotazu);
						if (pocetStitku != 0)
						{
							if (Konfigurace.TypTiskarny == 0)
								TiskSdilenaTiskarna.Print(Konfigurace.AdresaTiskarny, eplPrikaz.Telo);
							if (Konfigurace.TypTiskarny == 1)
								TiskLokalniTiskarna.SendStringToPrinter(Konfigurace.AdresaTiskarny, eplPrikaz.Telo);
							if (Konfigurace.TypTiskarny == 2)
								TiskIPTiskarna.TiskniStitek(Konfigurace.AdresaTiskarny, eplPrikaz.Telo);
						}
					}
				}
			} while (!string.IsNullOrEmpty(hledanyText));
		}
	}
}
