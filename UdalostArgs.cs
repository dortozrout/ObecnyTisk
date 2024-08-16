using System;

namespace Labels
{
    public class ErrorEventArgs : EventArgs
    {
        public string LogText { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }
}