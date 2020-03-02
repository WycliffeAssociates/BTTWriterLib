using BTTWriterLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BTTWriterLib
{
    public interface IResourceContainer
    {
        /// <summary>
        /// Get manifest from resource container
        /// </summary>
        /// <returns>A manifest</returns>
        BTTWriterManifest GetManifest();
        /// <summary>
        /// Get all of the files inside of the resource container
        /// </summary>
        /// <param name="onlyFinished">If true only returns those listed as finished in the manifest</param>
        /// <returns>A list of file paths</returns>
        List<string> GetFiles(bool onlyFinished);
        /// <summary>
        /// Get the contents of a single file in the resource container
        /// </summary>
        /// <param name="fileName">The name of the file in the resource container</param>
        /// <remarks>Files names are listed as chapter-chunk</remarks>
        /// <returns>The contents of the file or null if it can't be found</returns>
        string GetFile(string fileName);
    }
}
