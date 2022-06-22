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
		public List<Dotaz> SeznamDotazu { get; private set; } 
		public EplPrikaz(string nazevSouboru, string telo)
		{
			NazevSouboru = nazevSouboru;
			Telo = telo;
		}
	}
}