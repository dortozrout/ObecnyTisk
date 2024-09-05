using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using Labels;
using Form;

namespace Labels
{
	/// <summary>
	/// Static class that holds the configuration for the program.
	/// </summary>
	class Configuration
	{
		//Nazev aplikace ktery se zobrazi v hlavicce
		private const string AppName = "Tisk štítků na EPL tiskárně";

		public const ConsoleColor defaultForegroundColor = ConsoleColor.White;
		public const ConsoleColor defaultBackgroundColor = ConsoleColor.Black;

		//Adresa konfig souboru
		public static string ConfigFile { get; private set; }

		//Cesta ke konf. adresari
		public static readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TiskStitku");

		//Adresář se soubory obsahujícími EPL příkazy
		public static string TemplatesDirectory { get; private set; }
		//Vyraz pro vyhledani jednoho nebo nekolika epl prikazu v adresari
		public static string SearchedText { get; private set; }
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
		public static string PrinterTypeByWords { get; private set; }
		//jestli vyzadovat zadani login
		public static bool Login { get; private set; }
		//uzivatelske jmeno
		public static string User { get; set; }
		//adresa souboru s daty expirace, sarze atd.
		public static string PrimaryDataAdress { get; private set; }
		//adresa hlavni sablony (pro tisk v rezimu jedne sablony)
		public static string MasterTemplateAddress { get; private set; }
		//klicove slovo pro spusteni spravce konfigurace
		public const string Editace = "edit";
		//kodovani pouzivane v epl prikazech "I8,B"
		public const string EplEncoding = "windows-1250";

		//Maximalni pocet radku kolik se zobrazi nez se zacne vypis posouvat 
		public const int MaxLines = 20;

		//format data v aplikaci
		public const string DateFormat = "yyyy-MM-dd";

		public static string line = "".PadRight(Console.WindowWidth, '\u2500');

		//obarveni konzole
		public static ConsoleColor ActiveForegroundColor { get; set; }
		public static ConsoleColor ActiveBackgroundColor { get; set; }

		public static string Header { get; private set; }

		//maximalni pocet stitku ktery lze vytisknout najednou
		public const int maxQuantity = 50;

		static Configuration()
		{
			Header = string.Empty;
		}

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
								case "iptiskarny":
									PrinterAddress = value;
									break;
								case "typtiskarny":
									int typTiskarny = 3;
									int.TryParse(value, out typTiskarny);
									PrinterType = typTiskarny;
									break;
								case "adresar":
									TemplatesDirectory = Path.GetFullPath(value);
									//TemplatesDirectory = value==""?"":Path.GetFullPath(value);
									break;
								case "hledanytext":
									SearchedText = value;
									break;
								case "jedensoubor":
									PrintOneFile = bool.Parse(value);
									break;
								case "opakovanytisk":
									Repeate = bool.Parse(value);
									break;
								case "kodovani":
									Encoding = value;
									break;
								case "prihlasit":
									Login = bool.Parse(value);
									break;
								case "data":
									// PrimaryDataAdress = value;
									PrimaryDataAdress = value == "" ? "" : Path.GetFullPath(value);
									break;
								case "hlavnisablona":
									MasterTemplateAddress = value;
									break;
							}
						}
					}
				}
			}
			catch (ArgumentException)
			{
				ErrorHandler.HandleError("Configuration", new ArgumentException("Zkontroluj nastaveni adresare v konfig souboru!"));
				result = 1;
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
					+ "# adresa souboru s primarnimi daty daty{0}"
					+ "data: {0}"
					+ "# adresa sablony pro tisk v modu jedne sablony{0}"
					+ "hlavniSablona: {0}", Environment.NewLine);
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
			NotificationForm notification = new NotificationForm("NOVÝ KONFIGURÁK", string.Format("První spuštění programu s konfiguračním souborem:\n\n      {0}.\n\n   Konfigurační soubor bude otevřen v editoru, uprav ho podle svých potřeb.", Path.GetFullPath(configFilePath)));
			notification.Display();
			Console.ReadKey();
			Manager spravce = new Manager();
			spravce.EditConfigFile(true);
			spravce.Restart();
		}
		private static void Initialize()
		{
			switch (PrinterType)
			{
				case 0:
					PrinterTypeByWords = "sdílená tiskárna";
					break;
				case 1:
					PrinterTypeByWords = "lokální tiskárna";
					break;
				case 2:
					PrinterTypeByWords = "síťová tiskárna";
					break;
				case 3:
					PrinterTypeByWords = "výstup na obrazovku";
					break;
				default:
					PrinterType = 3;
					PrinterTypeByWords = "výstup na obrazovku";
					break;
			}
			int align = 72;
			Header = string.Format("{7}{0}Konfigurační soubor: {1}"
						+ "Adresa tiskárny: {2}{0}"
						+ "Adresář se soubory:  {4}"
						+ "Typ tiskárny:    {3}{0}"
						+ "Kódování souborů:    {5}"
						+ "{6}{0}",
						Environment.NewLine, Path.Combine(ConfigPath, ConfigFile).PadRight(align - 22), PrinterAddress, PrinterTypeByWords,
						Path.GetFullPath(TemplatesDirectory).PadRight(align - 22), Encoding.PadRight(align - 22), RuntimeInformation.FrameworkDescription, AppName);
			ActiveBackgroundColor = ConsoleColor.DarkGreen;
			ActiveForegroundColor = ConsoleColor.Black;
		}
	}
}