using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using Form;

namespace Labels
{
	public class Manager
	{
		private string editor;
		private string fileManager;
		public Manager()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				editor = "notepad";
				fileManager = "explorer";
				//fileManager = @"C:\Program Files (x86)\FreeCommander\FreeCommander.exe";
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				editor = "xdg-open";
				fileManager = "xdg-open";
			}
			else
			{
				editor = "";
				fileManager = "";
			}
		}
		public void Interface()
		{
			List<string> choices = new List<string>()
			{
				"Editace souboru s daty",
				"Editace šablon",
				"Editace konfiguračního souboru",
				"Editace šablony hlavní",
				"Editace vstupních dat pro hlavní šablonu",
				"Nápověda"
			};
			string selectedChoice;
			SelectFromList<string> selecList = new SelectFromList<string>();
			do
			{
				var selection = selecList.Select(choices);
				selectedChoice = selection?[0];
				switch (choices.IndexOf(selectedChoice))
				{
					case 0:
						EditConfigFile();
						break;
					case 1:
						EditData();
						break;
					case 2:
						EditTemplates();
						break;
					case 3:
						EditMasterTemplate();
						break;
					case 4:
						EditMasterTemplateInput();
						break;
					case 5:
						ShowReadme();
						break;
					default:
						break;
				}
			} while (selectedChoice != null);
			Restart();
		}
		public void EditTemplates()
		{
			RunExternalProcess(fileManager, Path.GetFullPath(Configuration.TemplatesDirectory));
		}
		public void EditConfigFile(bool waitForExit = false)
		{
			RunExternalProcess(editor, path: Path.GetFullPath(Path.Combine(Configuration.ConfigPath, Configuration.ConfigFile)), waitForExit: waitForExit);
		}
		public void EditData()
		{
			RunExternalProcess(editor, path: Configuration.PrimaryDataAdress);
		}
		public void EditMasterTemplate()
		{
			RunExternalProcess(editor, path: Configuration.MasterTemplateAddress);
		}
		public void EditMasterTemplateInput()
		{
			RunExternalProcess(editor, path: Configuration.MasterTemplateInputAddress);
		}
		public void ShowReadme()
		{
			//string adrReadMe = @".\Readme.txt";
			//string adrReadMe = @".\zdrojak\Readme.txt";
			string adrReadMe = Path.Combine(".", "zdrojak", "Readme.txt");
			if (File.Exists(adrReadMe))
			{
				RunExternalProcess(editor, path: Path.GetFullPath(adrReadMe));
			}
			else ErrorHandler.HandleError(this, new FileNotFoundException("Nenašel jsem soubor s nápovědou:" + Path.GetFullPath(adrReadMe)));
		}
		public void Restart()
		{
			//novy beh programu
			Program.Run(new string[] { Configuration.ConfigFile });
			//aby stary beh nepokracoval
			Environment.Exit(0);
		}

		// private void RunExternalProcess(string command = "mousepad", string path = "", bool waitForExit = false)
		private void RunExternalProcess(string command = "notepad.exe", string path = "", bool waitForExit = false)
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