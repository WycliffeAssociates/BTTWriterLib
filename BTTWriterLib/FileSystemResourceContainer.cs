using BTTWriterLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BTTWriterLib
{
    public class FileSystemResourceContainer : IResourceContainer
    {
        private string baseDirectory;
        public FileSystemResourceContainer(string directory)
        {
            baseDirectory = directory;
        }
        public string GetFile(string fileName)
        {
            string path = Path.Combine(baseDirectory, ConvertFileName(fileName));
            if (!File.Exists(path))
            {
                return null;
            }
            return File.ReadAllText(path);
        }

        private string ConvertFileName(string fileName)
        {
            return fileName.Replace("-", "/") + ".txt";
        }

        public BTTWriterManifest GetManifest()
        {
            try
            {
                string path = Path.Combine(baseDirectory, "manifest.json");
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Could not load manifest {path}");
                }
                var contents = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<BTTWriterManifest>(contents) ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> GetFiles(bool onlyFinished)
        {
            if (onlyFinished)
            {
                return GetManifest().finished_chunks.ToList();
            }

            var output = new List<string>();
            foreach(var file in Directory.GetFiles(baseDirectory, "*.txt", SearchOption.AllDirectories))
            {
                string dirName = GetLastDirInPath(file);
                string fileName = Path.GetFileNameWithoutExtension(file);
                output.Add($"{dirName}-{fileName}");
            }
            return output;
        }
        string GetLastDirInPath(string path)
        {
            var splitPath = path.Split(Path.DirectorySeparatorChar);
            return splitPath[splitPath.Length - 2];
        }

    }
}
