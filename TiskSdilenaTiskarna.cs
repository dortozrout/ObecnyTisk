using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Runtime.InteropServices;

namespace TiskStitku
{
	public static class TiskSdilenaTiskarna
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern SafeFileHandle CreateFile(string lpFileName, FileAccess dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		public static bool Print(string printerAddress, string eplText)
		{
			bool IsConnected = true;
			try
			{
				Byte[] buffer = new byte[eplText.Length];
				buffer = Encoding.GetEncoding("windows-1250").GetBytes(eplText);
				SafeFileHandle fh = CreateFile(printerAddress, FileAccess.Write, 0, IntPtr.Zero, FileMode.OpenOrCreate, 0, IntPtr.Zero);
				FileStream lpt1 = new FileStream(fh, FileAccess.ReadWrite);
				lpt1.Write(buffer, 0, buffer.Length);
				lpt1.Close();
			}
			catch (Exception ex)
			{
				// Catch Exception
				UzivRozhrani.Oznameni(" Tisk štítků na EPL tiskárně", " Chyba při tisku: " + Environment.NewLine
					+ " " + ex.Message + Environment.NewLine
					+ " Zkontroluj nastavení tiskárny v konfiguračním souboru:" + Environment.NewLine
					+ " " + Konfigurace.KonfiguracniSoubor, " Pokračuj stisknutím libovolné klávesy.");
				IsConnected = false;
			}
			return IsConnected;
		}
	}
}