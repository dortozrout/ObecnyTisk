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
                eplFiles = eplFiles.FindAll(e => e.FileName.Contains(filter.ToLower(), StringComparison.OrdinalIgnoreCase));
            }
            return eplFiles;
        }
    }
}