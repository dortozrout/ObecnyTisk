using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TiskStitku
{
	class EplPrikaz
	{
		public string NazevSouboru { get; private set; }
		public string Sablona { get; private set; }
		public string Telo { get; private set; }
		public List<Dotaz> ListDotazu { get; set; }
		public EplPrikaz(string nazevSouboru, string sablona) //konstruktor
		{
			NazevSouboru = nazevSouboru;
			Sablona = sablona;
		}
		//Pokud eplPrikaz je sablona tj. obsahuje <vzor> nebo P na konci
		//souboru, je treba doplnit potrebne udaje
		public int VyplnSablonu()
		{
			Telo = Sablona;
			Dotazy dotazy = new Dotazy();
			ListDotazu = dotazy.GenerujListDotazu(Telo);
			int pocetStitku = 1;
			//list obsahujicí jeden člen pokud se dotaz.Otazka najde v listu Dotazy.Data
			List<Dotaz> dataVyhledany = new List<Dotaz>();
			foreach (Dotaz dotaz in ListDotazu) //ziskani odpovedi
			{
				if (Dotazy.Data != null)
					dataVyhledany = Dotazy.Data.Where(x => x.Otazka.Equals(dotaz.Otazka.ToLower())).ToList();
				if (dataVyhledany.Count != 0)
				{
					dotaz.Odpoved = dataVyhledany[0].Odpoved;
				}
				else if (dotaz.Otazka == "počet štítků")
				{
					pocetStitku = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(NazevSouboru), " Zadej počet štítků od 1 do 20 (přednastaveno 1): ", 0, 20, 1);
					dotaz.Odpoved = pocetStitku.ToString();
				}
				else if (dotaz.Otazka.Contains("pocet|"))
				{
					string[] pole = dotaz.Otazka.Split('|');
					int prednastPocet = 1;
					int.TryParse(pole[1], out prednastPocet);
					pocetStitku = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(NazevSouboru), " Zadej počet štítků od 1 do 30 (přednastaveno " + prednastPocet + "): ", 0, 30, prednastPocet);
					dotaz.Odpoved = pocetStitku.ToString();
				}
				else if (Konfigurace.Prihlasit && dotaz.Otazka == "uzivatel")
				{
					dotaz.Odpoved = Konfigurace.Uzivatel;
				}
				else if (DatumNCas(dotaz.Otazka))
				{
					dotaz.Odpoved = VratDatumNCas(dotaz.Otazka);
				}
				else
				{
					dotaz.Odpoved = UzivRozhrani.VratText(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(NazevSouboru), " Zadej " + dotaz.Otazka + ": ", "");
				}
			}
			foreach (Dotaz dotaz in ListDotazu) //vepsani odpovedi do sablony
			{
				if (dotaz.Otazka == "počet štítků")
					Telo = Telo.TrimEnd(new char[] { '\r', '\n' }) + dotaz.Odpoved + Environment.NewLine;
				else Telo = Telo.Replace("<" + dotaz.Otazka + ">", dotaz.Odpoved);
			}
			if (pocetStitku != 0) return 0; //pokud je pocet stitku nenulovy
			else return 1; //jinak vraci 1
		}
		//Pomocna funkce, ktera vyhodnoti zda se otazka pta na cas|datum 
		private bool DatumNCas(string otazka)
		{
			bool nh = false;
			otazka = otazka.Split('+')[0];
			otazka = otazka.ToLower();
			if (otazka.Equals("date") || otazka.Equals("time"))
				nh = true;
			return nh;
		}
		//Pomocna funkce, ktera vraci odpoved na casovou otazku
		private string VratDatumNCas(string otazka)
		{
			string odpoved;
			string sCas;
			double Posun = 0;
			//if (otazka.Contains('+'))
			if (otazka.Contains("+"))
			{
				string[] pole = otazka.Split('+');
				sCas = pole[0];
				double.TryParse(pole[1], out Posun);
			}
			else sCas = otazka;
			DateTime cas = DateTime.Now;
			if (sCas.Equals("date"))
			{
				odpoved = cas.AddDays(Posun).ToString("d.M.yyyy");
			}
			else odpoved = cas.AddMinutes(Posun).ToString("H:mm");
			return odpoved;
		}
	}
}