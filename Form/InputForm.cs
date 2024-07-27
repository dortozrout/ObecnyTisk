using System;
using System.Collections.Generic;

namespace Form
{
    public class InputForm<T> : ConsoleForm
    {
        private Background Background { get; set; }
        private UInputField<T> InputField { get; set; }
        public InputForm()
        {
            Background = new Background();
        }
        private void Display()
        {
            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            Background.Display();
            InputField.Display();
        }
        public T Fill(string question, string defaultText)
        {
            AditionalParms ap = new AditionalParms()
            {
                switchToNext = new List<ConsoleKeyInfo>() { new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false) },
                end = true
            };
            InputField = new UInputField<T>("input_field", 3, 21, question, 35, this, defaultText, null, ap);
            Display();
            InputField.SwitchTo(true);
            if (Quit)
            {
                Quit = false;
                return default(T);
            }
            return InputField.Value;
        }
    }
}