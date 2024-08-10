using System;
using TiskStitku;

namespace Form
{
    public class Background<TForm> : FormItem<TForm>
    {

        private readonly FieldReadOnly<TForm> header;
        private readonly FieldReadOnly<TForm> barcode;
        public Background()
        {
            LeftPosition = 0;
            TopPosition = 5;
            string barcode39 = "   ▌ ▌█▌█▌▌▌█▌▌▌ █▌█▌▌▌ ▌█▌▌█▌▌ ▌█▌█▌▌█▌ ▌▌▌█▌▌▌ █▌▌█▌▌█▌ ▌▌ ▌█▌█▌▌   ";
            barcode = new FieldReadOnly<TForm>(0, 0, barcode39, barcode39.Length, null,ConsoleColor.Black,ConsoleColor.Gray);
            header = new FieldReadOnly<TForm>(0, 1, Configuration.Header, Console.WindowWidth, null, Configuration.ActiveBackgroundColor, Configuration.ActiveForegroundColor);
        }
        public override void Display()
        {
            barcode.Display();
            header.Display();
            ResetColor();
            Console.SetCursorPosition(LeftPosition, TopPosition);
            Console.Write(Configuration.line);
            Console.SetCursorPosition(LeftPosition, TopPosition + 1 + Configuration.MaxLines);
            Console.Write(Configuration.line);
        }

        public override void SwitchTo(bool forward)
        {
            throw new NotImplementedException();
        }

        public override void ToggleVisible(bool display)
        {
            throw new NotImplementedException();
        }
    }
}
