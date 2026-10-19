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
            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Hierarchies[0][0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Hierarchies[0][1]).Encoding);
            Assert.AreEqual(manifest.project.name, ((TOC1Marker)document.Hierarchies[0][2]).LongTableOfContentsText);
            Assert.AreEqual(manifest.project.name, ((TOC2Marker)document.Hierarchies[0][3]).ShortTableOfContentsText);
            Assert.AreEqual(manifest.project.id, ((TOC3Marker)document.Hierarchies[0][4]).BookAbbreviation);
            Assert.AreEqual(manifest.project.name, ((HMarker)document.Hierarchies[0][5]).HeaderText);
            Assert.AreEqual(manifest.project.name, ((MTMarker)document.Hierarchies[0][6]).Title);
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

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Hierarchies[0][0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Hierarchies[0][1]).Encoding);
            Assert.AreEqual(translatedName, ((TOC1Marker)document.Hierarchies[0][2]).LongTableOfContentsText);
            Assert.AreEqual(translatedName, ((TOC2Marker)document.Hierarchies[0][3]).ShortTableOfContentsText);
            Assert.AreEqual(manifest.project.id, ((TOC3Marker)document.Hierarchies[0][4]).BookAbbreviation);
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

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Hierarchies[0][0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Hierarchies[0][1]).Encoding);
            Assert.AreEqual(0, document.Hierarchies[0].GetChildMarkers<CMarker>()[0].GetChildMarkers<CLMarker>().Count);
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

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Hierarchies[0][0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Hierarchies[0][1]).Encoding);
            Assert.AreEqual("Translated", document.Hierarchies[0].GetChildMarkers<CMarker>()[0].GetChildMarkers<CLMarker>()[0].As<CLMarker>().Label);
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

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Hierarchies[0][0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Hierarchies[0][1]).Encoding);
            var chapters = document.Hierarchies[0].GetChildMarkers<CMarker>();
            Assert.AreEqual(1, chapters[0].As<CMarker>().Number);
            Assert.AreEqual(2, chapters[1].As<CMarker>().Number);
            Assert.AreEqual("Translated", chapters[0].GetChildMarkers<CLMarker>()[0].As<CLMarker>().Label);
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

            Assert.AreEqual(manifest.project.id, ((IDMarker)document.Hierarchies[0][0]).TextIdentifier);
            Assert.AreEqual("UTF-8", ((IDEMarker)document.Hierarchies[0][1]).Encoding);
            Assert.AreEqual("Translated", document.Hierarchies[0].GetChildMarkers<CMarker>()[0].GetChildMarkers<CLMarker>()[0].As<CLMarker>().Label);
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
                ["01-01"] = @"\c 1 \v 2 Second chapter First verse",
            };

            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual(1, document.Hierarchies[0].GetChildMarkers<CMarker>().Count);
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
                ["01-01"] = @"\v 1 \b",
            };

            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false, new USFMParser(["b"]));
            Assert.AreEqual(0, document.Hierarchies[0].GetChildMarkers<BMarker>().Count);
        }

        [TestMethod]
        public void TestParagraphExists()
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
                ["01-01"] = @"\c 1 \v 1 Verse contents",
                ["02-01"] = @"\v 1 Second Chapter contents",
                ["03-01"] = @"\c 3 \v 1 Third Chapter contents",
            };

            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);
            var chapters = document.Hierarchies[0].GetChildMarkers<CMarker>();
            var firstChapter = chapters[0];
            var secondChapter = chapters[1];
            var thirdChapter = chapters[2];
            Assert.IsTrue(firstChapter.Contents[0].Marker is PMarker);
            Assert.IsTrue(secondChapter.Contents[0].Marker is PMarker);
            Assert.IsTrue(thirdChapter.Contents[0].Marker is PMarker);
        }

        /// <summary>
        /// Verify that a chapter split across multiple chunks only gets a single chapter, label, and paragraph inserted
        /// </summary>
        [TestMethod]
        public void TestMultipleChunksInChapter()
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
                ["01-title"] = "Translated",
                ["01-01"] = @"\c 1 \v 1 First verse",
                ["01-02"] = @"\v 2 Second verse",
                ["01-03"] = @"\v 3 Third verse",
            };

            IResourceContainer container = new TestResourceContainer(manifest,content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);
            var chapters = document.Hierarchies[0].GetChildMarkers<CMarker>();
            Assert.AreEqual(1, chapters.Count);
            Assert.AreEqual(1, chapters[0].GetChildMarkers<CLMarker>().Count);
            Assert.AreEqual(1, chapters[0].GetChildMarkers<PMarker>().Count);
            Assert.AreEqual(3, chapters[0].GetChildMarkers<VMarker>().Count);
        }

        /// <summary>
        /// Verify that book identifiers are converted to uppercase in USFM markers
        /// </summary>
        [TestMethod]
        public void TestBookIdentifierIsUppercase()
        {
            var manifest = new BTTWriterManifest()
            {
                project = new IdNameCombo()
                {
                    id = "exo",
                    name = "Exodus"
                }
            };
            var content = new Dictionary<string, string>();
            IResourceContainer container = new TestResourceContainer(manifest, content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual("EXO", ((IDMarker)document.Hierarchies[0][0]).TextIdentifier);
            Assert.AreEqual("EXO", ((TOC3Marker)document.Hierarchies[0][4]).BookAbbreviation);
        }

        /// <summary>
        /// Verify that mixed case book identifiers are converted to uppercase in USFM markers
        /// </summary>
        [TestMethod]
        public void TestMixedCaseBookIdentifierIsUppercase()
        {
            var manifest = new BTTWriterManifest()
            {
                project = new IdNameCombo()
                {
                    id = "Exo",
                    name = "Exodus"
                }
            };
            var content = new Dictionary<string, string>();
            IResourceContainer container = new TestResourceContainer(manifest, content, false);
            var document = BTTWriterLoader.CreateUSFMDocumentFromContainer(container, false);

            Assert.AreEqual("EXO", document.Hierarchies[0][0].As<IDMarker>().TextIdentifier);
            Assert.AreEqual("EXO", document.Hierarchies[0][4].As<TOC3Marker>().BookAbbreviation);
        }
    }
}
