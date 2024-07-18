using System;
using System.IO;
using Form;
using System.Collections.Generic;

namespace CCData
{
	//staticka trida s konfiguraci programu
	class Konfigurace
	{
		//Minimální délka skenovaného řetězce
		public const int MinDelka = 8;
		//Maximalni pocet radku kolik se zobrazi nez se zacne vypis posouvat 
		public const int MaxPocetRadku = 20;
		//heslo pro zasifrovani uzivatelskych dat
		public static string SifrovaciHeslo = "Krokus158";
		public static string line = "".PadRight(Console.WindowWidth, '\u2500');
		//Adresa konfig souboru
		public static string KonfiguracniSoubor { get; private set; }
		//Uzivatelske jmeno
		public static string UzivatelskeJmeno { get; set; }
		//Heslo
		public static string Heslo { get; private set; }
		//Heslo
		public static string LogCesta { get; private set; }
		//Dodavatele
		public static string Dodavatele { get; private set; }
		//ip adresa nebo jméno tiskárny
		public static string AdresaTiskarny { get; private set; }
		//Typ tiskarny tj. lokalni, sdilena, sitova 
		public static int TypTiskarny { get; private set; }
		//Sablona
		public static string Sablona { get; private set; }
		//Cesta ke konf. adresari
		public static readonly string cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CCData");
		//datum, ktere CCData nezobrazuje
		public static readonly DateTime VychoziDatum = new DateTime(2100, 01, 01);
		//format data v aplikaci
		public const string Format = "yyyy-MM-dd";
		//vychozi barvy konzole
		public const ConsoleColor defaultFgColor = ConsoleColor.Gray;
		public const ConsoleColor defaultBgColor = ConsoleColor.Black;
		//obarveni konzole
		public static ConsoleColor FgColor { get; private set; }
		public static ConsoleColor BgColor { get; private set; }
		//promena pro ulozeni uzemky
		public static string Uzemka { get; private set; }
		public static string SkladNazev { get; private set; }
		//promena pro zahlavi
		public static string Uvod { get; private set; }
		static Konfigurace()
		{
			FgColor = defaultBgColor;
			BgColor = defaultFgColor;
			Uvod = "┌".PadRight(38, '─') + "┐" + Environment.NewLine +
				   "│ Příjem reagencií do skladu CCData".PadRight(38) + "│" + Environment.NewLine +
				   "└".PadRight(38, '─') + "┘" + Environment.NewLine;
		}
		//nacteni konfig souboru
		public static int Nacti(string konfiguracniSoubor)
		{
			int nh = 0;
			KonfiguracniSoubor = konfiguracniSoubor;
			try //pokusi se vytvorit adresar CCData v %appdata%, pokud neexistuje
			{
				if (!Directory.Exists(cesta))
					Directory.CreateDirectory(cesta);
			}
			catch (Exception ex)
			{
				ErrorHandler errorHandler = new ErrorHandler();
				errorHandler.ZpracujChybu("Konfigurace", ex);
				nh = 1;
			}
			if (File.Exists(Path.Combine(cesta, konfiguracniSoubor))) //pokud konfig soubor existuje
			{
				nh = NactiConfigSoubor(konfiguracniSoubor); //nacte ho
			}
			else //pokud konfig soubor neexituje, vytvori novy
			{
				try
				{
					File.WriteAllText(Path.Combine(cesta, konfiguracniSoubor),
						//"# uzivatelske jmeno pro prihlaseni do skladu" + Environment.NewLine +
						//"username:" + Environment.NewLine +
						//"#heslo" + Environment.NewLine +
						//"password:" + Environment.NewLine +
						"#cesta k logovacimu souboru" + Environment.NewLine +
						"logfile:" + Path.Combine(cesta, "log.txt") + Environment.NewLine +
						"#seznam dodavatelu oddeleny carkou" + Environment.NewLine +
						"dodavatele: ABBOTT,Roche,BIO-RAD,Medista,Lékárna,Gali,Medesa,Biovendor,Lab Mark," +
									"Radiometer s.r.o.,Beckman Coulter,Binding Site,Siemens," +
									"DiaSorin,Tecom,Promedica" + Environment.NewLine +
									"#adresa tiskarny" + Environment.NewLine +
						@"tiskarna: \\172.16.54.43\TSC_TTP-225" + Environment.NewLine +
						"#typ tiskarny 0 - sdilena, 1 - mistni, 2 - sitova, 3 - vystup na obrazovku" + Environment.NewLine +
						"typ_tiskarny: 3" + Environment.NewLine +
						"#sablona tisku" + Environment.NewLine +
						"#sablona: A30,40,0,4,1,1,N,<nazev>" + Environment.NewLine +
						"#sablona: A30,80,0,3,1,1,N,<katCislo>" + Environment.NewLine +
						"#sablona: A30,110,0,3,1,1,N,<uzemka>" + Environment.NewLine +
						"#sablona: A30,140,0,3,1,1,N,<datum>" + Environment.NewLine +
						"#sablona: A30,170,0,3,1,1,N,<boxCislo>" + Environment.NewLine +
						"#sablona: B60,210,0,1,3,4,120,B,<barkod>" + Environment.NewLine +
						"#sablona: A370,80,1,5,1,1,N,<novyLot>" + Environment.NewLine);
					nh = NactiConfigSoubor(konfiguracniSoubor); //nacteni nove vytvoreneho souboru
				}
				catch (Exception ex)
				{
					ErrorHandler errorHandler = new ErrorHandler();
					errorHandler.ZpracujChybu("Konfigurace", ex);
					nh = 1;
				}
			}
			return nh;
		}
		//pomocna privatni metoda pro vlastni nacteni config souboru 
		private static int NactiConfigSoubor(string konfiguracniSoubor)
		{
			int nh = 0;
			try //pokusi se ho nacist
			{
				// string konf = File.ReadAllText(Path.Combine(cesta, konfiguracniSoubor));
				// string[] konfArray = konf.Split('\n');
				string[] konfArray = File.ReadAllLines(Path.Combine(cesta, konfiguracniSoubor));
				foreach (string s in konfArray)
				{
					if (!s.StartsWith("#"))
					{
						if (s.ToLower().Contains("username:"))
							UzivatelskeJmeno = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("password:"))
							Heslo = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("logfile:"))
							LogCesta = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("dodavatele:"))
							Dodavatele = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("tiskarna:"))
							AdresaTiskarny = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("typ_tiskarny:"))
						{
							int typTiskarny = 3;
							int.TryParse(s.Substring(s.IndexOf(':') + 1).Trim(), out typTiskarny);
							TypTiskarny = typTiskarny;
						}
						if (s.ToLower().Contains("sablona:"))
							Sablona += s.Substring(s.IndexOf(':') + 1).Trim() + Environment.NewLine;
					}
				}
			}
			catch (Exception ex) //osetreni chyby
			{
				ErrorHandler errorHandler = new ErrorHandler();
				errorHandler.ZpracujChybu("Konfigurace", ex);
				nh = 1;
			}
			return nh;
		}
		//uvodni nastaveni barvicek a jmena skladu v zahlavi podle uzemky
		public static void UvodniNastaveni(string uzemka)
		{
			Uzemka = uzemka;
			switch (uzemka)
			{
				case "A":
					SkladNazev = "SBL";
					BgColor = ConsoleColor.DarkYellow;
					FgColor = ConsoleColor.Black;
					break;
				case "B":
					SkladNazev = "MLVP";
					BgColor = ConsoleColor.DarkCyan;
					FgColor = ConsoleColor.Black;
					break;
				case "C":
					SkladNazev = "LID";
					FgColor = ConsoleColor.Black;
					BgColor = ConsoleColor.Gray;
					break;
				case "D":
					SkladNazev = "SLH";
					BgColor = ConsoleColor.DarkMagenta;
					FgColor = ConsoleColor.Black;
					break;
				case "E":
					SkladNazev = "OIG";
					FgColor = ConsoleColor.DarkGreen;
					BgColor = ConsoleColor.Black;
					break;
			}
			Uvod = ("┌".PadRight(32, '─') + "┐").PadLeft(22) + Environment.NewLine +
			   ("│" + ("Příjem reagencií do skladu " + SkladNazev).PadRight(31).PadLeft(20) + "│").PadLeft(22) + Environment.NewLine +
			   ("└".PadRight(32, '─') + "┘").PadLeft(22) + Environment.NewLine;
		}
		//funkce pro vymazani uziv jmena a hesla po pouziti
		//aby nezustavali v pameti
		public static void RemoveLoginInfo()
		{
			//UzivatelskeJmeno = null;
			Heslo = null;
		}
	}
}