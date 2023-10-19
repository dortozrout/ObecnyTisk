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
			if (!string.IsNullOrEmpty(Konfigurace.AdresaDat) && (Data == null))
			{
				List<Uloha> data = new List<Uloha>();
				try
				{
					string[] dataArray = File.ReadAllLines(Path.GetFullPath(Konfigurace.AdresaDat));
					foreach (string s in dataArray)
					{
						Uloha datapar;
						string[] radekDat;
						if (!s.StartsWith("#")) //ignoruje radky zakomentovane pomoci #
						{
							if (s.ToLower().Contains(":"))
							{
								radekDat = s.Split(':');
								datapar = new Uloha(radekDat[0].ToLower().Trim(), radekDat[1].Trim());
								data.Add(datapar);
							}
						}
					}
					Data = data;
				}
				catch (Exception ex)
				{
					UzivRozhrani.OznameniChyby("  Tisk štítků na EPL tiskárně",
										" Při zpracování souboru: " + Environment.NewLine +
										 Path.GetFullPath(Konfigurace.AdresaDat) + Environment.NewLine +
										" Došlo k chybě: " + Environment.NewLine + ex.Message,
										" Pokračuj stisknutím libovolné klávesy.");
				}
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
			while (teloEPLprikazu.Contains("<"))
			{
				pocatek = teloEPLprikazu.IndexOf('<');
				konec = teloEPLprikazu.IndexOf('>', pocatek);
				uloha = new Uloha(teloEPLprikazu.Substring(pocatek + 1, konec - pocatek - 1));
				teloEPLprikazu = teloEPLprikazu.Substring(konec);
				listUloh.Add(uloha);
			}
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
