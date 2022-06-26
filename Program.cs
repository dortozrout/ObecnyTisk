using System.Text;

namespace TiskStitku;

class Program
{
    static void Main(string[] args)
    {
        //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //Console.OutputEncoding = Encoding.Unicode;
        Console.OutputEncoding = Encoding.UTF8;
        int nh;//navratova hodnota 1 pokud konf soubor neexistuje vytvori se novy
        if (args.Length == 0)
        {
            nh = Konfigurace.Nacti("conf.txt");
        }
        else
        {
            nh = Konfigurace.Nacti(args[0]);
        }
        //nacteni konfigurace pokud byl konfig soubor nove vytvoren 
        if (nh == 1) Konfigurace.Nacti(Konfigurace.KonfiguracniSoubor);
        EplPrikaz eplPrikaz;
        EplPrikazy eplPrikazy = new EplPrikazy(Konfigurace.Adresar, Konfigurace.HledanyText);
        if (Konfigurace.JedenSoubor) //Tisk pouze jednoho souboru
        {
            if (eplPrikazy.VratSeznam().Count == 0)
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
                    " Pokračuj stisknutím libovolné klávesy.");
            }
            else
            {
                eplPrikaz = eplPrikazy.VratSeznam()[0];
                int i = eplPrikaz.VyplnSablonu();
                if (i == 0)
                {
                    Tisk.TiskniStitek(eplPrikaz.Telo);
                }
            }
        }
        //Tisk vice souboru vymezenych hledanim
        else if (!Konfigurace.JedenSoubor && !string.IsNullOrEmpty(Konfigurace.HledanyText))
        {
            do
            {
                eplPrikaz = eplPrikazy.Vyber();
                if (eplPrikaz != null)
                {
                    int i = eplPrikaz.VyplnSablonu();
                    if (i == 0)
                    {
                        Tisk.TiskniStitek(eplPrikaz.Telo);
                    }
                }
            } while (eplPrikaz != null);
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
                    + " Kódování souborů: " + Konfigurace.Kodovani;
                hledanyText = UzivRozhrani.VratText(" Tisk štítků na EPL tiskárně", telo, " Zadej část názvu hledaného souboru" + Environment.NewLine + " nebo * pro zobrazení všech souborů " + Environment.NewLine + " (prázdný vstup ukončí program): ", "");
                if (!string.IsNullOrEmpty(hledanyText))
                {
                    eplPrikaz = eplPrikazy.Vyber(hledanyText);
                    if (eplPrikaz != null)
                    {
                        int i = eplPrikaz.VyplnSablonu();
                        if (i == 0)
                        {
                            Tisk.TiskniStitek(eplPrikaz.Telo);
                        }
                    }
                }
            } while (!string.IsNullOrEmpty(hledanyText));
        }
    }
}

