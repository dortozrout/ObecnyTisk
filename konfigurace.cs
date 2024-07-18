using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using TiskStitku;

namespace TiskStitku
{
	/// <summary>
	/// Static class that holds the configuration for the program.
	/// </summary>
	class Configuration
	{
		//Nazev aplikace ktery se zobrazi v hlavicce
		private const string AppName = "Obecný tisk štítků";

		public const ConsoleColor defaultForegroundColor = ConsoleColor.White;
		public const ConsoleColor defaultBackgroundColor = ConsoleColor.Black;

		//Adresa konfig souboru
		public static string ConfigFile { get; private set; }

		//Cesta ke konf. adresari
		private static readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TiskStitku");

		//Adresář se soubory obsahujícími EPL příkazy
		public static string TemplatesDirectory { get; private set; }
		//Vyraz pro vyhledani jednoho nebo nekolika epl prikazu v adresari
		public static string HledanyText { get; private set; }
		//Tisk pouze jednoho souboru?
		public static bool PrintOneFile { get; private set; }
		//Opakovany tisk 1 souboru?
		public static bool Repeate { get; private set; }
		//Kodovani souborů
		public static string Encoding { get; private set; }
		//ip adresa nebo jméno tiskárny
		public static string PrinterAddress { get; private set; }
		//typ tiskarny - lokalni, sdilena nebo sitova
		public static int PrinterType { get; private set; }
		public static string TypTiskarnySlovy { get; private set; }
		//jestli vyzadovat zadani login
		public static bool Prihlasit { get; private set; }
		//uzivatelske jmeno
		public static string Uzivatel { get; private set; }
		//adresa souboru s daty expirace, sarze atd.
		public static string AdresaDat { get; private set; }
		//klicove slovo pro spusteni spravce konfigurace
		public const string Editace = "edit";

		//Maximalni pocet radku kolik se zobrazi nez se zacne vypis posouvat 
		public const int MaxLines = 20;

		//format data v aplikaci
		public const string DateFormat = "yyyy-MM-dd";

		public static string line = "".PadRight(Console.WindowWidth, '\u2500');

		//obarveni konzole
		public static ConsoleColor ForegroundColor { get; set; }
		public static ConsoleColor BackgroundColor { get; set; }

		public static string Header { get; private set; }

