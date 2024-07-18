using System;
using Form;

namespace CCData
{
	class ErrorHandler
	{
		public object sender { get; set; }
		public Exception exception { get; set; }
		public event EventHandler<UdalostArgs> Error;
		public void ZpracujChybu(object zdroj, Exception exception)
		{
			ErrorForm errorForm = new ErrorForm(zdroj.ToString(), exception.Message);
			string textLog = string.Format("{0}|Neošetřená vyjímka: {1}|Zdroj: {2}", DateTime.Now, exception.Message, zdroj);
			UdalostArgs udalostArgs = new UdalostArgs() { LogText = textLog };
			Error += Log.Zapis;
			if (Error != null) Error(this, udalostArgs);
			errorForm.Display();
			Console.ReadKey();
			Environment.Exit(1);
		}
		//pro staticke tridy
		public void ZpracujChybu(string zdroj, Exception exception)
		{
			ErrorForm errorForm = new ErrorForm(zdroj, exception.Message);
			string textLog = string.Format("{0}|Neošetřená vyjímka: {1}|Zdroj: {2}", DateTime.Now, exception.Message, zdroj);
			UdalostArgs udalostArgs = new UdalostArgs() { LogText = textLog };
			if (zdroj != "Konfigurace") Error += Log.Zapis;
			if (Error != null) Error(this, udalostArgs);
			errorForm.Display();
			Console.ReadKey();
			Environment.Exit(1);
		}

	}
}