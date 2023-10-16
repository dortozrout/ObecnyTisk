using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace TiskStitku
{

	class Program
	{
		static void Main(string[] args)
		{
			//Je treba pro .NET CORE
			//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Console.OutputEncoding = Encoding.UTF8;
			if (args.Length == 0) //pokud je program spusten bez parametru
			{
				Konfigurace.Nacti("conf.txt"); //pouzije se vychozi konfigurak
			}
			else //jinak se prvni parametr bere jako jmeno konfiguraku v %appdata%\TiskStitku
			{
				Konfigurace.Nacti(args[0]);
			}
			//deklarace promennych
			List<EplPrikaz> vybraneEplPrikazy;
			EplPrikaz eplPrikaz;
			//Konstruktor - nacte soubory z Adresare do privatniho seznamu (eplPrikazy.seznam)
			EplPrikazy eplPrikazy = new EplPrikazy(Konfigurace.Adresar);
			//Program muze bezet ve 3 modech - tisk pouze jednoho souboru podle konfiguraku
			//                               - vyber ze souboru vyfiltrovanych jiz v konfiguraku
			//                               - vyber ze souboru vyfiltrovanych zadanim filtru
			//Tisk pouze jednoho souboru
			if (Konfigurace.JedenSoubor)
			{
				if (eplPrikazy.VratSeznam(Konfigurace.HledanyText).Count == 0)
				{
					UzivRozhrani.Oznameni(" Tisk štítků na EPL tiskárně",
						" Program je nakonfigurován na tisk jednoho souboru"
						+ Environment.NewLine
						+ " ale soubor "
						+ Konfigurace.HledanyText
						+ " se v adresari "
						+ Konfigurace.Adresar
						+ " nenašel."
						+ Environment.NewLine
						+ " Zkontroluj nastavení v konfiguračním souboru: "
						+ Path.GetFullPath(Konfigurace.KonfiguracniSoubor),
						" Pokračuj stisknutím libovolné klávesy...");
				}
				else
				{
					int i;
					int j = 0;
					do
					{
						vybraneEplPrikazy = eplPrikazy.VratSeznam(Konfigurace.HledanyText);
						eplPrikaz = vybraneEplPrikazy[0];
						i = eplPrikaz.VyplnSablonu();
						if (i == 0)
						{
							Tisk.TiskniStitek(eplPrikaz.Telo);
						}
						j++;
						if (j == 10) break;//pojistka aby se netisklo donekonecna
					} while (i == 0 && Konfigurace.OpakovanyTisk);
				}
			}
			//Tisk vice souboru vymezenych hledanim
			else if (!Konfigurace.JedenSoubor && !string.IsNullOrEmpty(Konfigurace.HledanyText))
			{
				do
				{
					vybraneEplPrikazy = eplPrikazy.Vyber(Konfigurace.HledanyText);
					//eplPrikaz = eplPrikazy.Vyber(Konfigurace.HledanyText);
					if (vybraneEplPrikazy != null)
					{
						foreach (EplPrikaz epl in vybraneEplPrikazy)
						{
							int i = epl.VyplnSablonu();
							if (i == 0)
							{
								Tisk.TiskniStitek(epl.Telo);
							}
						}
					}
				} while (vybraneEplPrikazy != null);
			}
			else //Zobrazi nebo prohleda cely adresar
			{
				string hledanyText = "";
				do
				{
					string telo = " Konfigurační soubor: " + Konfigurace.KonfiguracniSoubor + Environment.NewLine
						+ " Adresa tiskárny: " + Konfigurace.AdresaTiskarny + Environment.NewLine
						+ " Typ tiskárny: " + Konfigurace.TypTiskarnySlovy + Environment.NewLine
						+ " Adresář se soubory: " + Path.GetFullPath(Konfigurace.Adresar) + Environment.NewLine
						+ " Kódování souborů: " + Konfigurace.Kodovani + Environment.NewLine
						+ " " + RuntimeInformation.FrameworkDescription;
					hledanyText = UzivRozhrani.VratText(" Tisk štítků na EPL tiskárně", telo, " Zadej část názvu hledaného souboru" + Environment.NewLine + " nebo * pro zobrazení všech souborů " + Environment.NewLine + " (prázdný vstup ukončí program): ", "");
					if (!string.IsNullOrEmpty(hledanyText))
					{
						vybraneEplPrikazy = eplPrikazy.Vyber(hledanyText);
						//eplPrikaz = eplPrikazy.Vyber(Konfigurace.HledanyText);
						if (vybraneEplPrikazy != null)
						{
							foreach (EplPrikaz epl in vybraneEplPrikazy)
							{
								int i = epl.VyplnSablonu();
								if (i == 0)
								{
									Tisk.TiskniStitek(epl.Telo);
								}
							}
						}
					}
				} while (!string.IsNullOrEmpty(hledanyText));
			}
		}
	}
}

