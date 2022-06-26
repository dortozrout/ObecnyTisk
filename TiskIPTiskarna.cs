using System.Text;
using System.Net.Sockets;

namespace TiskStitku
{
	class TiskIPTiskarna
	{
		public static int TiskniStitek(string adresaTiskarny, string EPLString)
		{

			// Printer IP Address and communication port
			string ipAddress = adresaTiskarny;
			int port = 9100;
			// EPL Command(s)
			//string EPLString = File.ReadAllText(@args[0], Encoding.GetEncoding("windows-1250"));
			try
			{
				// Open connection
				TcpClient client = new TcpClient();
				client.Connect(ipAddress, port);

				// Write ZPL String to connection
				StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.GetEncoding("windows-1250"));
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
				UzivRozhrani.Oznameni(" Tisk štítků na EPL tiskárně", " Chyba při tisku: " + Environment.NewLine
					+ " " + ex.Message + Environment.NewLine
					+ " Zkontroluj nastavení tiskárny v konfiguračním souboru:" + Environment.NewLine
					+ " " + Konfigurace.KonfiguracniSoubor, " Pokračuj stisknutím libovolné klávesy.");
				//MessageBox.Show(ex.Message, "Network Printer", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return 1;
			}
		}
	}
}
