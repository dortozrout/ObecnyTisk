using System;
using System.Collections.Generic;
using Labels;

namespace Form
{
	//Formular pro prihlaseni do aplikace
	public class LoginForm : ConsoleForm<string>
	{
		private UInputField<string,string> Login { get; set; }
		private FieldReadOnly<string> Label { get; set; }
		private Background<string> Background { get; set; }
		private FieldReadOnly<string> help;
		public LoginForm()
		{
			Label = new FieldReadOnly<string>("label", 3, 7, "Je vyžadována identifikace uživatele", 25, this);
			Login = new UInputField<string,string>("login", 3, 10, "Login: ", 20, this, string.Empty, null, null, new AditionalParms() { end = true, switchToNext = new List<ConsoleKeyInfo>() { new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false) } });
			help = new FieldReadOnly<string>("help", 0, Configuration.MaxLines + 7, "".PadRight(Polozka.col1) + "Zadej uživatelské jméno, potvrdit \"Enter\", zrušit \"Esc\"", Console.WindowWidth, this);
			Background = new Background<string>();
		}
		public override void Display()
		{
			ResetColor();
			Console.Clear();
			Console.CursorVisible = true;
			Background.Display();
			Label.Display();
			Login.Display();
			help.Display();
			Login.SwitchTo(true);
		}

		public override string Fill()
		{
			ResetColor();
			Console.Clear();
			Display();
			Console.CursorVisible = false;
			if (Quit) return string.Empty;
			return Login.Value.ToString();
		}
	}
}