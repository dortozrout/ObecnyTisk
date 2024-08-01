using System;
using System.Collections.Generic;
using TiskStitku;

namespace Form
{
	//Formular pro prihlaseni do aplikace
	public class LoginForm : ConsoleForm
	{
		private UInputField<string> Login { get; set; }
		private FieldReadOnly Label { get; set; }
		private Background Background { get; set; }
		private FieldReadOnly help;
		public LoginForm()
		{
			Label = new FieldReadOnly("label", 3, 7, "Je vyžadována identifikace uživatele", 25, this);
			Login = new UInputField<string>("login", 3, 10, "Login: ", 20, this, string.Empty, null, null, new AditionalParms() { end = true, switchToNext = new List<ConsoleKeyInfo>() { new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false) } });
			help = new FieldReadOnly("help", 0, Configuration.MaxLines + 7, "".PadRight(Polozka.col1) + "Zadej uživatelské jméno, potvrdit \"Enter\", zrušit \"Esc\"", Console.WindowWidth, this);
			Background = new Background();
		}
		public void Display()
		{
			Console.ResetColor();
			Console.Clear();
			Console.CursorVisible = true;
			Background.Display();
			Label.Display();
			Login.Display();
			help.Display();
			Login.SwitchTo(true);
		}

		public string Fill()
		{
			Console.ResetColor();
			Console.Clear();
			Display();
			Console.CursorVisible = false;
			if (Quit) return string.Empty;
			return Login.Value.ToString();
		}
	}
}