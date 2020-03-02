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
        /// <summary>
        /// Create a new instance of a file system resource container
        /// </summary>
        /// <param name="directory">The directory which contains the resource container</param>
        public FileSystemResourceContainer(string directory)
        {
            baseDirectory = directory;
        }
        /// <summary>
        /// Get the contents of a file
        /// </summary>
        /// <param name="fileName">The name of the file to get</param>
        /// <returns>The files contents or null if it is missing</returns>
        public string GetFile(string fileName)
        {
            string path = Path.Combine(baseDirectory, ConvertFileName(fileName));
            if (!File.Exists(path))
            {
                return null;
            }
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Converts a file name from the chapter-chunk format to an actual relative path
        /// </summary>
        /// <param name="fileName">The incoming file name</param>
        /// <returns>A converted path</returns>
        private string ConvertFileName(string fileName)
        {
            return fileName.Replace("-", "/") + ".txt";
        }

        /// <summary>
        /// Get the manifest from the resource container
        /// </summary>
        /// <returns>The manifest from the resource container</returns>
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

        /// <summary>
        /// Get a list of all files in the container
        /// </summary>
        /// <param name="onlyFinished">If set to true it will only return files in the manifest</param>
        /// <returns>A list of files</returns>
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
        /// <summary>
        /// Get the last occuring dir in a path
        /// </summary>
        /// <param name="path">The original path</param>
        /// <returns>The last dir in a path</returns>
        private string GetLastDirInPath(string path)
        {
            var splitPath = path.Split(Path.DirectorySeparatorChar);
            return splitPath[splitPath.Length - 2];
        }

    }
}
