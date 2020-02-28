using BTTWriterLib.Models;
using System;
using System.IO;
using USFMToolsSharp.Models.Markers;
using Newtonsoft.Json;
using USFMToolsSharp;
using System.Collections.Generic;
using System.Linq;

namespace BTTWriterLib
{
    public class BTTWriterLoader
    {
        public static USFMDocument CreateUSFMDocumentFromContainer(IResourceContainer resourceContainer, bool onlyComplete)
        {
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
            foreach (var item in files.Select(e => e.Split('-')[0]).Distinct())
            {
                if (item == "front")
                {
                    continue;
                }

                document.Insert(LoadChapter(resourceContainer, files, item));
            }

            return document;
        }

        private static USFMDocument LoadChapter(IResourceContainer resourceContainer, List<string> files, string chapter)
        {
            USFMParser parser = new USFMParser(new List<string> { "s5", "fqa*" });
            USFMDocument output = new USFMDocument();
            string titleManifestName = $"{chapter}-title";
            string chapterTitle = null;
            if (files.Contains(titleManifestName)) 
            {
                chapterTitle = resourceContainer.GetFile(titleManifestName);
            }

            foreach(var item in files.Where(c => c.StartsWith($"{chapter}-")).OrderBy(c => c))
            {
                if (item == titleManifestName)
                {
                    continue;
                }
                string chunk = resourceContainer.GetFile(item);
                if (chunk != null)
                {
                    output.Insert(parser.ParseFromString(chunk));
                }
            }
            
            if (chapterTitle != null)
            {
                var chapters = output.GetChildMarkers<CMarker>();
                if (chapters.Count == 1)
                {
                    chapters[0].TryInsert(new CLMarker() { Label = chapterTitle });
                }
            }

            return output;
        }
    }
}
