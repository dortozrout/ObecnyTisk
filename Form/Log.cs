using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Form;

namespace CCData
{
	//staticka trida pro zapis do logu
	//muze se zapisovat primo nebo pomoci udalosti
	static class Log
	{
		//vlastnost pro ulozeni zpravy
		public static string Zprava { get; set; }
		//zapise zpravu predanou v parametru metody
		public static void Zapis(string logZprava)
		{
			try
			{
				File.AppendAllText(Konfigurace.LogCesta, logZprava + Environment.NewLine);
				//File.AppendAllText(Konfigurace.LogCesta, logZprava);
			}
			catch (Exception ex)
			{
				ErrorHandler errorHandler = new ErrorHandler();
				errorHandler.ZpracujChybu("Konfigurace", ex);
			}
		}
		//zapise zpravu nastavenou ve vlastnosti 
		public static void Zapis()
		{
			try
			{
				File.AppendAllText(Konfigurace.LogCesta, Zprava + Environment.NewLine);
			}
			catch (Exception ex)
			{
				ErrorHandler errorHandler = new ErrorHandler();
				errorHandler.ZpracujChybu("Konfigurace", ex);
			}
		}
		//Zapise zpravu vyvolanou udalosti
		public static void Zapis(object sender, UdalostArgs argumenty)
		{
			try
			{
				File.AppendAllText(Konfigurace.LogCesta, argumenty.LogText + Environment.NewLine);
			}
			catch (Exception ex)
			{
				ErrorHandler errorHandler = new ErrorHandler();
				errorHandler.ZpracujChybu("Konfigurace", ex);
			}
		}
		public static void Zapis(object sender, MyEventArgs argumenty)
		{
			string logZprava = string.Empty;
			if (argumenty.OldText != argumenty.NewText)
			{
				if ((sender as FormItem).Label.Contains("DMGS1 kód:"))
				{
					string logCode = argumenty.NewText.Replace('\u001D', '\u2194');
					logZprava = DateTime.Now + " Naskenovaný kód: " + logCode + Environment.NewLine;
				}
				else if ((sender as FormItem).Label.Contains("Šarže:") && (sender as UInputField<string>).IsActivable)
				{
					logZprava = DateTime.Now + " Manuálně zadaná šarže: " + argumenty.NewText + Environment.NewLine;
				}
				else if ((sender as FormItem).Label.Contains("Expirace:") && (sender as UInputField<DateTime>).IsActivable)
				{
					logZprava = DateTime.Now + " Manuálně zadaná expirace: " + argumenty.NewText + Environment.NewLine;
				}
			}
			if (logZprava != string.Empty)
			{
				try
				{
					File.AppendAllText(Konfigurace.LogCesta, logZprava);
				}
				catch (Exception ex)
				{
					ErrorHandler errorHandler = new ErrorHandler();
					errorHandler.ZpracujChybu("Konfigurace", ex);
				}
			}
		}
	}
}