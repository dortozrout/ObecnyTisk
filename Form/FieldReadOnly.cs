using System;
using System.Linq;
using System.Collections.Generic;
using TiskStitku;
using System.Linq.Expressions;

namespace Form
{
    public class FieldReadOnly<TForm> : FormItem<TForm>
    {
        public ConsoleColor BgColor { get; protected set; }
        public ConsoleColor FgColor { get; protected set; }
        public FieldReadOnly(int leftPosition, int topPosition, string label, int length, ConsoleForm<TForm> consoleForm, ConsoleColor fgColor = Configuration.defaultForegroundColor, ConsoleColor bgColor = Configuration.defaultBackgroundColor) : this("", leftPosition, topPosition, label, length, consoleForm, fgColor, bgColor) { }
        public FieldReadOnly(string id, int leftPosition, int topPosition, string label, int length, ConsoleForm<TForm> consoleForm, ConsoleColor fgColor = Configuration.defaultForegroundColor, ConsoleColor bgColor = Configuration.defaultBackgroundColor) : base(id, leftPosition, topPosition, label, length, consoleForm)
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
            DisplayText(Label);
        }
        public virtual void Hide()
        {
            ResetColor();
            Console.SetCursorPosition(LeftPosition, TopPosition);
            DisplayText(Label, true);
        }
        public virtual void ToggleVisible(object sender, MyEventArgs scanArgs)
        {
            if (scanArgs.OldText != scanArgs.NewText) Display();
            else Hide();
        }
        public override void SwitchTo(bool forward) { }

        public override void ToggleVisible(bool display)
        {
            if (display) Display();
            else Hide();
        }
        
        private void DisplayText(string text, bool erase = false)
        {
            var textArray = text.Split(Environment.NewLine);
            for (int i = 0; i < textArray.Length; i++)
            {
                textArray[i] = erase
                    ? new string(' ', Length)
                    : textArray[i].PadRight(Length).Substring(0, Length);
                Console.SetCursorPosition(LeftPosition, TopPosition + i);
                Console.Write(textArray[i]);
            }
        }
    }

}