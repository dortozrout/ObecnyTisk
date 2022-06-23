using System;
using System.Text;
using System.Collections.Generic;
//using System.Net;
//using System.Net.Sockets;
//using System.IO;

namespace TiskStitku
{
	class EplPrikaz
	{
		public string NazevSouboru { get; private set; }
		public string Telo { get; private set; }
		public List<Dotaz> SeznamDotazu { get; set; }
		public EplPrikaz(string nazevSouboru, string telo)
		{
			NazevSouboru = nazevSouboru;
			Telo = telo;
		}
		public void UpravTelo(List<Dotaz> listDotazu)
		{
			foreach (Dotaz dotaz in listDotazu)
			{
				if (dotaz.Otazka == "počet štítků")
					Telo = Telo.TrimEnd(new char[] { '\r', '\n' }) + dotaz.Odpoved + Environment.NewLine;
				else Telo = Telo.Replace("<" + dotaz.Otazka + ">", "\"" + dotaz.Odpoved + "\"");
			}
		}
	}
}