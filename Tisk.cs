using Form;

namespace TiskStitku
{
    public class Tisk
    {
        public static void TiskniStitek(string telo)
        {
            if (Configuration.PrinterType == 0)
                TiskSdilenaTiskarna.Print(Configuration.PrinterAddress, telo);
            if (Configuration.PrinterType == 1)
                TiskLokalniTiskarna.SendStringToPrinter(Configuration.PrinterAddress, telo);
            if (Configuration.PrinterType == 2)
                TiskIPTiskarna.TiskniStitek(Configuration.PrinterAddress, telo);
            if (Configuration.PrinterType == 3)
            {
                NotificationForm notification = new NotificationForm("", telo);
                notification.Display();
                Console.ReadKey();
                // UzivRozhrani.Oznameni(" Tisk štítků na EPL tiskárně", telo, " Pokračuj stisnutím libovolné klávesy");
            }
        }
    }
}

