using System;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace Labels
{
	class TiskIPTiskarna
	{
		public static int TiskniStitek(string adresaTiskarny, string EPLString)
		{

			// Printer IP Address and communication port
			string ipAddress = adresaTiskarny;
			int port = 9100;
			try
			{
				// Open connection
				TcpClient client = new TcpClient();
				client.Connect(ipAddress, port);

				// Write ZPL String to connection
				StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.GetEncoding("windows-1250"));
				//pridavam "N" na zacatek kvuli vymazani predchoziho stitku
				writer.Write("N" + Environment.NewLine);
				writer.Write(EPLString);
				writer.Flush();

				// Close Connection
				writer.Close();
				client.Close();
				return 0;
			}
			catch (Exception ex)
			{
				// Catch Exception
				ErrorHandler.HandleError("TiskIPTiskarna",ex);
				return 1;
			}
		}
	}
}
