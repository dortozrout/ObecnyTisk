using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace TiskStitku
{
	class EplPrikaz
	{
		public string NazevSouboru { get; private set; }
		public string Telo { get; private set; }
		//public List<Dotaz> SeznamDotazu { get; set; }
		public EplPrikaz(string nazevSouboru, string telo) //konstruktor
		{
			NazevSouboru = nazevSouboru;
			Telo = telo;
		}
		//Pokud eplPrikaz je sablona tj. obsahuje <vzor> nebo P na konci
		//souboru, je treba doplnit potrebne udaje
		public int VyplnSablonu()
		{
			Dotazy dotazy = new Dotazy();
			List<Dotaz> listDotazu = dotazy.GenerujListDotazu(Telo);
			int pocetStitku = 1;
			foreach (Dotaz dotaz in listDotazu) //ziskani odpovedi
			{
				if (dotaz.Otazka == "počet štítků")
				{
					pocetStitku = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(NazevSouboru), " Zadej počet štítků od 1 do 20 (přednastaveno 1): ", 0, 20, 1);
					dotaz.Odpoved = pocetStitku.ToString();
				}
				else
				{
					dotaz.Odpoved = UzivRozhrani.VratText(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(NazevSouboru), " Zadej " + dotaz.Otazka + ": ", "");
				}
			}
			foreach (Dotaz dotaz in listDotazu) //vepsani odpovedi do sablony
			{
				if (dotaz.Otazka == "počet štítků")
					Telo = Telo.TrimEnd(new char[] { '\r', '\n' }) + dotaz.Odpoved + Environment.NewLine;
				else Telo = Telo.Replace("<" + dotaz.Otazka + ">", "\"" + dotaz.Odpoved + "\"");
			}
			if (pocetStitku != 0) return 0; //pokud je pocet stitku nenulovy
			else return 1; //jinak vraci 1
		}
	}
}