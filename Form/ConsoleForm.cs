using System;
using System.Collections.Generic;
using TiskStitku;

namespace Form
{
    //Konzolovy formular 
    public abstract class ConsoleForm<T>
    {
        //promena predstavujici stisknuti ESC
        public bool Quit { get; set; }
        public abstract void Display();
        public abstract T Fill();
        public static void ResetColor()
        {
            Console.ForegroundColor = Configuration.defaultForegroundColor;
            Console.BackgroundColor = Configuration.defaultBackgroundColor;
        }
    }
}