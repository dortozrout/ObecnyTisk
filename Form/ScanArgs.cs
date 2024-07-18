using System;

namespace Form
{
    public class MyEventArgs : EventArgs
    {
        public string Code { get; set; }
        public string OldText { get; set; }
        public string NewText { get; set; }
        public string Label { get; set; }
        public string Id { get; set; }
        public MyEventArgs()
        {
            Code = "";
            OldText = "";
            NewText = "";
            Label = "";
            Id = "";
        }
    }

}