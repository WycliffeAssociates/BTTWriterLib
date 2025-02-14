using BTTWriterLib.Models;
using System;
using USFMToolsSharp.Models.Markers;
using USFMToolsSharp;
using System.Collections.Generic;
using System.Linq;

namespace BTTWriterLib
{
    public class BTTWriterLoader
    {
        /// <summary>
        /// Create a usfm document from a resource container
        /// </summary>
        /// <param name="resourceContainer">The resource container to get the information from</param>
        /// <param name="onlyComplete">If set to true only chunks labeled as complete in the manifest will be included</param>
        /// <returns>A USFM document built from the container</returns>
        public static USFMDocument CreateUSFMDocumentFromContainer(IResourceContainer resourceContainer, bool onlyComplete, USFMParser parser = null)
        {
            if (parser == null)
            {
                parser = new USFMParser(new List<string> { "s5", "fqa*" });
            }
            var manifest = resourceContainer.GetManifest();
            USFMDocument document = new USFMDocument();

            string bookName = resourceContainer.GetFile("front-title");
            if (bookName == null)
            {
                bookName = manifest.project.name;
            }

            var files = resourceContainer.GetFiles(onlyComplete);

            document.Insert(new IDMarker() { TextIdentifier = manifest.project.id });
            document.Insert(new IDEMarker() { Encoding = "UTF-8" });
            document.Insert(new TOC1Marker() { LongTableOfContentsText = bookName });
            document.Insert(new TOC2Marker() { ShortTableOfContentsText = bookName });
            document.Insert(new TOC3Marker() { BookAbbreviation = manifest.project.id });
            document.Insert(new HMarker() { HeaderText = bookName });
            document.Insert(new MTMarker() { Title = bookName });

            // Get a distinct list of chapters from the contents that are actually numbers
            var sortedFiles = files.Select(e => e.Split('-')[0])
                .Distinct()
                .Where(e => int.TryParse(e, out int _))
                .OrderBy(e => int.Parse(e));
            foreach (var item in sortedFiles)
            {
                document.Insert(LoadChapter(resourceContainer, files, item, parser));
            }

            return document;
        }

        /// <summary>
        /// Load a specified chapter from the resource container
        /// </summary>
        /// <param name="resourceContainer">The container we are currently working with</param>
        /// <param name="files">List of files previously extracted from the resource container</param>
        /// <param name="chapter">The chapter to get chunks for</param>
        /// <returns>A USFM document for the chapter</returns>
        private static USFMDocument LoadChapter(IResourceContainer resourceContainer, List<string> files, string chapter, USFMParser parser)
        {
            USFMDocument output = new USFMDocument();
            string titleManifestName = $"{chapter}-title";
            string chapterTitle = null;
            if (files.Contains(titleManifestName)) 
            {
                chapterTitle = resourceContainer.GetFile(titleManifestName);
            }


            // Break the filename out to its components, get all that are valid numbered chunks and are in our chapter, and then order them by chunks
            // The format of the chunk names are "<chapter>-<chunk>"
            var sortedFiles = files.Select(c => c.Split('-'))
                .Where(c => c.Length == 2 && c[0] == chapter && int.TryParse(c[1], out int _))
                .OrderBy(c => int.Parse(c[1]))
                .Select(c => string.Join("-",c))
                .ToList();

            foreach(var item in sortedFiles)
            {
                string chunk = resourceContainer.GetFile(item);
                if (chunk != null)
                {
                    try
                    {
                        output.Insert(parser.ParseFromString(chunk));
                    }
                    catch(Exception e)
                    {
                        throw new Exception($"Error loading chunk {item}: {e.Message}");
                    }
                }
            }


            // Time to rewrite the world
            var currentContents = output.Contents;
            output = new USFMDocument();
            var currentChapter = currentContents.Count != 0 && currentContents[0] is CMarker
                ? currentContents[0] as CMarker
                : null;
            if (int.TryParse(chapter, out int chapterNumber) && currentChapter == null)
            {
                // Pull out the contents and put them into a new document with a new parent chapter
                currentChapter = new CMarker() { Number = chapterNumber };
                output.Insert(currentChapter);
            }
            else
            {
                if (currentChapter != null)
                {
                    output.Insert(currentChapter);
                    currentContents = currentChapter.Contents;
                    currentChapter.Contents = [];
                }
            }

            if (currentChapter != null)
            {
                if (chapterTitle != null)
                {
                    output.Insert(new CLMarker() { Label = chapterTitle });
                }
            }
            output.Insert(new PMarker());
            
            output.InsertMultiple(currentContents);

            return output;
        }
        public static BTTWriterManifest GetManifest(IResourceContainer resourceContainer)
        {
            return resourceContainer.GetManifest();
        }
    }
}
