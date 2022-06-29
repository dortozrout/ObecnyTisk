using System;
using System.Collections.Generic;

namespace TiskStitku
{

	public class Dotazy
	{
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
			return listDotazu;
		}
	}
}
