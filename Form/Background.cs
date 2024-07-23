using System;
using TiskStitku;
//using TiskStitku;

namespace Form
{
    public class Background : FormItem
    {

        private readonly FieldReadOnly header;
        public Background()
        {
            LeftPosition = 0;
            TopPosition = 5;
            header = new FieldReadOnly(0, 1, Configuration.Header, 60, null, Configuration.BackgroundColor, Configuration.ForegroundColor);
        }
        public override void Display()
        {
            Configuration.DisplayLogo();
            header.Display();
            //hlavicka.Display();
            Console.ResetColor();
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
