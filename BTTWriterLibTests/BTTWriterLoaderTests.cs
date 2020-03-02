using BTTWriterLib;
using BTTWriterLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using USFMToolsSharp.Models.Markers;

namespace BTTWriterLibTests
{
    [TestClass]
    public class BTTWriterLoaderTests
    {
        [TestMethod]
        public void BlankRender()
        {
            var manifest = new BTTWriterManifest()
            {
                project = new IdNameCombo()
                {
                    id = "EXO",
                    name = "Exodus"
                }
            };
            var content = new Dictionary<string, string>();
            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);
            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Contents[0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Contents[1]).Encoding);
            Assert.AreEqual(manifest.project.name, ((TOC1Marker)document.Contents[2]).LongTableOfContentsText);
            Assert.AreEqual(manifest.project.name, ((TOC2Marker)document.Contents[3]).ShortTableOfContentsText);
            Assert.AreEqual(manifest.project.id, ((TOC3Marker)document.Contents[4]).BookAbbreviation);
        }

        [TestMethod]
        public void TestWithTranslatedName()
        {
            string translatedName = "Translated";
            var manifest = new BTTWriterManifest()
            {
                project = new IdNameCombo()
                {
                    id = "EXO",
                    name = "Exodus"
                }
            };
            var content = new Dictionary<string, string>() { ["front-title"] = translatedName};
            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Contents[0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Contents[1]).Encoding);
            Assert.AreEqual(translatedName, ((TOC1Marker)document.Contents[2]).LongTableOfContentsText);
            Assert.AreEqual(translatedName, ((TOC2Marker)document.Contents[3]).ShortTableOfContentsText);
            Assert.AreEqual(manifest.project.id, ((TOC3Marker)document.Contents[4]).BookAbbreviation);
        }

        [TestMethod]
        public void TestWithUntranslatedChapterTitle()
        {
            var manifest = new BTTWriterManifest()
            {
                project = new IdNameCombo()
                {
                    id = "EXO",
                    name = "Exodus"
                }
            };
            var content = new Dictionary<string, string>() { ["01-01"] = "\\c 1 \\v 1 First verse"};
            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Contents[0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Contents[1]).Encoding);
            Assert.AreEqual(0, document.GetChildMarkers<CMarker>()[0].GetChildMarkers<CLMarker>().Count);
        }

        [TestMethod]
        public void TestWithTranslatedChapterTitle()
        {
            var manifest = new BTTWriterManifest()
            {
                project = new IdNameCombo()
                {
                    id = "EXO",
                    name = "Exodus"
                }
            };
            var content = new Dictionary<string, string>() { 
                ["01-01"] = "\\c 1 \\v 1 First verse",
                ["01-title"] = "Translated"
            };
            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Contents[0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Contents[1]).Encoding);
            Assert.AreEqual("Translated", document.GetChildMarkers<CMarker>()[0].GetChildMarkers<CLMarker>()[0].Label);
        }

        [TestMethod]
        public void TestWithOutOfOrderChapter()
        {
            var manifest = new BTTWriterManifest()
            {
                project = new IdNameCombo()
                {
                    id = "EXO",
                    name = "Exodus"
                }
            };
            var content = new Dictionary<string, string>() { 
                ["02-01"] = "\\c 2 \\v 1 Second chapter First verse",
                ["01-01"] = "\\c 1 \\v 1 First verse",
                ["01-title"] = "Translated"
            };
            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Contents[0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Contents[1]).Encoding);
            Assert.AreEqual("Translated", document.GetChildMarkers<CMarker>()[0].GetChildMarkers<CLMarker>()[0].Label);
        }

        [TestMethod]
        public void TestWithOutOfOrderChunk()
        {
            var manifest = new BTTWriterManifest()
            {
                project = new IdNameCombo()
                {
                    id = "EXO",
                    name = "Exodus"
                }
            };
            var content = new Dictionary<string, string>() { 
                ["01-02"] = "\\v 2 Second chapter First verse",
                ["01-01"] = "\\c 1 \\v 1 First verse",
                ["01-title"] = "Translated"
            };
            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Contents[0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Contents[1]).Encoding);
            Assert.AreEqual("Translated", document.GetChildMarkers<CMarker>()[0].GetChildMarkers<CLMarker>()[0].Label);
        }
    }
}
