namespace TiskStitku
{
    public class Tisk
    {
        public static void TiskniStitek(string telo)
        {
            if (Konfigurace.TypTiskarny == 0)
                TiskSdilenaTiskarna.Print(Konfigurace.AdresaTiskarny, telo);
            if (Konfigurace.TypTiskarny == 1)
                TiskLokalniTiskarna.SendStringToPrinter(Konfigurace.AdresaTiskarny, telo);
            if (Konfigurace.TypTiskarny == 2)
                TiskIPTiskarna.TiskniStitek(Konfigurace.AdresaTiskarny, telo);
            if (Konfigurace.TypTiskarny == 3)
                UzivRozhrani.Oznameni(" Tisk štítků na EPL tiskárně", telo, " Pokračuj stisnutím libovolné klávesy");

        }
    }
}

