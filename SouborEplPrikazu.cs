using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TiskStitku
{
	class SouborEplPrikazu
	{
		private List<EplPrikaz> seznam = new List<EplPrikaz>();
		//konstruktor vytvori list epl prikazu ze souboru v danem umisteni
		public SouborEplPrikazu(string AdresaSlozky)
		{
			var adresySouboru = Directory.EnumerateFiles(AdresaSlozky, "*").OrderBy(filename => filename); //jmena souboru jsou vcetne cesty
			foreach (string adresaSouboru in adresySouboru)
			{
				string teloSouboru = File.ReadAllText(adresaSouboru, Encoding.GetEncoding(Konfigurace.Kodovani));
				seznam.Add(new EplPrikaz(adresaSouboru, teloSouboru));
			}
		}
		//vraci cely seznam
		public List<EplPrikaz> VratSeznam()
		{
			return seznam;
		}
		//vraci seznam vyhledanych prikazu
		public List<EplPrikaz> VratSeznam(string hledanyText)
		{
			List<EplPrikaz> seznamZuzeny = seznam.FindAll(e => e.JmenoSouboru.ToLower().Contains(hledanyText.ToLower()));
			return seznamZuzeny;
		}
		//uzivatel vybere jeden nebo vice eplPrikazu ze seznamu vymezeneho hledanym textem
		public List<EplPrikaz> UzivVyber(string hledanyText)
		{
			//nejprve se vytvori vstupni seznam vsechny soubory|vyber na zaklade textu
			List<EplPrikaz> vstupniSeznam;
			if (hledanyText == "*") vstupniSeznam = VratSeznam();
			else vstupniSeznam = VratSeznam(hledanyText);
			//deklarace promennych
			string telo = "";
			List<int> vybranaCisla = new List<int>();
			//vlastní uživatelský výběr
			if (vstupniSeznam.Count != 0) //osetreni prazdneho seznamu
			{
				for (int i = 0; i < vstupniSeznam.Count; i++)
				{
					telo += string.Format(("{0}.").PadLeft(5) + "\t{1}" + Environment.NewLine, i + 1, Path.GetFileName(vstupniSeznam[i].AdresaSouboru));
				}
				telo = telo.TrimEnd('\n');
				vybranaCisla = UzivRozhrani.VratCisla(" Tisk štítků na EPL tiskárně", telo, " Vyber soubor zadáním čísla (1 - " + vstupniSeznam.Count + "): ", 1, vstupniSeznam.Count, "0");
			}
			/*//pokud list prikazu obsahuje prave jeden preskoci se vyber
            else if (list.Count == 1)
            {
                cislo = 1;
            }*/
			else
			{
				telo = " Nenalezen žádný výskyt \"" + hledanyText + "\"";
				UzivRozhrani.Oznameni(" Tisk štítků na EPL tiskárně", telo, " Pokračuj stisknutím libovolné klávesy.");
			}
			//vytvoreni vystupniho seznamu podle uzivatelem zadanych cisel
			if (vybranaCisla.Count != 0 && vybranaCisla[0] != 0) //osetreni prazdneho vstupu
			{
				List<EplPrikaz> vystupniSeznam = new List<EplPrikaz>();
				foreach (int i in vybranaCisla)
				{
					vystupniSeznam.Add(vstupniSeznam[i - 1]);
				}
				return vystupniSeznam;
			}
			else
				return null;
		}
	}
}