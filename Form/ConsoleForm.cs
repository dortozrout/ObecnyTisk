using System;
using System.Collections.Generic;

namespace Form
{
    //Konzolovy formular pro snadnejsi prijem polozky
    //pro kazdou polozku se tvori novy
    public abstract class ConsoleForm
    {
        //promena predstavujici stisknuti ESC
        public bool Quit{get; set;}
        //public abstract Polozka Fill();
    }
}