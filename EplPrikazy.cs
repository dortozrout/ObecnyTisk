using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace TiskStitku
{
    class EplPrikazy
    {
        private List<EplPrikaz> seznam = new List<EplPrikaz>();
        //konstruktor vytvori list epl prikazu ze souboru v danem umisteni
        //public EplPrikazy(string AdresaSlozky, string hledanyText)
        public EplPrikazy(string AdresaSlozky)
        {
            //var jmenaSouboru = Directory.EnumerateFiles(AdresaSlozky, hledanyText); //jmena souboru jsou vcetne cesty
            var jmenaSouboru = Directory.EnumerateFiles(AdresaSlozky, "*"); //jmena souboru jsou vcetne cesty
            foreach (string jmenoSouboru in jmenaSouboru)
            {
                string teloSouboru = File.ReadAllText(jmenoSouboru, Encoding.GetEncoding(Konfigurace.Kodovani));
                EplPrikaz prikaz = new EplPrikaz(jmenoSouboru, teloSouboru);
                seznam.Add(prikaz);
            }
        }
        //vraci cely seznam
        public List<EplPrikaz> VratSeznam()
        {
            return this.seznam;
        }
        //vraci seznam vyhledanych prikazu
        public List<EplPrikaz> VratSeznam(string hledanyText)
        {
            List<EplPrikaz> seznam = new List<EplPrikaz>();
            hledanyText = hledanyText.ToLower();
            foreach (EplPrikaz prikaz in this.seznam)
            {
				string nazevBezCesty = Path.GetFileName(prikaz.NazevSouboru);
				//if (prikaz.NazevSouboru.ToLower().Contains(hledanyText))
                if(nazevBezCesty.ToLower().Contains(hledanyText))
                {
                    seznam.Add(prikaz);
                }
            }
            return seznam;
        }
        //Vybere jeden prikaz ze seznamu vymezeneho hledanym textem
        public EplPrikaz Vyber(string hledanyText)
        {
            List<EplPrikaz> list;
			if (hledanyText == "*")
            {
                list = VratSeznam();
            }
            else
            {
                list = VratSeznam(hledanyText);
            }
            string telo = "";
            int cislo = 0;
            if (list.Count != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    telo += string.Format(("{0}.").PadLeft(5) + "\t{1}" + Environment.NewLine, i + 1, Path.GetFileName(list[i].NazevSouboru));
                }
                telo = (telo).TrimEnd('\n');
                cislo = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", telo, " Vyber soubor zadáním čísla (1 - " + list.Count + "): ", 1, list.Count, 0);
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
            cislo--;
            if (cislo != -1)
            {
                EplPrikaz eplPrikaz = list[cislo];
                return eplPrikaz;
            }
            else
                return null;
        }
        public EplPrikaz Vyber()
        {
            List<EplPrikaz> list= VratSeznam();
            string telo = "";
            int cislo = 0;
            if (list.Count != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    telo += string.Format(("{0}.").PadLeft(5) + "\t{1}" + Environment.NewLine, i + 1, Path.GetFileName(list[i].NazevSouboru));
                }
                telo = (telo).TrimEnd('\n');
                cislo = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", telo, " Vyber soubor zadáním čísla (1 - " + list.Count + "): ", 1, list.Count, 0);
            }
            cislo--;
            if (cislo != -1)
            {
                EplPrikaz eplPrikaz = list[cislo];
                return eplPrikaz;
            }
            else
                return null;
        }
    }
}