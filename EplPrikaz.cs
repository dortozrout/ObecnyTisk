using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TiskStitku
{
	public class EplFile : IComparable
	{
		public string AdresaSouboru { get; private set; }
		public string JmenoSouboru { get; private set; }
		public string Sablona { get; private set; }
		public string Telo { get; set; }
		public EplFile(string adresaSouboru, string sablona) //konstruktor
		{
			AdresaSouboru = adresaSouboru;
			JmenoSouboru = Path.GetFileName(AdresaSouboru);
			Sablona = sablona;
		}

		public int CompareTo(object obj)
		{
			EplFile anotherEplPrikaz = (EplFile)obj;
			int rv = string.Compare(JmenoSouboru, anotherEplPrikaz.JmenoSouboru);
			return rv;
		}
		public override string ToString()
		{
			return JmenoSouboru;
		}
	}
}