using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using Form;

namespace TiskStitku
{
	public class Manager
	{
		public void Interface()
		{
			List<string> seznamVoleb = new List<string>() { "Editace souboru s daty", "Editace šablon", "Editace konfiguračního souboru", "Nápověda" };
			string volba;
			SelectFromList<string> selecList = new SelectFromList<string>();
			do
			{
				volba = selecList.Select(seznamVoleb);
				switch (seznamVoleb.IndexOf(volba))
				{
					case 1:
						EditData();
						break;
					case 2:
						EditTemplates();
						break;
					case 0:
						EditConfigFile();
						break;
					case 3:
						ShowReadme();
						break;
					default:
						break;
				}

			} while (volba != null);
			Restart();
		}
		public void EditTemplates()
		{
			RunExternalProcess("explorer",Path.GetFullPath(Configuration.TemplatesDirectory));
			//RunExternalProcess(@"C:\Program Files (x86)\FreeCommander\FreeCommander.exe", Path.GetFullPath(Configuration.TemplatesDirectory));
		}
		public void EditConfigFile(bool waitForExit = false)
		{
			RunExternalProcess(path: Path.GetFullPath(Path.Combine(Configuration.ConfigPath, Configuration.ConfigFile)), waitForExit: waitForExit);
		}
		public void EditData()
		{
			RunExternalProcess(path: Configuration.PrimaryDataAdress);
		}
		public void ShowReadme()
		{
			//string adrReadMe = @".\Readme.txt";
			string adrReadMe = @".\zdrojak\Readme.txt";
			if (File.Exists(adrReadMe))
			{
				RunExternalProcess(path: Path.GetFullPath(adrReadMe));
			}
			else ErrorHandler.HandleError(this, new FileNotFoundException(" Nenašel jsem soubor s nápovědou:" + Path.GetFullPath(adrReadMe)));
		}
		public void Restart()
		{
			//novy beh programu
			Program.Run(new string[] { Configuration.ConfigFile });
			//aby stary beh nepokracoval
			Environment.Exit(0);
		}
		
		// private void RunExternalProcess(string command = "notepad.exe", string path = "", bool waitForExit = false)
		private void RunExternalProcess(string command = "mousepad", string path = "", bool waitForExit = false)
		{
			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.FileName = command;
					process.StartInfo.Arguments = Path.GetFullPath(path);
					process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

					bool isStarted = process.Start();

					if (isStarted)
					{
						if (waitForExit)
						{
							process.WaitForExit(6000000); // Wait for the process to exit with a timeout
						}
					}
					else
					{
						Console.WriteLine("The process could not be started.");
					}
				}
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleError(this, ex);
			}
		}

	}
}