using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Labels
{
	public class EplFile : IComparable
	{
		public string FileName { get; set; }
		public string Template { get; private set; }
		public string Body { get; set; }
		public bool print = true;
		public EplFile() { }
		public EplFile(string name, string template) //konstruktor
		{
			FileName = name;
			Template = template;
		}

		public int CompareTo(object obj)
		{
			EplFile anotherEplFile = (EplFile)obj;
			int rv = string.Compare(FileName, anotherEplFile.FileName);
			return rv;
		}
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			var other = (EplFile)obj;
			return FileName == other.FileName;
		}

		public override int GetHashCode()
		{
			return FileName.GetHashCode();
		}
		public override string ToString()
		{
			return FileName;
		}
		public EplFile Copy()
		{
			return new EplFile()
			{
				FileName = FileName,
				Template = Template,
			};
		}
	}
}