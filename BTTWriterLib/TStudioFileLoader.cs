using BTTWriterLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace BTTWriterLib
{
    public class TStudioFileLoader : IResourceContainer
    {
        private string filePath;
        ZipArchive archive;
        string inDirPath;
        public TStudioFileLoader(string file)
        {
            filePath = file;
            archive = ZipFile.OpenRead(filePath);
            var mainManifestText = LoadFileContents("manifest.json");
            if (mainManifestText == null)
            {
                throw new Exception("File is missing a manifest");
            }

            var mainManifest = JsonConvert.DeserializeObject<BTTWriterManifest>(mainManifestText);

            if (mainManifest.target_translations.Length == 0)
            {
                throw new Exception("No projects found in file");
            }

            inDirPath = mainManifest.target_translations[0].path;
        }
        private string LoadFileContents(string filePath)
        {
            if (!archive.Entries.Any(e => e.FullName == filePath))
            {
                return null;
            }

            using (var stream = archive.GetEntry(filePath).Open())
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public string GetFile(string fileName)
        {
            fileName = fileName.Replace("-", "/") + ".txt";
            return LoadFileContents(CombinePath(inDirPath, fileName));
        }

        /// <summary>
        /// A path join function for combining paths inside of a zip file
        /// </summary>
        /// <param name="first">The first part of the path to combine</param>
        /// <param name="second">The second part of the path to combine</param>
        /// <returns>The combined path</returns>
        private string CombinePath(string first, string second)
        {
            return first.TrimEnd('/') + "/" + second.TrimStart('/');
        }

        public BTTWriterManifest GetManifest()
        {
            var manifestText = LoadFileContents(CombinePath(inDirPath, "manifest.json"));
            if (manifestText == null)
            {
                throw new Exception($"No manifest found in {inDirPath}");
            }
            return JsonConvert.DeserializeObject<BTTWriterManifest>(manifestText);
        }

        public List<string> GetFiles(bool onlyFinished)
        {
            if (onlyFinished)
            {
                return GetManifest().finished_chunks.ToList();
            }
            return archive.Entries.Where(e => e.Name.EndsWith(".txt")).Select(e => $"{GetLastDirInPath(e.FullName)}-{Path.GetFileNameWithoutExtension(e.FullName)}").ToList();
        }

        string GetLastDirInPath(string path)
        {
            var splitPath = path.Split('/');
            return splitPath[splitPath.Length - 2];
        }
    }
}
