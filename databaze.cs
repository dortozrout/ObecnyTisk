using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace TiskStitku
{
	class DatabazePrikazu
	{
		private List<EplPrikaz> seznam = new List<EplPrikaz>();
		//konstruktor vytvori list epl prikazu ze souboru v danem umisteni
		public DatabazePrikazu(string AdresaSlozky)
		{
			var jmenaSouboru = Directory.EnumerateFiles(AdresaSlozky); //jmena souboru jsou vcetne cesty
			foreach (string jmenoSouboru in jmenaSouboru)
			{
				string teloSouboru = File.ReadAllText(jmenoSouboru, Encoding.GetEncoding(Konfigurace.Kodovani));
				//string teloSouboru = File.ReadAllText(jmenoSouboru, Encoding.GetEncoding("windows-1250"));
				EplPrikaz prikaz = new EplPrikaz(jmenoSouboru, teloSouboru);
				seznam.Add(prikaz);
			}
		}
		//vraci cely seznam
		public List<EplPrikaz> VratSeznam()
		{
			return this.seznam;
		}
		//vraci seznam vyhledanych prikazu
		public List<EplPrikaz> VratSeznam(string hledanyText)
		{
			List<EplPrikaz> seznam = new List<EplPrikaz>();
			hledanyText = hledanyText.ToLower();
			foreach(EplPrikaz prikaz in this.seznam)
			{
				if (prikaz.NazevSouboru.ToLower().Contains(hledanyText))
				{
					seznam.Add(prikaz);
				}
			}
			return seznam;
		}

		public List<EplPrikaz> Najdi()
		{
			return null;
		}

	}
}