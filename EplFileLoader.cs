using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Labels
{
    public class EplFileLoader
    {
        public List<EplFile> LoadFiles(string directoryPath, string filter = null)
        {
            var eplFiles = new List<EplFile>();

            try
            {
                var files = Directory.GetFiles(directoryPath);
                foreach (var file in files)
                {
                    string content = File.ReadAllText(file);
                    var eplFile = new EplFile(file, content);
                    eplFiles.Add(eplFile);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, ex);
            }
            if (filter != null)
            {
                eplFiles = eplFiles.FindAll(e => e.FileName.ToLower().Contains(filter.ToLower()));
            }
            return eplFiles;
        }
        public List<EplFile> ReadFromFile(string filePath, string templatePath)
        {
            var eplFiles = new List<EplFile>();
            try
            {
                var fileLines = File.ReadAllLines(filePath);
                string template = File.ReadAllText(templatePath);
                //List<string> keys;
                string[] keys = Array.Empty<string>();
                string[] values;
                foreach (string line in fileLines)
                {
                    if (!line.StartsWith(";") && !line.StartsWith("─") && !line.StartsWith("#") && line != string.Empty)
                    {
                        if (line.StartsWith("keys"))
                        {
                            int indexOfSeparator = line.IndexOf(':');
                            keys = line.Substring(indexOfSeparator + 1).Trim().Split();
                        }
                        else
                        {
                            values = line.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (keys.Length == values.Length)
                            {
                                string templateCopy=template;
                                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                                for (int i = 0; i < keys.Length; i++)
                                {
                                    templateCopy=templateCopy.Replace(keys[i],values[i]);
                                    keyValuePairs.Add(keys[i], values[i]);
                                }
                                eplFiles.Add(new EplFile(values[0], templateCopy, keyValuePairs));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return eplFiles;
        }
    }
}