using System;

namespace TiskStitku
{
	static class UzivRozhrani
	{
		private static string zprava = "";
		private static readonly string Linka = "".PadLeft(92, '\u2500') + Environment.NewLine;
		public static string VratText(string nadpis, string telo, string vyzva, string vychozi)
		{
			zprava = nadpis + Environment.NewLine + Linka;
			zprava += telo + Environment.NewLine + Linka;
			zprava += vyzva;
			Console.Clear();
			VypisLogo();
			Console.Write(zprava);
			string uzVstup = Console.ReadLine().Trim();
			if (string.IsNullOrEmpty(uzVstup))
				uzVstup = vychozi;
			return uzVstup;
		}
		public static string VratText(string nadpis, string hlavicka, string telo, string vyzva, string vychozi)
		{
			zprava = nadpis + Environment.NewLine + Linka;
			zprava += hlavicka + Environment.NewLine + Linka;
			zprava += telo + Environment.NewLine + Linka;
			zprava += vyzva;
			Console.Clear();
			VypisLogo();
			Console.Write(zprava);
			string uzVstup = Console.ReadLine().Trim();
			if (string.IsNullOrEmpty(uzVstup))
				uzVstup = vychozi;
			return uzVstup;
		}
		public static void Oznameni(string nadpis, string telo, string vyzva)
		{
			zprava = nadpis + Environment.NewLine + Linka;
			zprava += telo + Environment.NewLine + Linka;
			zprava += vyzva;
			Console.Clear();
			VypisLogo();
			Console.Write(zprava);
			Console.ReadKey();
		}
		public static bool VratAnoNe(string nadpis, string telo, string vyzva, bool vychoziRozhodnuti)
		{
			zprava = nadpis + Environment.NewLine + Linka
			+ telo + Environment.NewLine + Linka + vyzva;
			bool rozhodnuti;
			while (true)
			{
				Console.Clear();
				VypisLogo();
				Console.Write(zprava);
				string s = Console.ReadLine();
				s = s.Trim().ToLower();
				if (string.IsNullOrEmpty(s))
				{
					rozhodnuti = vychoziRozhodnuti;
					break;
				}
				else if (s.StartsWith("n"))
				{
					rozhodnuti = false;
					break;
				}
				else if (s.StartsWith("a"))
				{
					rozhodnuti = true;
					break;
				}
			}
			return rozhodnuti;
		}
		public static int VratCislo(string nadpis, string hlavicka, string telo, string vyzva, int min, int max, int vychozi)
		{
			zprava = nadpis
				+ Environment.NewLine
				+ Linka
				+ hlavicka
				+ Environment.NewLine
				+ Linka
				+ telo
				+ Environment.NewLine
				+ Linka
				+ vyzva;
			int cislo;
			while (true)
			{
				Console.Clear();
				VypisLogo();
				Console.Write(zprava);
				string s = Console.ReadLine();
				if (string.IsNullOrEmpty(s))
				{
					cislo = vychozi;
					break;
				}
				else if (s == "*")
				{
					cislo = -2;
					break;
				}
				else
				{
					bool jeCislo = int.TryParse(s, out cislo);
					if (jeCislo && cislo >= min && cislo <= max)
						break;
				}
			}
			return cislo;
		}
		public static int VratCislo(string nadpis, string telo, string vyzva, int min, int max, int vychozi)
		{
			zprava = nadpis
				+ Environment.NewLine
				+ Linka
				+ telo
				+ Environment.NewLine
				+ Linka
				+ vyzva;
			int cislo;
			while (true)
			{
				Console.Clear();
				VypisLogo();
				Console.Write(zprava);
				string s = Console.ReadLine();
				if (string.IsNullOrEmpty(s))
				{
					cislo = vychozi;
					break;
				}
				else
				{
					bool jeCislo = int.TryParse(s, out cislo);
					if (jeCislo && cislo >= min && cislo <= max)
						break;
				}
			}
			return cislo;
		}
		public static DateTime VratDatum(string nadpis, string telo, string vyzva, DateTime vychozi)
		{
			zprava = nadpis
				+ Environment.NewLine
				+ Linka
				+ telo
				+ Environment.NewLine
				+ Linka
				+ vyzva;
			DateTime datum;
			while (true)
			{
				Console.Clear();
				VypisLogo();
				Console.Write(zprava);

				string s = Console.ReadLine();
				if (string.IsNullOrEmpty(s))
				{
					datum = vychozi;
					break;
				}
				else
				{
					bool jeDatum = DateTime.TryParse(s, out datum);
					if (jeDatum) break;
				}
			}
			return datum;
		}
		public static void VypisLogo()
		{
			//Zjisteni barev console
			ConsoleColor puvodniPozadi = Console.BackgroundColor;
			ConsoleColor puvodniPismo = Console.ForegroundColor;
			//Nastaveni novych barev
			Console.BackgroundColor = ConsoleColor.Gray;
			Console.ForegroundColor = ConsoleColor.Black;
			//Vypsani barkodu
			string barcode = "   ▌ ▌█▌█▌▌▌█▌▌▌ █▌█▌▌▌ ▌█▌▌█▌▌ ▌█▌█▌▌█▌ ▌▌▌█▌▌▌ █▌▌█▌▌█▌ ▌▌ ▌█▌█▌▌   ";
			Console.WriteLine(barcode);
			//Vraceni puvodnich barev
			Console.BackgroundColor = puvodniPozadi;
			Console.ForegroundColor = puvodniPismo;
		}
	}
}
