using System;

namespace CCData
{
    class UdalostArgs : EventArgs
    {
        public string Text { get; set; }
        public string VydejText { get; set; }
        public string LogText { get; set; }
        public string Nadpis { get; set; }
        public string Telo { get; set; }
        public string Vyzva { get; set; }
        //public KindfOfAdd ZpusobZapisu { get; set; }
    }
}