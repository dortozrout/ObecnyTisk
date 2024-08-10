using System;
using TiskStitku;

namespace Form
{
    public class ErrorForm : ConsoleForm<string>
    {
        private FieldReadOnly<string> Label { get; set; }
        private FieldReadOnly<string> Source { get; set; }
        private FieldReadOnly<string> Message { get; set; }
        private FieldReadOnly<string> Notification { get; set; }
        public ErrorForm()
        {
            Label = new FieldReadOnly<string>(3, 4, "Chybička se vloudila!", 25, this, ConsoleColor.White, ConsoleColor.Red);
            Source = new FieldReadOnly<string>(3, 7, "Zdroj: ", 25, this);
            Message = new FieldReadOnly<string>(3, 10, "Popis: ", 25, this);
            Notification = new FieldReadOnly<string>(3, Configuration.MaxLines + 7, "Stisknutím libovolné klávesy program ukončíš...", 25, this);
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
            ResetColor();
            Console.Clear();
            Background<string> bg = new Background<string>();
            bg.Display();
            Label.Display();
            Source.Display();
            Message.Display();
            Notification.Display();
            Console.ReadKey();
        }

        public override void Display()
        {
            throw new NotImplementedException();
        }

        public override string Fill()
        {
            throw new NotImplementedException();
        }
    }
}
