using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Labels
{
	public class EplFile : IComparable
	{
		public string FileAddress { get; private set; }
		public string FileName { get; private set; }
		public string Template { get; private set; }
		public string Body { get; set; }
		public Dictionary<string, string> KeyValuePairs { get; private set; }

		public bool print = true;
		public EplFile(string fileAddress, string template) //konstruktor
		{
			FileAddress = fileAddress;
			FileName = Path.GetFileName(FileAddress);
			Template = template;
		}
		public EplFile(string name, string template, Dictionary<string, string> keyValuePairs)
		{
			FileName = name;
			Template = template;
			KeyValuePairs = keyValuePairs;
		}

		public int CompareTo(object obj)
		{
			EplFile anotherEplFile = (EplFile)obj;
			int rv = string.Compare(FileName, anotherEplFile.FileName);
			return rv;
		}
		public override string ToString()
		{
			return FileName;
		}
	}
}