using System;
using Labels;

namespace Form
{
    public class NotificationForm : ConsoleForm<string>
    {
        private FieldReadOnly<string> Label { get; set; }
        private FieldReadOnly<string> Message { get; set; }
        private FieldReadOnly<string> Notification { get; set; }
        public NotificationForm(string label, string message)
        {
            Label = new FieldReadOnly<string>(3, 4, label, label.Length, this);
            //string messageFormated = message.Replace(Environment.NewLine, Environment.NewLine + "   ");
            Message = new FieldReadOnly<string>(3, 7, message, message.Length, this);
            string prompt = "Pokračuj stisknutím libovolné klávesy...";
            Notification = new FieldReadOnly<string>(3, Configuration.MaxLines + 7, prompt, prompt.Length, this);
        }
        public override void Display()
        {
            ResetColor();
            Console.Clear();
            Background<string> bg = new Background<string>();
            bg.Display();
            if (Label.Label != "")
                Label.Display();
            Message.Display();
            Notification.Display();
        }

        public override string Fill()
        {
            throw new NotImplementedException();
        }
    }
}