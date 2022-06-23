using System;
using System.Collections.Generic;
using System.IO;

namespace TiskStitku
{

	public class Dotazy
	{
		public List<Dotaz> GenerujListDotazu(string teloEPLprikazu)
		{
			List<Dotaz> listDotazu = new List<Dotaz>();
			int pocatek;
			int konec;
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
