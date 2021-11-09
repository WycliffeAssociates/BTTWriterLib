using BTTWriterLib;
using BTTWriterLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using USFMToolsSharp;
using USFMToolsSharp.Models.Markers;

namespace BTTWriterLibTests
{
    /// <summary>
    /// Tests for the BTTWriterLoader class
    /// </summary>
    [TestClass]
    public class BTTWriterLoaderTests
    {
        /// <summary>
        /// Verify the basic structure of the created USFM document
        /// </summary>
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
            Assert.AreEqual(manifest.project.name, ((HMarker)document.Contents[5]).HeaderText);
            Assert.AreEqual(manifest.project.name, ((MTMarker)document.Contents[6]).Title);
        }

        /// <summary>
        /// Verify that the translated name if present overrides the name in the manifest
        /// </summary>
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

        /// <summary>
        /// Tests that an untranslated chapter doesn't get a cl marker added
        /// </summary>
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

        /// <summary>
        /// Test that if there just so happens to be a CMarker in the source that an additional one doesn't get added
        /// </summary>
        [TestMethod]
        public void TestWithChapterMarkersInChunks()
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

            Assert.AreEqual(1, document.GetChildMarkers<CMarker>().Count);
        }

        /// <summary>
        /// Verify that if a chapter title has been translated that a cl marker gets added
        /// </summary>
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
                ["01-01"] = "\\v 1 First verse",
                ["01-title"] = "Translated"
            };
            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Contents[0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Contents[1]).Encoding);
            Assert.AreEqual("Translated", document.GetChildMarkers<CMarker>()[0].GetChildMarkers<CLMarker>()[0].Label);
        }

        /// <summary>
        /// Verify that chapters get ordered correctly
        /// </summary>
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
                ["02-01"] = "\\v 1 Second chapter First verse",
                ["01-01"] = "\\v 1 First verse",
                ["01-title"] = "Translated"
            };
            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Contents[0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Contents[1]).Encoding);
            var chapters = document.GetChildMarkers<CMarker>();
            Assert.AreEqual(1, chapters[0].Number);
            Assert.AreEqual(2, chapters[1].Number);
            Assert.AreEqual("Translated", chapters[0].GetChildMarkers<CLMarker>()[0].Label);
        }

        /// <summary>
        /// Verify that chunks get ordered correctly
        /// </summary>
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
                ["01-01"] = "\\v 1 First verse",
                ["01-title"] = "Translated"
            };
            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Contents[0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Contents[1]).Encoding);
            Assert.AreEqual("Translated", document.GetChildMarkers<CMarker>()[0].GetChildMarkers<CLMarker>()[0].Label);
        }

        /// <summary>
        /// Verify that if a chapter exists in a chunk then don't add an additional one
        /// </summary>
        [TestMethod]
        public void TestWithExistingChapter()
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
                ["01-01"] = "\\c 1 \\v 2 Second chapter First verse",
            };

            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual(1, document.GetChildMarkers<CMarker>().Count);
        }

        /// <summary>
        /// Verify that a blank chunk won't crash
        /// </summary>
        [TestMethod]
        public void TestWithBlankContent()
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
                ["01-01"] = "",
            };

            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);
        }

        /// <summary>
        /// Test manual configuration of USFMParser
        /// </summary>
        [TestMethod]
        public void TestWithManualParserConfiguration()
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
                ["01-01"] = "\\v 1 \\b",
            };

            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false, new USFMParser(new List<string>() { "b"}));
            Assert.AreEqual(0, document.GetChildMarkers<BMarker>().Count);
        }
    }
}
