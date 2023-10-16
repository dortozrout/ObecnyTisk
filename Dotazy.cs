using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TiskStitku
{

	public class Dotazy
	{
		public static List<Dotaz> Data;
		public Dotazy()
		{
			//pokud je v config souboru zadana adresa souboru s daty - otazka:odpoved
			//vyplni se pri prvnim vytvoreni instance statický list Dotazy.Data
			if (!string.IsNullOrEmpty(Konfigurace.AdresaDat) && (Data == null))
			{
				List<Dotaz> data = new List<Dotaz>();
				try
				{
					string[] dataArray = File.ReadAllLines(Path.GetFullPath(Konfigurace.AdresaDat));
					foreach (string s in dataArray)
					{
						Dotaz datapar;
						string[] radekDat;
						if (!s.StartsWith("#")) //ignoruje radky zakomentovane pomoci #
						{
							if (s.ToLower().Contains(":"))
							{
								radekDat = s.Split(':');
								datapar = new Dotaz(radekDat[0].ToLower().Trim(), radekDat[1].Trim());
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
		// seznam dotazu pro jeden soubor s epl prikazem
		// ktery je napsan jako sablona k doplneni
		// tj. obsahuje P na konci, nebo <vzor> 
		public List<Dotaz> GenerujListDotazu(string teloEPLprikazu)
		{
			List<Dotaz> listDotazu = new List<Dotaz>();
			int pocatek; //pocatek vzoru
			int konec; //konec vzoru
			Dotaz dotaz;
			while (teloEPLprikazu.Contains("<"))
			{
				pocatek = teloEPLprikazu.IndexOf('<');
				konec = teloEPLprikazu.IndexOf('>', pocatek);
				dotaz = new Dotaz(teloEPLprikazu.Substring(pocatek + 1, konec - pocatek - 1));
				teloEPLprikazu = teloEPLprikazu.Substring(konec);
				listDotazu.Add(dotaz);
			}
			if (teloEPLprikazu.TrimEnd(new char[] { '\r', '\n' }).EndsWith("P"))
			{
				dotaz = new Dotaz("počet štítků");
				listDotazu.Add(dotaz);
			}
			//odstaneni duplicitnich dotazu
			listDotazu = listDotazu.GroupBy(p => p.Otazka).Select(g => g.First()).ToList();
			return listDotazu;
		}
	}
}
