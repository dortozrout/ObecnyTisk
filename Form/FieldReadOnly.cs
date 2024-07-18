using System;
using System.Linq;
using System.Collections.Generic;

namespace Form
{
    public class FieldReadOnly : FormItem
    {
        public ConsoleColor BgColor { get; protected set; }
        public ConsoleColor FgColor { get; protected set; }
        public FieldReadOnly(int leftPosition, int topPosition, string label, int length, ConsoleForm consoleForm) : base(leftPosition, topPosition, label, length, consoleForm)
        {
            LeftPosition = leftPosition;
            TopPosition = topPosition;
            Label = label;
            Length = length;
            SuperiorForm = consoleForm;
            BgColor = defaultBgColor;
            FgColor = defaultFgColor;
        }
        public FieldReadOnly(int leftPosition, int topPosition, string label, int length, ConsoleForm consoleForm, ConsoleColor fgColor, ConsoleColor bgColor) : base(leftPosition, topPosition, label, length, consoleForm)
        {
            LeftPosition = leftPosition;
            TopPosition = topPosition;
            Label = label;
            Length = length;
            SuperiorForm = consoleForm;
            BgColor = bgColor;
            FgColor = fgColor;
        }
        public FieldReadOnly(string id, int leftPosition, int topPosition, string label, int length, ConsoleForm consoleForm) : base(id, leftPosition, topPosition, label, length, consoleForm)
        {
            Id = id;
            LeftPosition = leftPosition;
            TopPosition = topPosition;
            Label = label;
            Length = length;
            SuperiorForm = consoleForm;
            BgColor = defaultBgColor;
            FgColor = defaultFgColor;
        }
        public FieldReadOnly(string id, int leftPosition, int topPosition, string label, int length, ConsoleForm consoleForm, ConsoleColor fgColor, ConsoleColor bgColor) : base(id, leftPosition, topPosition, label, length, consoleForm)
        {
            Id = id;
            LeftPosition = leftPosition;
            TopPosition = topPosition;
            Label = label;
            Length = length;
            SuperiorForm = consoleForm;
            BgColor = bgColor;
            FgColor = fgColor;
        }
        public override void Display()
        {
            Console.BackgroundColor = BgColor;
            Console.ForegroundColor = FgColor;
            Console.SetCursorPosition(LeftPosition, TopPosition);
            Console.Write(Label.PadRight(Length));
        }
        public virtual void Hide()
        {
            Console.BackgroundColor = defaultBgColor;
            Console.ForegroundColor = defaultFgColor;
            Console.SetCursorPosition(LeftPosition, TopPosition);
            Console.Write("".PadRight(Label.Length));
        }
        public virtual void ToggleVisible(object sender, MyEventArgs scanArgs)
        {
            if (scanArgs.OldText != scanArgs.NewText) Display();
            else Hide();
            //Console.ReadKey();
            //Console.SetCursorPosition(15, 2);
            //Console.WriteLine("event");
        }
        public override void SwitchTo(bool forward) { }

        public override void ToggleVisible(bool display)
        {
            if (display) Display();
            else Hide();
        }
    }

}