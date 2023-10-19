namespace TiskStitku
{
	public class Uloha //jedna Uloha je dvojice zadani-vysledek
	{
		public string Zadani { get; set; }
		public string Vysledek { get; set; }
		public Uloha(string zadani)
		{
			Zadani = zadani;
		}
		public Uloha(string zadani, string vysledek)
		{
			Zadani = zadani;
			Vysledek = vysledek;
		}
		public override string ToString()
		{
			string navratovaHodnota = "Otazka = " + Zadani + " Odpoved = " + Vysledek;
			return navratovaHodnota;
		}
	}
}
