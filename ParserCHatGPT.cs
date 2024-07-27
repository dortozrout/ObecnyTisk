using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Form;

namespace TiskStitku
{
    public class Parser1
    {
        private readonly Dictionary<string, string> primaryData;

        public Parser1()
        {
            primaryData = LoadPrimaryData();
        }

        private Dictionary<string, string> LoadPrimaryData()
        {
            var rv = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(Configuration.PrimaryDataAdress)) return rv;

            try
            {
                string[] dataArray = File.ReadAllLines(Configuration.PrimaryDataAdress);
                foreach (string line in dataArray)
                {
                    int indexOfSeparator = line.IndexOf(':');
                    if (!line.StartsWith("#") && indexOfSeparator > 0)
                    {
                        string key = line.Substring(0, indexOfSeparator).Trim().ToLower();
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
            if (template.Trim().EndsWith("P"))
            {
                template = string.Format("{0}<pocet>{1}", template.TrimEnd(), Environment.NewLine);
                //template = $"{template.TrimEnd()}<pocet>{Environment.NewLine}";
            }

            List<string> keys = ReadKeys(template);
            var keyValuePairs = keys.ToDictionary(key => key, key => FindValue(key));

            foreach (var keyValue in keyValuePairs)
            {
                template = template.Replace(keyValue.Key, keyValue.Value);
            }

            return template;
        }

        private List<string> ReadKeys(string template)
        {
            var rv = new List<string>();
            int start = template.IndexOf('<');
            int end = template.IndexOf('>');

            while (start != -1 && end != -1)
            {
                string key = template.Substring(start, end - start + 1);
                rv.Add(key);
                start = template.IndexOf('<', end + 1);
                end = template.IndexOf('>', end + 1);
            }

            return rv.Distinct().ToList();
        }

        private string FindValue(string key)
        {
            try
            {
                return primaryData[key.Trim('<', '>').ToLower()];
            }
            catch (KeyNotFoundException)
            {
                return FindValueNotInPrimaryData(key);
            }
        }

        private string FindValueNotInPrimaryData(string key)
        {
            if (key == "<GS1>")
                return "\u001D";

            if (Configuration.Prihlasit && key == "<uzivatel>")
                return Configuration.Uzivatel;

            if (key.StartsWith("<time"))
                return HandleTimeKey(key);

            if (key.StartsWith("<date"))
                return HandleDateKey(key);

            if (key.StartsWith("<pocet"))
                return HandlePocetKey(key);

            return HandleDefaultKey(key);
        }

        private string HandleTimeKey(string key)
        {
            int indexOfPlus = key.IndexOf('+');
            if (indexOfPlus == -1)
                return DateTime.Now.ToString("H:mm");

            int drift;
            if (int.TryParse(key.Substring(indexOfPlus + 1).TrimEnd('>'), out drift))
                return DateTime.Now.AddMinutes(drift).ToString("H:mm");

            var inputForm = new InputForm<int>();
            drift = inputForm.Fill("Zadej počet minut: ", "30");
            return DateTime.Now.AddMinutes(drift).ToString("H:mm");
        }

        private string HandleDateKey(string key)
        {
            int indexOfPlus = key.IndexOf('+');
            if (indexOfPlus == -1)
                return DateTime.Now.ToString("dd.MM.yyyy");

            var keyArray = key.Trim('<', '>').Split(new[] { '+', '|' }, StringSplitOptions.None);
            int drift;
            if (int.TryParse(keyArray[1], out drift))
            {
                DateTime bottleExpiration = DateTime.Now.AddDays(drift);
                string lotExpirationStr;
                DateTime lotExpiration;
                if (keyArray.Length == 2)
                    return bottleExpiration.ToString("dd.MM.yyyy");

                if (key.IndexOf('|') != -1 && keyArray.Length == 3)
                {
                    if (primaryData.TryGetValue(keyArray[2].ToLower(), out lotExpirationStr) &&
                        DateTime.TryParse(lotExpirationStr, out lotExpiration))
                    {
                        DateTime dateToPrint = bottleExpiration < lotExpiration ? bottleExpiration : lotExpiration;
                        return dateToPrint.ToString("dd.MM.yyyy");
                    }
                }
            }
            return string.Empty;
        }

        private string HandlePocetKey(string key)
        {
            var inputForm = new InputForm<int>();
            int indexOfSeparator = key.IndexOf('|');
            if (indexOfSeparator < 1)
                return inputForm.Fill("Zadej počet štítků: ", "1").ToString();

            int defaultQuantity;
            if (int.TryParse(key.Substring(indexOfSeparator + 1).TrimEnd('>'), out defaultQuantity))
                return inputForm.Fill("Zadej počet štítků: ", defaultQuantity.ToString()).ToString();

            return "1";
        }

        private string HandleDefaultKey(string key)
        {
            var inputForm = new InputForm<string>();
            //return inputForm.Fill($"Zadej {key.Trim('<', '>')}: ", "");
            return inputForm.Fill("Zadej " + key.Trim('<', '>') + ": ", "");
        }
    }
}
