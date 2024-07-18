using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public static void OznameniChyby(string nadpis, string telo, string vyzva)
		{
			Console.Clear();
			//vypsani loga
			VypisLogo();
			Console.Write(nadpis + Environment.NewLine + Linka);
			//ulozeni puvodnich barev konzole do promenych
			ConsoleColor puvodniPozadi = Console.BackgroundColor;
			ConsoleColor puvodniPopredi = Console.ForegroundColor;
			//nastaveni zvyraznenych barev bila/cervena
			Console.BackgroundColor = ConsoleColor.DarkRed;
			Console.ForegroundColor = ConsoleColor.White;
			//vypsani zvyrazneneho tela zpravy
			Console.Write(telo);
			//nastaveni puvodnich barev
			Console.BackgroundColor = puvodniPozadi;
			Console.ForegroundColor = puvodniPopredi;
			Console.Write(Environment.NewLine + Linka + vyzva);
			//cekani na stisknuti libovolne klavesy
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

		public static List<int> VratCisla(string nadpis, string telo, string vyzva, int min, int max, string vychozi)
		{
			List<int> vystup = new List<int>();
			zprava = nadpis
				+ Environment.NewLine
				+ Linka
				+ telo
				+ Environment.NewLine
				+ Linka
				+ vyzva;
			bool opakovat;
			do
			{
				opakovat = false;
				Console.Clear();
				VypisLogo();
				Console.Write(zprava);
				string vstup = Console.ReadLine();
				if (vstup.ToLower() == Configuration.Editace)//vložení možnosti konfigurace z programu
				{
					Spravce spravce = new Spravce();
					spravce.Rozhrani();
				}
				else if (string.IsNullOrEmpty(vstup))//vychozi vstup pri prazdnem zadani
				{
					vstup = vychozi;
					var result = vstup.Split(',')
					.Select(x => x.Split('-'))
					.Select(p => new { First = int.Parse(p.First()), Last = int.Parse(p.Last()) })
					.SelectMany(x => Enumerable.Range(x.First, x.Last - x.First + 1))
					.OrderBy(z => z).Distinct();
					vystup = result.ToList();
				}
				else
				{
					try
					{
						var result = vstup.Split(',')
						.Select(x => x.Split('-'))
						.Select(p => new { First = int.Parse(p.First()), Last = int.Parse(p.Last()) })
						.SelectMany(x => Enumerable.Range(x.First, x.Last - x.First + 1))
						.OrderBy(z => z).Distinct();
						vystup = result.ToList();
						if (vystup.Max() > max || vystup.Min() < min) opakovat = true;//kontrola min-max
					}
					catch
					{
						opakovat = true;
					}
				}
			} while (opakovat);
			return vystup;
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
