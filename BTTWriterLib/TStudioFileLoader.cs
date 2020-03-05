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
    /// <summary>
    /// Represents a TStudio/BTTWriter file resource container
    /// </summary>
    public class TStudioFileLoader : IResourceContainer
    {
        private string filePath;
        ZipArchive archive;
        string inDirPath;
        /// <summary>
        /// Create an instance of the the resource container
        /// </summary>
        /// <param name="file">Path to the file to load</param>
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

        /// <summary>
        /// Loads a file's contents from the TS archive
        /// </summary>
        /// <param name="filePath">The relative path inside of zip file</param>
        /// <returns>The contents of the file or null if the file is missing</returns>
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

        /// <summary>
        /// Get a file from the container
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the manifest from the resource container
        /// </summary>
        /// <returns>The manifest from the container</returns>
        public BTTWriterManifest GetManifest()
        {
            var manifestText = LoadFileContents(CombinePath(inDirPath, "manifest.json"));
            if (manifestText == null)
            {
                throw new Exception($"No manifest found in {inDirPath}");
            }
            return JsonConvert.DeserializeObject<BTTWriterManifest>(manifestText);
        }

        /// <summary>
        /// Get a list of files from the container
        /// </summary>
        /// <param name="onlyFinished">If true only files listed as complete in the manifest will be returned</param>
        /// <returns>A list of relative file paths in the container</returns>
        public List<string> GetFiles(bool onlyFinished)
        {
            if (onlyFinished)
            {
                return GetManifest().finished_chunks.ToList();
            }
            return archive.Entries.Where(e => e.Name.EndsWith(".txt")).Select(e => $"{GetLastDirInPath(e.FullName)}-{Path.GetFileNameWithoutExtension(e.FullName)}").ToList();
        }

        /// <summary>
        /// Gets the last dir in a zip file path
        /// </summary>
        /// <param name="path">The path to get the last dir of</param>
        /// <returns>The last dir in the path</returns>
        string GetLastDirInPath(string path)
        {
            var splitPath = path.Split('/');
            return splitPath[splitPath.Length - 2];
        }
    }
}
