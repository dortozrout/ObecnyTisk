using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Form;

namespace TiskStitku
{

	class Program
	{
		static void Main(string[] args)
		{
			Run(args);
		}
		//rozdeleno kvuli "restartovani" programu (objekt spravce)
		public static void Run(string[] args)
		{
#if NETCOREAPP
			//Je treba pro .NET CORE
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
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
			SouborEplPrikazu eplPrikazy = new SouborEplPrikazu(Konfigurace.Adresar);
			//Program muze bezet ve 3 modech - tisk pouze jednoho souboru podle konfiguraku
			//                               - vyber ze souboru vyfiltrovanych jiz v konfiguraku
			//                               - vyber ze souboru vyfiltrovanych zadanim filtru
			//Tisk pouze jednoho souboru
			if (Konfigurace.JedenSoubor)
			{
				//osetreni pripadu kdy se nenajde soubor definovaný v konfiguráku
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
				//pokud se soubor najde vytiskne se
				else
				{
					//deklarace promennych
					int nhVyplnSablonu;
					int zarazka = 0;
					do
					{
						//vyberou se sablony odpovidajici hledanemu textu
						vybraneEplPrikazy = eplPrikazy.VratSeznam(Konfigurace.HledanyText);
						//z nich se vybere 1. nalezena
						eplPrikaz = vybraneEplPrikazy[0];
						//sablona se vyplni
						nhVyplnSablonu = eplPrikaz.VyplnSablonu();
						//pokud je vse ok vytiskne se
						if (nhVyplnSablonu == 0)
						{
							Tisk.TiskniStitek(eplPrikaz.Telo);
						}
						zarazka++;
						if (zarazka == 10) break;//pojistka aby se netisklo donekonecna
					} while (nhVyplnSablonu == 0 && Konfigurace.OpakovanyTisk);
				}
			}
			else
			{
				SelectFromList<EplPrikaz> selectList = new SelectFromList<EplPrikaz>();
				eplPrikaz = selectList.Select(eplPrikazy.VratSeznam());
				while (eplPrikaz != null)
				{
					if (eplPrikaz.VyplnSablonu() == 0)
					{
						Tisk.TiskniStitek(eplPrikaz.Telo);
					}
					eplPrikaz = selectList.Select(eplPrikazy.VratSeznam());
				}
			}
			// //Tisk vice souboru vymezenych hledanim
			// else if (!Konfigurace.JedenSoubor && !string.IsNullOrEmpty(Konfigurace.HledanyText))
			// {
			// 	do
			// 	{
			// 		// SelectFromList<string> selectList = new SelectFromList<string>();
			// 		// vybraneEplPrikazy = selectList.Select(eplPrikazy);
			// 		vybraneEplPrikazy = eplPrikazy.UzivVyber(Konfigurace.HledanyText);
			// 		if (vybraneEplPrikazy != null)
			// 		{
			// 			foreach (EplPrikaz epl in vybraneEplPrikazy)
			// 			{
			// 				int nhVyplnSablonu = epl.VyplnSablonu();
			// 				if (nhVyplnSablonu == 0)
			// 				{
			// 					Tisk.TiskniStitek(epl.Telo);
			// 				}
			// 			}
			// 		}
			// 	} while (vybraneEplPrikazy != null);
			// }
			// else //Zobrazi nebo prohleda cely adresar
			// {
			// 	string hledanyText;
			// 	do
			// 	{
			// 		string telo = " Konfigurační soubor: " + Konfigurace.KonfiguracniSoubor + Environment.NewLine
			// 			+ " Adresa tiskárny: " + Konfigurace.AdresaTiskarny + Environment.NewLine
			// 			+ " Typ tiskárny: " + Konfigurace.TypTiskarnySlovy + Environment.NewLine
			// 			+ " Adresář se soubory: " + Path.GetFullPath(Konfigurace.Adresar) + Environment.NewLine
			// 			+ " Kódování souborů: " + Konfigurace.Kodovani + Environment.NewLine
			// 			+ " " + RuntimeInformation.FrameworkDescription;
			// 		hledanyText = UzivRozhrani.VratText(" Tisk štítků na EPL tiskárně", telo, " Zadej část názvu hledaného souboru nebo * pro zobrazení všech souborů " + Environment.NewLine + " (" + Konfigurace.Editace + " = konfigurace, prázdný vstup = konec): ", "");
			// 		if (hledanyText.ToLower() == Konfigurace.Editace) //vložení konfigurace po zadání "edit"
			// 		{
			// 			Spravce spravce = new Spravce();
			// 			spravce.Rozhrani();
			// 		}
			// 		else if (!string.IsNullOrEmpty(hledanyText))
			// 		{
			// 			vybraneEplPrikazy = eplPrikazy.UzivVyber(hledanyText);
			// 			//eplPrikaz = eplPrikazy.Vyber(Konfigurace.HledanyText);
			// 			if (vybraneEplPrikazy != null)
			// 			{
			// 				foreach (EplPrikaz epl in vybraneEplPrikazy)
			// 				{
			// 					int i = epl.VyplnSablonu();
			// 					if (i == 0)
			// 					{
			// 						Tisk.TiskniStitek(epl.Telo);
			// 					}
			// 				}
			// 			}
			// 		}
			// 	} while (!string.IsNullOrEmpty(hledanyText));
			// }
		}
	}
}

