using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TiskStitku
{

	public class SouborUloh
	{
		//staticky list slouzi k ulozeni udaju ze souboru dat definovanem v konfiguraku
		public static List<Uloha> Data;
		public SouborUloh()
		{
			//pokud je v config souboru zadana adresa souboru s daty - otazka:odpoved
			//vyplni se pri prvnim vytvoreni instance statický list Dotazy.Data
			if (!string.IsNullOrEmpty(Configuration.AdresaDat) && (Data == null))
			{
				NactiData();
			}
		}
		public static void NactiData()
		{
			if (Data != null) Data.Clear();
			List<Uloha> data = new List<Uloha>();
			try
			{
				string[] obsahSouboruDat = File.ReadAllLines(Path.GetFullPath(Configuration.AdresaDat));
				foreach (string s in obsahSouboruDat)
				{
					Uloha uloha;
					string[] radekDat;
					if (!s.StartsWith("#")) //ignoruje radky zakomentovane pomoci #
					{
						if (s.ToLower().Contains(":"))
						{
							radekDat = s.Split(':');
							uloha = new Uloha(radekDat[0].ToLower().Trim(), radekDat[1].Trim());
							data.Add(uloha);
						}
					}
				}
				Data = data;
			}
			catch (Exception ex)
			{
				UzivRozhrani.OznameniChyby("  Tisk štítků na EPL tiskárně",
									" Při zpracování souboru: " + Environment.NewLine +
									 Path.GetFullPath(Configuration.AdresaDat) + Environment.NewLine +
									" Došlo k chybě: " + Environment.NewLine + ex.Message,
									" Pokračuj stisknutím libovolné klávesy.");
			}

		}
		// seznam uloh pro jeden soubor s epl prikazem
		// ktery je napsan jako sablona k doplneni
		// tj. obsahuje P na konci, nebo <vzor> 
		public List<Uloha> GenerujListUloh(string teloEPLprikazu)
		{
			List<Uloha> listUloh = new List<Uloha>();
			int pocatek; //pocatek vzoru
			int konec; //konec vzoru
			Uloha uloha;
			//parsovani sablony dokud obsahuje "<"
			while (teloEPLprikazu.Contains('<'))
			{
				//definice pocatku a konce ulohy - vzoru
				pocatek = teloEPLprikazu.IndexOf('<');
				konec = teloEPLprikazu.IndexOf('>', pocatek);
				//definice ulohy vymezene pocatkem a koncem
				uloha = new Uloha(teloEPLprikazu.Substring(pocatek + 1, konec - pocatek - 1));
				//odstrizeni zbytku eplPrikazu
				teloEPLprikazu = teloEPLprikazu.Substring(konec + 1);
				//pridani ulohy do listu
				listUloh.Add(uloha);
			}
			//osetreni pripadu, kdy sablona konci na "P"
			if (teloEPLprikazu.TrimEnd(new char[] { '\r', '\n' }).EndsWith("P"))
			{
				uloha = new Uloha("počet štítků");
				listUloh.Add(uloha);
			}
			//odstaneni duplicitnich dotazu
			listUloh = listUloh.GroupBy(p => p.Zadani).Select(g => g.First()).ToList();
			return listUloh;
		}
	}
}
