using System;

namespace TiskStitku
{
	public class Dotaz
	{
		public string Otazka { get; set; }
		public string Odpoved { get; set; }
		public Dotaz(string otazka)
		{
			Otazka = otazka;
		}
		public override string ToString()
		{
			string navratovaHodnota = "Otazka = " + Otazka + " Odpoved = " + Odpoved;
			return navratovaHodnota;
		}
	}
}
