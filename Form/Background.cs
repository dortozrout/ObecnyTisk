using System;
using CCData;
//using TiskStitku;

namespace Form
{
    public class Background : FormItem
    {

        private readonly FieldReadOnly nazevSkladu;
        public Background()
        {
            LeftPosition = 0;
            TopPosition = 5;
            if (string.IsNullOrEmpty(Konfigurace.Uzemka)) nazevSkladu = new FieldReadOnly(0, 0, Konfigurace.Uvod, 60, null);
            else nazevSkladu = new FieldReadOnly(0, 0, Konfigurace.Uvod, 60, null, Konfigurace.FgColor, Konfigurace.BgColor);
        }
        public override void Display()
        {
            nazevSkladu.Display();
            //hlavicka.Display();
            Console.ResetColor();
            Console.SetCursorPosition(LeftPosition, TopPosition);
            Console.Write(Konfigurace.line);
            Console.SetCursorPosition(LeftPosition, TopPosition + 1 + Konfigurace.MaxPocetRadku);
            Console.Write(Konfigurace.line);
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
