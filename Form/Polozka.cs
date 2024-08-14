using System;

namespace Labels
{

	public class Polozka : IComparable //trida pro ulozeni polozky
	{
		public const int col1 = 7;
		public const int col2 = 35;
		public const int col3 = 25;
		public const int col4 = 10;
		//public const int col5 =
		public string Kid { get; set; }
		public string Sid { get; set; }
		public string Oid { get; set; }
		public string Jmeno { get; set; }
		public string KatCislo { get; set; }
		public string Sarze { get; set; }
		public string Expirace { get; set; }
		public string Mnozstvi { get; set; }
		public string Umisteni { get; set; }
		public string Pribalak { get; set; }
		public bool NovyLot { get; set; }
		public bool ZmenaLotu { get; set; }
		public bool OznaceniNovyLot { get; set; }
		public bool ZmenaMnozstvi { get; set; }
		public bool ZmenaPribalaku { get; set; }
		public string CisloBoxu { get; set; }
		public Polozka()
		{

		}
		public Polozka(string kid, string jmeno, string katCislo, string sarze, string expirace, string mnozstvi, string umisteni)
		{
			Kid = kid;
			Jmeno = jmeno;
			KatCislo = katCislo;
			Sarze = sarze;
			Expirace = expirace;
			Mnozstvi = mnozstvi;
			Umisteni = umisteni;
		}
		public Polozka(string kid, string sid, string jmeno, string katCislo, string sarze, string expirace, string mnozstvi, string umisteni)
		{
			Kid = kid;
			Sid = sid;
			Jmeno = jmeno;
			KatCislo = katCislo;
			Sarze = sarze;
			Expirace = expirace;
			Mnozstvi = mnozstvi;
			Umisteni = umisteni;
		}
		public string ToString(string s)
		{
			return " Číslo krabice: "
				+ this.Kid
				+ Environment.NewLine
				+ " Číslo položky: "
				+ this.Sid
				+ Environment.NewLine
				+ " Katalogové číslo: "
				+ this.KatCislo
				+ Environment.NewLine
				+ " Název: "
				+ this.Jmeno
				+ Environment.NewLine
				+ " Množství: "
				+ this.Mnozstvi
				+ Environment.NewLine
				+ " Umístění: "
				+ this.Umisteni
				+ Environment.NewLine
				+ " Šarže: "
				+ this.Sarze
				+ Environment.NewLine
				+ " Expirace: "
				+ this.Expirace;
		}
		public override string ToString()
		{
			string name = Jmeno.Length > 30 ? Jmeno.Substring(0, 30) : Jmeno;
			return name.PadRight(col2) + KatCislo.PadRight(col3) + Mnozstvi.PadLeft(col4);
		}
		public int CompareTo(object obj)
		{
			Polozka jinaPolozka = (Polozka)obj;
			int nh = string.Compare(Jmeno, jinaPolozka.Jmeno);
			if (nh == 0)
				return string.Compare(KatCislo, jinaPolozka.KatCislo);
			return nh;
		}
	}
}