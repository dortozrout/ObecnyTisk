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
				//nejprve se hleda odpoved v souboru s daty (Dotazy.Data)
				if (Dotazy.Data != null)
					dataVyhledany = Dotazy.Data.Where(x => x.Otazka.Equals(dotaz.Otazka.ToLower())).ToList();
				if (dataVyhledany.Count != 0)
				{
					dotaz.Odpoved = dataVyhledany[0].Odpoved;
				}
				//sablona s P na konci
				else if (dotaz.Otazka == "počet štítků")
				{
					pocetStitku = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(NazevSouboru), " Zadej počet štítků od 1 do 20 (přednastaveno 1): ", 0, 20, 1);
					dotaz.Odpoved = pocetStitku.ToString();
				}
				//otazka typu <pocet|12> prednastaveny pocet 12
				else if (dotaz.Otazka.Contains("pocet|"))
				{
					string[] pole = dotaz.Otazka.Split('|');
					int prednastPocet = 1;
					int.TryParse(pole[1], out prednastPocet);
					pocetStitku = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(NazevSouboru), " Zadej počet štítků od 1 do 30 (přednastaveno " + prednastPocet + "): ", 0, 30, prednastPocet);
					dotaz.Odpoved = pocetStitku.ToString();
				}
				//uzivatel
				else if (Konfigurace.Prihlasit && dotaz.Otazka == "uzivatel")
				{
					dotaz.Odpoved = Konfigurace.Uzivatel;
				}
				//dotaz na datum nebo cas
				else if (DatumNCas(dotaz.Otazka))
				{
					string otazka = dotaz.Otazka;
					if (otazka.Contains('|'))//otazka hlidajici prekroceni expirace sarze
					{
						string dotazNaExpiraci = otazka.Split('|')[1];
						DateTime expirace;
						string zjistenaExpirace = "";
						if (DateTime.TryParse(dotazNaExpiraci, out expirace))
						{
							zjistenaExpirace = expirace.ToString("dd.MM.yyyy");
						}
						else
						{
							if (Dotazy.Data != null)
								dataVyhledany = Dotazy.Data.Where(x => x.Otazka.Equals(dotazNaExpiraci.ToLower())).ToList();
							if (dataVyhledany.Count != 0)
								zjistenaExpirace = dataVyhledany[0].Odpoved;
						}
						otazka = otazka.Replace(dotazNaExpiraci, zjistenaExpirace);
					}
					dotaz.Odpoved = VratDatumNCas(otazka);
				}
				//pokud se nenajde prednastavena odpoved polozi se otazka uzivateli
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
			DateTime expiraceSarze = DateTime.MaxValue;
			string[] castiOtazky = otazka.Split(new char[] { '+', '|' });
			sCas = castiOtazky[0];
			if (castiOtazky.Length > 1) //Cas s posunem
			{
				if (!double.TryParse(castiOtazky[1], out Posun))
					Posun = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(NazevSouboru), " Zadej " + castiOtazky[1] + ": ", 0, int.MaxValue, 0);
			}
			if (castiOtazky.Length > 2) //Cas s posunem a expiraci
				DateTime.TryParse(castiOtazky[2], out expiraceSarze);
			DateTime cas = DateTime.Now;
			if (sCas.Equals("date"))
			{
				cas = cas.AddDays(Posun);
				odpoved = cas < expiraceSarze ? cas.ToString("d.M.yyyy") : expiraceSarze.ToString("d.M.yyyy");
			}
			else odpoved = cas.AddMinutes(Posun).ToString("H:mm");
			return odpoved;
		}
	}
}