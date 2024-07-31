using System;
using System.Collections.Generic;
using TiskStitku;

namespace Form
{
    public class InputForm<T> : ConsoleForm
    {
        private Background Background { get; set; }
        private FieldReadOnly EplFileName { get; set; }
        private FieldReadOnly EplTemplate { get; set; }
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
            EplFileName.Display();
            EplTemplate.Display();
            InputField.Display();
        }
        public T Fill(EplFile eplFile, string question, string defaultText)
        {
            EplFileName = new FieldReadOnly("eplfilespec", 3, 7, string.Format("Tisk souboru: {0}", eplFile.JmenoSouboru), 20, this);
            EplTemplate = new FieldReadOnly("epltemplate", 3, 9, string.Format("Šablona:\n{0}", eplFile.Sablona), 20, this);
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
                eplFile.print = false;
                return default(T);
            }
            return InputField.Value;
        }
    }
}