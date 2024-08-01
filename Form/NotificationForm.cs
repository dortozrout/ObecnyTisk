using System;
using TiskStitku;

namespace Form
{
    public class NotificationForm : ConsoleForm
    {
        private FieldReadOnly Label { get; set; }
        private FieldReadOnly Message { get; set; }
        private FieldReadOnly Notification { get; set; }
        public NotificationForm(string label, string message)
        {
            Label = new FieldReadOnly(3, 4, label, 25, this);
            string messageFormated = message.Replace(Environment.NewLine, Environment.NewLine + "   ");
            Message = new FieldReadOnly(3, 7, messageFormated, 25, this);
            Notification = new FieldReadOnly(3, Configuration.MaxLines + 7, "Pokračuj stisknutím libovolné klávesy...", 1, this);
        }
        public void Display()
        {
            Console.ResetColor();
            Console.Clear();
            Background bg = new Background();
            bg.Display();
            Label.Display();
            Message.Display();
            Notification.Display();
        }
    }
}