using System;
using System.Collections.Generic;
using TiskStitku;

namespace Form
{
    public class InputForm<T> : ConsoleForm<T>
    {
        private Background<T> Background { get; set; }
        private FieldReadOnly<T> EplFileName { get; set; }
        private FieldReadOnly<T> EplTemplate { get; set; }
        private UInputField<T,T> InputField { get; set; }
        public InputForm()
        {
            Background = new Background<T>();
        }
        public override  void Display()
        {
            ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            Background.Display();
            EplFileName.Display();
            EplTemplate.Display();
            InputField.Display();
        }
        public T Fill(EplFile eplFile, string question, string defaultText)
        {
            EplFileName = new FieldReadOnly<T>("eplfilespec", 3, 7, string.Format("Tisk souboru: {0}", eplFile.JmenoSouboru), 20, this);
            EplTemplate = new FieldReadOnly<T>("epltemplate", 3, 9, string.Format("Šablona:\n{0}", eplFile.Sablona), 20, this);
            AditionalParms ap = new AditionalParms()
            {
                switchToNext = new List<ConsoleKeyInfo>() { new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false) },
                end = true
            };
            InputField = new UInputField<T,T>("input_field", 3, 21, question, 35, this, defaultText, null, ap);
            Display();
            InputField.SwitchTo(true);
            if (Quit)
            {
                eplFile.print = false;
                return default(T);
            }
            return InputField.Value;
        }

        public override T Fill()
        {
            throw new NotImplementedException();
        }
    }
}