using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TiskStitku
{
	class EplPrikaz
	{
		public string AdresaSouboru { get; private set; }
		public string JmenoSouboru { get; private set; }
		public string Sablona { get; private set; }
		public string Telo { get; private set; }
		public List<Uloha> ListUloh { get; set; }
		public EplPrikaz(string adresaSouboru, string sablona) //konstruktor
		{
			AdresaSouboru = adresaSouboru;
			JmenoSouboru = Path.GetFileName(AdresaSouboru);
			Sablona = sablona;
		}
		//Pokud eplPrikaz je sablona tj. obsahuje <vzor> nebo P na konci
		//souboru, je treba doplnit potrebne udaje, vzoru rikam uloha
		public int VyplnSablonu()
		{
			Telo = Sablona;
			SouborUloh souborUloh = new SouborUloh();
			ListUloh = souborUloh.GenerujListUloh(Telo);
			int pocetStitku = 1;
			//list obsahujicí jeden člen pokud se dotaz.Otazka najde v listu Dotazy.Data
			List<Uloha> dataVyhledana = new List<Uloha>();
			foreach (Uloha dotaz in ListUloh) //ziskani odpovedi
			{
				//nejprve se hleda odpoved v souboru s daty (Dotazy.Data)
				if (SouborUloh.Data != null)
					dataVyhledana = SouborUloh.Data.Where(x => x.Zadani.Equals(dotaz.Zadani.ToLower())).ToList();
				if (dataVyhledana.Count != 0)
				{
					dotaz.Vysledek = dataVyhledana[0].Vysledek;
				}
				//sablona s P na konci
				else if (dotaz.Zadani == "počet štítků")
				{
					pocetStitku = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(AdresaSouboru), " Zadej počet štítků od 1 do 20 (přednastaveno 1): ", 0, 20, 1);
					dotaz.Vysledek = pocetStitku.ToString();
				}
				//otazka typu <pocet|12> prednastaveny pocet 12
				else if (dotaz.Zadani.Contains("pocet|"))
				{
					string[] pole = dotaz.Zadani.Split('|');
					int prednastPocet = 1;
					int.TryParse(pole[1], out prednastPocet);
					pocetStitku = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(AdresaSouboru), " Zadej počet štítků od 1 do 30 (přednastaveno " + prednastPocet + "): ", 0, 30, prednastPocet);
					dotaz.Vysledek = pocetStitku.ToString();
				}
				//uzivatel
				else if (Konfigurace.Prihlasit && dotaz.Zadani == "uzivatel")
				{
					dotaz.Vysledek = Konfigurace.Uzivatel;
				}
				//dotaz na datum nebo cas
				else if (DatumNCas(dotaz.Zadani))
				{
					string otazka = dotaz.Zadani;
					if (otazka.Contains('|'))//otazka hlidajici prekroceni expirace sarze
					{
						string dotazNaExpiraci = otazka.Split('|')[1];
						DateTime expirace;
						string zjistenaExpirace = "";
						if (!DateTime.TryParse(dotazNaExpiraci, out expirace))
						/*{
							zjistenaExpirace = expirace.ToString("dd.MM.yyyy");
						}
						else*/
						{
							if (SouborUloh.Data != null)
								dataVyhledana = SouborUloh.Data.Where(x => x.Zadani.Equals(dotazNaExpiraci.ToLower())).ToList();
							if (dataVyhledana.Count != 0)
							{
								zjistenaExpirace = dataVyhledana[0].Vysledek;
								otazka = otazka.Replace(dotazNaExpiraci, zjistenaExpirace);
							}
						}
					}
					dotaz.Vysledek = VratDatumNCas(otazka);
					if (dotaz.Vysledek == "netisknout") return 1;


				}
				//pokud se nenajde prednastavena odpoved polozi se otazka uzivateli
				else
				{
					dotaz.Vysledek = UzivRozhrani.VratText(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(AdresaSouboru), " Zadej " + dotaz.Zadani + ": ", "");
				}
			}
			foreach (Uloha dotaz in ListUloh) //vepsani odpovedi do sablony
			{
				if (dotaz.Zadani == "počet štítků")
					Telo = Telo.TrimEnd(new char[] { '\r', '\n' }) + dotaz.Vysledek + Environment.NewLine;
				else Telo = Telo.Replace("<" + dotaz.Zadani + ">", dotaz.Vysledek);
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
			string casTyp;
			double Posun = 0;
			DateTime expiraceSarze = DateTime.MaxValue;
			string[] castiOtazky = otazka.Split(new char[] { '+', '|' });
			casTyp = castiOtazky[0];
			if (castiOtazky.Length > 1) //Cas s posunem
			{
				if (!double.TryParse(castiOtazky[1], out Posun))
					Posun = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(AdresaSouboru), " Zadej " + castiOtazky[1] + ": ", 0, int.MaxValue, 0);
			}
			if (castiOtazky.Length > 2) //Cas s posunem a expiraci
				DateTime.TryParse(castiOtazky[2], out expiraceSarze);
			DateTime expiraceVypoctena = DateTime.Now;
			if (casTyp.Equals("date")) //vyhodnoceni dotazu na datum
			{
				expiraceVypoctena = expiraceVypoctena.AddDays(Posun);

				if (expiraceVypoctena < expiraceSarze) //materiál před expirací
				{
					odpoved = expiraceVypoctena.ToString("d.M.yyyy");
				}
				else if (DateTime.Now > expiraceSarze) //proexpirovany material
				{
					odpoved = "netisknout";
					UzivRozhrani.OznameniChyby(
						" Tisk štítků na EPL tiskárně",
						string.Format(" Tisk šablony {0} " + Environment.NewLine + " Materiál je proexpirovaný, expirace {1}," + Environment.NewLine + " štítek se nevytiskne!", Path.GetFileName(AdresaSouboru), expiraceSarze.ToString("d.M.yyyy")),
						" Pokračuj stisknutím libovolné klávesy...");
				}
				else //material ktery expiruje drive nez je trvanlivost po otevreni
				{
					odpoved = expiraceSarze.ToString("d.M.yyyy");
					UzivRozhrani.OznameniChyby(
						" Tisk štítků na EPL tiskárně",
						string.Format(" Tisk šablony {0}" + Environment.NewLine + " Materiál expiruje již za {1} dní ({2})!", Path.GetFileName(AdresaSouboru), (expiraceSarze - DateTime.Today).Days, expiraceSarze.ToString("d.M.yyyy")),
						" Pokračuj stisknutím libovolné klávesy...");
				}
			}
			else odpoved = expiraceVypoctena.AddMinutes(Posun).ToString("H:mm"); //dotaz na cas
			return odpoved;
		}
	}
}