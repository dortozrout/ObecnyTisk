using System;
using TiskStitku;

namespace Form
{
    public class ErrorForm : ConsoleForm
    {
        private FieldReadOnly Label { get; set; }
        private FieldReadOnly Source { get; set; }
        private FieldReadOnly Message { get; set; }
        private FieldReadOnly Notification { get; set; }
        public ErrorForm()
        {
            Label = new FieldReadOnly(3, 4, "Chybka se vloudila!", 25, this, ConsoleColor.White, ConsoleColor.Red);
            Source = new FieldReadOnly(3, 7, "Zdroj: ", 25, this);
            Message = new FieldReadOnly(3, 10, "Popis: ", 25, this);
            Notification = new FieldReadOnly(3, Configuration.MaxLines + 7, "Stisknutím libovolné klávesy program ukončíš...", 25, this);
        }
        public void Display(object sender, TiskStitku.ErrorEventArgs eventArgs)
        {
            string message = eventArgs.Message;
            string messageFormated = "\n";
            int step = Console.WindowWidth - 3;
            string segment;
            int indexOfSpace;
            int messageLength = message.Length;
            for (int i = 0; i < messageLength; i += step)
            {
                if (i + step < messageLength)
                {
                    indexOfSpace = message.LastIndexOf(' ', step);
                    //segment = message.Substring(0, step);
                    //indexOfSpace = segment.LastIndexOf(' ');
                    //segment = segment.Substring(0, indexOfSpace);
                    segment = message.Substring(0, indexOfSpace);
                    messageFormated += "\n".PadRight(4) + segment;
                    message = message.Substring(indexOfSpace + 1);
                }
                else messageFormated += "\n".PadRight(4) + message;
            }
            Source.Label += eventArgs.Source;
            Message.Label += messageFormated;
            Console.ResetColor();
            Console.Clear();
            Background bg = new Background();
            bg.Display();
            Label.Display();
            Source.Display();
            Message.Display();
            Notification.Display();
            Console.ReadKey();
        }
    }
}