		//metoda slouzici pro nacteni konfigurace
		public static int Load(string configFile) //jmeno konfig souboru v adresari %appdata%/TiskStitku
		{
			ConfigFile = configFile;
			int result = 0;
			try //pokusi se vytvorit adresar CCData v %appdata%, pokud neexistuje
			{
				if (!Directory.Exists(ConfigPath))
					Directory.CreateDirectory(ConfigPath);
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleError("Konfigurace", ex);
				result = 1;
			}
			string fullPath = Path.Combine(ConfigPath, configFile);

			if (File.Exists(fullPath)) //pokud konfig soubor existuje
			{
				result = LoadConfigFile(fullPath); //nacte ho
			}
			else //pokud konfig soubor neexituje, vytvori novy
			{
				result = CreateDefaultConfigFile(fullPath);
				if (result == 0)
				{
					result = LoadConfigFile(fullPath);
				}
			}
			Initialize();
			return result;

			string cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TiskStitku");
			//jestlize neexistuje adresar TiskStitku v %appdata% vytvori se
			if (!Directory.Exists(cesta)) Directory.CreateDirectory(cesta);
			//jestlize existuje konfiguracni soubor nacte se
			if (File.Exists(Path.Combine(cesta, configFile)))
			{
				string[] konfArray = File.ReadAllLines(Path.Combine(cesta, configFile));
				bool jedenSoubor = false;
				bool opakovanyTisk = false;
				bool prihlasit = false;
				foreach (string s in konfArray)
				{
					if (!s.StartsWith("#")) //ignoruje radky zakomentovane pomoci #
					{
						if (s.ToLower().Contains("iptiskarny:")) //tiskarna
							PrinterAddress = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("typtiskarny:")) //tiskarna
							PrinterType = int.Parse(s.Substring(s.IndexOf(':') + 1).Trim());
						if (s.ToLower().Contains("adresar:")) //adresar se soubory epl prikazu
							TemplatesDirectory = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("hledanytext:")) //hledany text v nazvu souboru
							HledanyText = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("jedensoubor:")) //true pokud se ma vytisknout pouze jeden soubor
							bool.TryParse(s.Substring(s.IndexOf(':') + 1).Trim(), out jedenSoubor);
						if (s.ToLower().Contains("opakovanytisk:")) //true pkud se jeden soubor ma tisknout opakovane
							bool.TryParse(s.Substring(s.IndexOf(':') + 1).Trim(), out opakovanyTisk);
						if (s.ToLower().Contains("kodovani:")) //kodovani souboru sablon epl prikazu
							Encoding = s.Substring(s.IndexOf(':') + 1).Trim();
						if (s.ToLower().Contains("prihlasit:")) //kodovani souboru sablon epl prikazu
							bool.TryParse(s.Substring(s.IndexOf(':') + 1).Trim(), out prihlasit);
						if (s.ToLower().Contains("data:")) //dvojice globalnich dat otazka-odpoved
							AdresaDat = s.Substring(s.IndexOf(':') + 1).Trim();
					}
				}
				PrintOneFile = jedenSoubor;
				Repeate = opakovanyTisk;
				try
				{
					System.Text.Encoding.GetEncoding(Encoding);
				}
				catch (Exception ex)
				{
					UzivRozhrani.OznameniChyby("  Tisk štítků na EPL tiskárně",
					" Kódování \"" + Encoding + "\" uvedené v konfiguračním souboru není platné." + Environment.NewLine +
					" Popis chyby: " + Environment.NewLine +
					ex.Message + Environment.NewLine +
					" Program bude ukončen.",
					" Pokračuj stisknutím libovolné klávesy.");
					Environment.Exit(-1);
				}
				Prihlasit = prihlasit;
				if (prihlasit && Uzivatel == null)
				{
					do
					{
						Uzivatel = UzivRozhrani.VratText("  Tisk štítků na EPL tiskárně", " Je vyžadována identifikace uživatele.", " Zadej login: ", "");
					} while (string.IsNullOrWhiteSpace(Uzivatel));
				}
				if (!Directory.Exists(TemplatesDirectory))
				{
					UzivRozhrani.Oznameni("  Tisk štítků na EPL tiskárně",
					" Adresář se šablonami uvedený v konfiguračním souboru neexistuje " + Environment.NewLine +
					//" " + Path.GetFullPath(Path.Combine(cesta, konfiguracniSoubor)) + Environment.NewLine +
					" Konfigurační soubor bude otevřen v editoru, uprav ho podle svých potřeb.",
					" Pokračuj stisknutím libovolné klávesy.");
					Process externiProces = new Process();
					//externiProces.StartInfo.FileName = "Notepad.exe";
					externiProces.StartInfo.FileName = "mousepad";
					externiProces.StartInfo.Arguments = Path.GetFullPath(Path.Combine(cesta, configFile));
					externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
					externiProces.Start();
					externiProces.WaitForExit();
					result = 1;
				}
				if (PrinterType == 0)
					TypTiskarnySlovy = "sdílená tiskárna";
				if (PrinterType == 1)
					TypTiskarnySlovy = "lokální tiskárna";
				if (PrinterType == 2)
					TypTiskarnySlovy = "síťová tiskárna";
				if (PrinterType == 3)
					TypTiskarnySlovy = "výstup na obrazovku";
			}
			else //pokud konfig soubor neexistuje, vytvori se novy a otevre v editoru
			{
				File.WriteAllText(Path.Combine(cesta, configFile),
					"# IP adresa nebo jmeno tiskarny" + Environment.NewLine +
					"IPtiskarny:" + Environment.NewLine +
					"# typ tiskarny 0 - sdilena, 1 - mistni, 2 - sitova, 3 - výstup na obrazovku" + Environment.NewLine +
					"TypTiskarny:3" + Environment.NewLine +
					"# adresar souboru s epl prikazy" + Environment.NewLine +
					"adresar:." + Environment.NewLine +
					"# text ktery se hleda v nazvu souboru" + Environment.NewLine +
					"hledanyText:" + Environment.NewLine +
					"# jestli se ma tisknout jenom jeden soubor" + Environment.NewLine +
					"jedenSoubor:false" + Environment.NewLine +
					"# jeden soubor se tiskne opakovane" + Environment.NewLine +
					"opakovanytisk:false" + Environment.NewLine +
					"# kodovani ulozenych souboru (UTF-8 nebo windows-1250)" + Environment.NewLine +
					"kodovani:UTF-8" + Environment.NewLine +
					"# zda vyzadovat login" + Environment.NewLine +
					"prihlasit:false" + Environment.NewLine +
					"# adresa souboru s daty" + Environment.NewLine +
					"data:" + Environment.NewLine);
				UzivRozhrani.Oznameni("  Tisk štítků na EPL tiskárně",
					" První spuštění programu s konfiguračním souborem " + Environment.NewLine +
					" " + Path.GetFullPath(Path.Combine(cesta, configFile)) + Environment.NewLine +
					" Konfigurační soubor bude otevřen v editoru, uprav ho podle svých potřeb.",
					" Pokračuj stisknutím libovolné klávesy.");
				//otevreni konfig souboru v editoru pri prvnim spusteni
				Process externiProces = new Process();
				externiProces.StartInfo.FileName = "Notepad.exe";
				//externiProces.StartInfo.FileName = "mousepad";
				//externiProces.StartInfo.FileName = "leafpad";
				externiProces.StartInfo.Arguments = Path.GetFullPath(Path.Combine(cesta, configFile));
				externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
				externiProces.Start();
				externiProces.WaitForExit();
				result = 1;
			}
			ConfigFile = Path.GetFullPath(Path.Combine(cesta, configFile));
			//nacteni konfigurace pokud byl konfig soubor nove vytvoren 
			if (result == 1) Load(ConfigFile);
			return result;
		}

		/// <summary>
		/// Loads the configuration from the specified file.
		/// </summary>
		/// <param name="configFilePath">The full path to the configuration file.</param>
		/// <returns>Returns 0 if successful, 1 if an error occurred.</returns>
		private static int LoadConfigFile(string configFilePath)
		{
			int result = 0;

			try
			{
				string[] configLines = File.ReadAllLines(configFilePath);

				foreach (string line in configLines)
				{
					if (!line.StartsWith("#"))
					{
						int separatorIndex = line.IndexOf(':');
						if (separatorIndex > 0) // Ensure there's a colon and it's not at the start
						{
							string key = line.Substring(0, separatorIndex).Trim().ToLower();
							string value = line.Substring(separatorIndex + 1).Trim();

							switch (key)
							{
								case "IPtiskarny":
									PrinterAddress = value;
									break;
								case "TypTiskarny":
									int typTiskarny = 3;
									int.TryParse(value, out typTiskarny);
									PrinterType = typTiskarny;
									break;
								case "adresar":
									TemplatesDirectory = value;
									break;
								case "hledanyText":
									HledanyText = value;
									break;
								case "jedenSoubor":
									PrintOneFile = bool.Parse(value);
									break;
								case "opakovanytisk":
									Repeate = bool.Parse(value);
									break;
								case "kodovani":
									Encoding = value;
									break;
								case "prihlasit":
									Prihlasit = bool.Parse(value);
									break;
								case "data":
									AdresaDat = value;
									break;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleError("Configuration", ex);
				result = 1;
			}

			return result;
		}

		/// <summary>
		/// Creates a default configuration file.
		/// </summary>
		/// <param name="configFilePath">The full path to the configuration file.</param>
		/// <returns>Returns 0 if successful, 1 if an error occurred.</returns>
		private static int CreateDefaultConfigFile(string configFilePath)
		{
			int result = 0;
			try
			{
				string content = string.Format("# IP adresa nebo jmeno tiskarny{0}"
					+ "IPtiskarny: {0}"
					+ "# typ tiskarny 0 - sdilena, 1 - mistni, 2 - sitova, 3 - výstup na obrazovku{0}"
					+ "TypTiskarny: 3{0}"
					+ "# adresar souboru s epl prikazy{0}"
					+ "adresar: .{0}"
					+ "# text ktery se hleda v nazvu souboru{0}"
					+ "hledanyText: {0}"
					+ "# jestli se ma tisknout jenom jeden soubor{0}"
					+ "jedenSoubor: false{0}"
					+ "# jeden soubor se tiskne opakovane{0}"
					+ "opakovanytisk: false{0}"
					+ "# kodovani ulozenych souboru (UTF-8 nebo windows-1250){0}"
					+ "kodovani: UTF-8{0}"
					+ "# zda vyzadovat login{0}"
					+ "prihlasit: false{0}"
					+ "# adresa souboru s daty{0}"
					+ "data: {0}", Environment.NewLine);
				File.WriteAllText(configFilePath, content);
				UserEdit(configFilePath);
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleError("Configuration", ex);
				result = 1;
			}
			return result;
		}
		private static void UserEdit(string configFilePath)
		{
			UzivRozhrani.Oznameni("  Tisk štítků na EPL tiskárně",
					" První spuštění programu s konfiguračním souborem " + Environment.NewLine +
					" " + Path.GetFullPath(configFilePath) + Environment.NewLine +
					" Konfigurační soubor bude otevřen v editoru, uprav ho podle svých potřeb.",
					" Pokračuj stisknutím libovolné klávesy.");
			//otevreni konfig souboru v editoru pri prvnim spusteni
			Process externiProces = new Process();
			externiProces.StartInfo.FileName = "Notepad.exe";
			//externiProces.StartInfo.FileName = "mousepad";
			//externiProces.StartInfo.FileName = "leafpad";
			externiProces.StartInfo.Arguments = Path.GetFullPath(configFilePath);
			externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			externiProces.Start();
			externiProces.WaitForExit();
		}
		private static void Initialize()
		{
			switch (PrinterType)
			{
				case 0:
					TypTiskarnySlovy = "sdílená tiskárna";
					break;
				case 1:
					TypTiskarnySlovy = "lokální tiskárna";
					break;
				case 2:
					TypTiskarnySlovy = "síťová tiskárna";
					break;
				case 3:
					TypTiskarnySlovy = "výstup na obrazovku";
					break;
				default:
					PrinterType = 3;
					TypTiskarnySlovy = "výstup na obrazovku";
					break;
			}
			int align = 72;
			Header = string.Format(" {7}{0} Konfigurační soubor: {1}"
						+ "Adresa tiskárny: {2}{0}"
						+ " Adresář se soubory:  {4}"
						+ "Typ tiskárny:    {3}{0}"
						+ " Kódování souborů:    {5}"
						+ "{6}{0}",
						Environment.NewLine, Path.Combine(ConfigPath, ConfigFile).PadRight(align - 22), PrinterAddress, TypTiskarnySlovy,
						Path.GetFullPath(TemplatesDirectory).PadRight(align - 22), Encoding.PadRight(align - 22), RuntimeInformation.FrameworkDescription, AppName);
			BackgroundColor = ConsoleColor.DarkGreen;
			ForegroundColor = ConsoleColor.Black;
		}
		public static void DisplayLogo()
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