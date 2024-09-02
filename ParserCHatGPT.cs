using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Form;

namespace Labels
{
    public class Parser
    {
        private readonly Dictionary<string, string> primaryData;
        private EplFile CurrentEplFile { get; set; }
        private bool continueProcessing;

        public Parser()
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

        public void Process(ref EplFile eplFile)
        {
            continueProcessing = true;
            CurrentEplFile = eplFile;
            eplFile.Body = FillOutTemplate(eplFile.Template);
        }

        private string FillOutTemplate(string template)
        {
            template = RemoveCommentedLines(template);
            if (template.Trim().EndsWith("P"))
            {
                template = string.Format("{0}<pocet>{1}", template.TrimEnd(), Environment.NewLine);
            }

            List<string> keys = ReadKeys(template);
            var keyValuePairs = keys.ToDictionary(key => key, key => continueProcessing ? FindValue(key) : "");

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
                if (end < start) ErrorHandler.HandleError(this, new ArgumentOutOfRangeException());
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

            if (Configuration.Login && key == "<uzivatel>")
                return Configuration.User;

            if (key.StartsWith("<time"))
                return HandleTimeKey(key);

            if (key.StartsWith("<date"))
                return HandleDateKey(key);

            if (key.StartsWith("<pocet"))
                return HandlePocetKey(key);

            return HandleDefaultKey(key);
        }
        private T HandleInput<T>(EplFile eplFile, string prompt, string defaultValue)
        {
            InputForm<T> inputForm = new InputForm<T>();
            T rv = inputForm.Fill(eplFile, prompt, defaultValue);
            if (inputForm.Quit)
            {
                continueProcessing = false;
                return default(T);
            }
            return rv;
        }
        private string HandleTimeKey(string key)
        {
            int indexOfPlus = key.IndexOf('+');
            if (indexOfPlus == -1)
                return DateTime.Now.ToString("H:mm");

            int drift;
            if (int.TryParse(key.Substring(indexOfPlus + 1).TrimEnd('>'), out drift))
                return DateTime.Now.AddMinutes(drift).ToString("H:mm");

            drift = HandleInput<int>(CurrentEplFile, "Zadej počet minut: ", "30");
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
                if (keyArray.Length == 2)
                    return bottleExpiration.ToString("dd.MM.yyyy");

                if (key.IndexOf('|') > 0 && keyArray.Length == 3)
                {
                    DateTime lotExpiration = GetLotExpiration(keyArray[2]);
                    if (lotExpiration < DateTime.Today)
                    {
                        new NotificationForm("Expirovaná šarže", $"Datum expirace materiálu ({lotExpiration.ToShortDateString()}) je v minulosti. Štítky nebudou vytištěny! Zkontroluj případně uprav expiraci...").Display();
                        Console.ReadKey();
                        continueProcessing = false;
                        CurrentEplFile.print = false;
                        return string.Empty;
                    }
                    else if(lotExpiration<DateTime.Today.AddMonths(1))
                    {
                        new NotificationForm("Blíží se expirace materiálu", $"Datum expirace materiálu ({lotExpiration.ToShortDateString()}) je menší něž 1 měsíc. Zkontroluj případně uprav expiraci...").Display();
                        Console.ReadKey();
                    }
                    DateTime dateToPrint = bottleExpiration < lotExpiration ? bottleExpiration : lotExpiration;
                    return dateToPrint.ToString("dd.MM.yyyy");
                }
            }
            return string.Empty;
        }
        private DateTime GetLotExpiration(string key)
        {
            DateTime lotExpiration;
            if (DateTime.TryParse(key, out lotExpiration))
            {
                return lotExpiration;
            }
            if (primaryData.TryGetValue(key.ToLower(), out string lotExpirationStr)
                    && DateTime.TryParse(lotExpirationStr, out lotExpiration))
            {
                return lotExpiration;
            }
            lotExpiration = HandleInput<DateTime>(CurrentEplFile, "Zadej expiraci: ", DateTime.MaxValue.ToString("dd.MM.yyyy"));
            return lotExpiration;
        }

        private string HandlePocetKey(string key)
        {
            var inputForm = new InputForm<int>();
            int indexOfSeparator = key.IndexOf('|');
            if (indexOfSeparator < 1)
                return inputForm.Fill(CurrentEplFile, "Zadej počet štítků: ", "1").ToString();

            int defaultQuantity;
            if (int.TryParse(key.Substring(indexOfSeparator + 1).TrimEnd('>'), out defaultQuantity))
            {
                defaultQuantity = HandleInput<int>(CurrentEplFile, "Zadej počet štítků: ", defaultQuantity.ToString());
                return defaultQuantity.ToString();
            }
            return "1";
        }
        private string HandleDefaultKey(string key)
        {
            string rv = HandleInput<string>(CurrentEplFile, "Zadej " + key.Trim('<', '>') + ": ", "");
            return rv;
        }
        private string RemoveCommentedLines(string input)
        {
            // Split the input string into lines
            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // Filter out lines that start with '#', after trimming leading whitespace
            var filteredLines = lines.Where(line => !line.TrimStart().StartsWith("#"));

            // Join the filtered lines back into a single string
            return string.Join(Environment.NewLine, filteredLines);
        }
    }
}
