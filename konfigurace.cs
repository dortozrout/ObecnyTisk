using System;
using System.IO;
using System.Diagnostics;

namespace TiskStitku
{
    class Konfigurace
    {
        //Adresa konfig souboru
        public static string KonfiguracniSoubor { get; private set; }

        //Adresář se soubory obsahujícími EPL příkazy
        public static string Adresar { get; private set; }
        //Vyraz pro vyhledani jednoho nebo nekolika epl prikazu v adresari
        public static string HledanyText { get; private set; }
        //Tisk pouze jednoho souboru?
        public static bool JedenSoubor { get; private set; }
        //Kodovani souborů
        public static string Kodovani { get; private set; }

        //ip adresa nebo jméno tiskárny
        public static string AdresaTiskarny { get; private set; }
        //typ tiskarny - lokalni, sdilena nebo sitova
        public static int TypTiskarny { get; private set; }
        public static string TypTiskarnySlovy { get; private set; }
        public static int Nacti(string konfiguracniSoubor) //jmeno konfig souboru v adresari %appdata%/TiskStitku
        {
            int navratovaHodnota = 0;
            string cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TiskStitku");
            if (!Directory.Exists(cesta))
                Directory.CreateDirectory(cesta);
            if (File.Exists(Path.Combine(cesta, konfiguracniSoubor))) //pokud konfig soubor existuje
            {
                string[] konfArray = File.ReadAllLines(Path.Combine(cesta, konfiguracniSoubor));
                bool jedenSoubor = false;
                foreach (string s in konfArray)
                {
                    if (!s.StartsWith("#")) //ignoruje radky zakomentovane pomoci #
                    {
                        if (s.ToLower().Contains("iptiskarny:")) //tiskarna
                            AdresaTiskarny = s.Substring(s.IndexOf(':') + 1).Trim();
                        if (s.ToLower().Contains("typtiskarny:")) //tiskarna
                            TypTiskarny = int.Parse(s.Substring(s.IndexOf(':') + 1).Trim());
                        if (s.ToLower().Contains("adresar:")) //adresar se soubory epl prikazu
                            Adresar = s.Substring(s.IndexOf(':') + 1).Trim();
                        if (s.ToLower().Contains("hledanytext:")) //hledany text v nazvu souboru
                            HledanyText = s.Substring(s.IndexOf(':') + 1).Trim();
                        if (s.ToLower().Contains("jedensoubor:")) //true pokud se ma vytisknout pouze jeden soubor
                            bool.TryParse(s.Substring(s.IndexOf(':') + 1).Trim(), out jedenSoubor);
                        if (s.ToLower().Contains("kodovani:")) //adresar se soubory epl prikazu
                            Kodovani = s.Substring(s.IndexOf(':') + 1).Trim();
                    }
                }
                //if(jedenSoubor) JedenSoubor=true;
                JedenSoubor = jedenSoubor;
                if (TypTiskarny == 0)
                    TypTiskarnySlovy = "sdílená tiskárna";
                if (TypTiskarny == 1)
                    TypTiskarnySlovy = "lokální tiskárna";
                if (TypTiskarny == 2)
                    TypTiskarnySlovy = "síťová tiskárna";
                if (TypTiskarny == 3)
                    TypTiskarnySlovy = "výstup na obrazovku";
            }
            else //pokud konfig soubor neexistuje, vytvori novy
            {
                File.WriteAllText(Path.Combine(cesta, konfiguracniSoubor),
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
                    "# kodovani ulozenych souboru (UTF-8 nebo windows-1250)" + Environment.NewLine +
                    "kodovani:UTF-8" + Environment.NewLine);
                UzivRozhrani.Oznameni("  Tisk štítků na EPL tiskárně",
                    " První spuštění programu s konfiguračním souborem " + Environment.NewLine +
                    " " + Path.GetFullPath(Path.Combine(cesta, konfiguracniSoubor)) + Environment.NewLine +
                    " Konfigurační soubor bude otevřen v notepadu, uprav ho podle svých potřeb.",
                    " Pokračuj stisknutím libovolné klávesy.");
                Process externiProces = new Process();
                //externiProces.StartInfo.FileName = "Notepad.exe";
                externiProces.StartInfo.FileName = "mousepad";
                externiProces.StartInfo.Arguments = Path.GetFullPath(Path.Combine(cesta, konfiguracniSoubor));
                externiProces.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                externiProces.Start();
                externiProces.WaitForExit();
                navratovaHodnota = 1;
            }
            KonfiguracniSoubor = Path.GetFullPath(Path.Combine(cesta, konfiguracniSoubor));
            return navratovaHodnota;
        }
    }
}