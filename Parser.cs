namespace TiskStitku
{
    public class Parser
    {
        private Dictionary<string, string> primaryData;
        public Parser()
        {
            primaryData = LoadPrimaryData();
        }
        private Dictionary<string, string> LoadPrimaryData()
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(Configuration.PrimaryDataAdress)) return rv;
            try
            {
                string[] dataArray = File.ReadAllLines(Configuration.PrimaryDataAdress);
                foreach (string line in dataArray)
                {
                    int indexOfSeparator = line.IndexOf(':');
                    if (!line.StartsWith('#') && indexOfSeparator > 0)
                    {
                        string key = line.Substring(0, indexOfSeparator).Trim().ToLower();
                        //key = string.Format("<{0}>", key);
                        string value = line.Substring(indexOfSeparator + 1).Trim();
                        rv.Add(key, value);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, ex);
            }
            return rv;
        }
        public void Process(ref EplPrikaz eplPrikaz)
        {
            eplPrikaz.Telo = FillOutTemplate(eplPrikaz.Sablona);
        }
        private string FillOutTemplate(string template)
        {
            if (template.Trim().EndsWith('P'))
                template = string.Format("{0}<počet štítků>{1}", template.TrimEnd(), Environment.NewLine);
            List<string> keys = ReadKeys(template);
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (string key in keys)
            {
                keyValuePairs.Add(key, FindValue(key));
            }
            Dictionary<string, string> emptyValues = findEmptyValues(keyValuePairs);

            string eplString = template;
            foreach (var keyValue in keyValuePairs)
            {
                eplString = eplString.Replace(keyValue.Key, keyValue.Value);
            }
            return eplString;
        }
        private Dictionary<string, string> findEmptyValues(Dictionary<string, string> keyValues)
        {
            Dictionary<string, string> emptyValues = new Dictionary<string, string>();
            foreach (var kv in keyValues)
            {
                if (kv.Value == string.Empty)
                    emptyValues = emptyValues.Append(kv).ToDictionary();
            }
            return emptyValues;
        }

        private List<string> ReadKeys(string template)
        {
            List<string> rv = new List<string>();
            int start = template.IndexOf('<');
            int end = template.IndexOf('>');
            while (start != -1 && end != -1)
            {
                string key = template.Substring(start, end - start + 1);
                rv.Add(key);
                //if (template.Length > end) template = template.Substring(end + 1);
                start = template.IndexOf('<', end + 1);
                end = template.IndexOf('>', end + 1);
            }
            return rv;
        }
        private string FindValue(string key)
        {
            string rv;
            try
            {
                rv = primaryData[key.Trim(['<', '>']).ToLower()];
            }
            catch
            {
                rv = FindValueNotInPrimaryData(key);
            }

            return rv;
        }
        private string FindValueNotInPrimaryData(string key)
        {
            if (key == "<GS1>")
                return "\u001D";
            if (Configuration.Prihlasit && key == "uzivatel")
                return Configuration.Uzivatel;
            if (key.StartsWith("<time"))
            {
                int indexOfPlus = key.IndexOf('+');
                if (indexOfPlus == -1)
                    return DateTime.Now.ToString("H:mm");
                int drift;
                if (int.TryParse(key.Substring(indexOfPlus + 1).TrimEnd('>'), out drift))
                    return DateTime.Now.AddMinutes(drift).ToString("H:mm");
            }
            if (key.StartsWith("<date"))
            {
                int indexOfPlus = key.IndexOf('+');
                if (indexOfPlus == -1)
                    return DateTime.Now.ToString("dd.MM.yyyy");
                string[] keyArray = key.Trim(['<', '>']).Split(['+', '|']);
                int drift;
                if (int.TryParse(keyArray[1], out drift))
                {
                    DateTime bottleExpiration = DateTime.Now.AddDays(drift);
                    if (keyArray.Count() == 2)
                        return bottleExpiration.ToString("dd.MM.yyyy");
                    if (key.IndexOf('|') != -1 && keyArray.Count() == 3)
                    {
                        DateTime lotExpiration;
                        try
                        {
                            lotExpiration = DateTime.Parse(primaryData[keyArray[2].ToLower()]);
                            DateTime dateToPrint = bottleExpiration < lotExpiration ? bottleExpiration : lotExpiration;
                            return dateToPrint.ToString("dd.MM.yyyy");
                        }
                        catch
                        {
                            return string.Empty;
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}