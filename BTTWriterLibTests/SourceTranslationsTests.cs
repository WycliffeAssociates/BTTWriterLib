using BTTWriterLib;
using BTTWriterLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.Json;

namespace BTTWriterLibTests
{
    /// <summary>
    /// Tests for reading source translations out of a manifest
    /// </summary>
    [TestClass]
    public class SourceTranslationsTests
    {
        /// <summary>
        /// Write a manifest with the given source_translations JSON to a temp directory and load it through a FileSystemResourceContainer
        /// </summary>
        private static BTTWriterManifest LoadManifest(string sourceTranslationsJson)
        {
            var directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(directory);
            try
            {
                File.WriteAllText(Path.Combine(directory, "manifest.json"),
                    "{\"package_version\":8,\"project\":{\"id\":\"gen\",\"name\":\"Genesis\"},\"source_translations\":" + sourceTranslationsJson + "}");
                return new FileSystemResourceContainer(directory).GetManifest();
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }

        /// <summary>
        /// Verify checking_level and version load whether they were saved as strings or numbers
        /// </summary>
        [DataTestMethod]
        [DataRow("\"3\"", "\"1\"", "3", "1", DisplayName = "Strings")]
        [DataRow("3", "1", "3", "1", DisplayName = "Integers")]
        [DataRow("3", "\"2.0.0-beta13\"", "3", "2.0.0-beta13", DisplayName = "Mixed")]
        [DataRow("3.0", "12.10", "3.0", "12.10", DisplayName = "Decimals keep their original text")]
        [DataRow("null", "null", null, null, DisplayName = "Nulls")]
        public void LoadsCheckingLevelAndVersion(string checkingLevelJson, string versionJson, string expectedCheckingLevel, string expectedVersion)
        {
            var manifest = LoadManifest($"[{{\"language_id\":\"en\",\"resource_id\":\"ulb\",\"checking_level\":{checkingLevelJson},\"version\":{versionJson}}}]");

            Assert.AreEqual(1, manifest.source_translations.Length);
            Assert.AreEqual("en", manifest.source_translations[0].language_id);
            Assert.AreEqual("ulb", manifest.source_translations[0].resource_id);
            Assert.AreEqual(expectedCheckingLevel, manifest.source_translations[0].checking_level);
            Assert.AreEqual(expectedVersion, manifest.source_translations[0].version);
        }

        /// <summary>
        /// Verify missing checking_level and version are left as null
        /// </summary>
        [TestMethod]
        public void MissingCheckingLevelAndVersionAreNull()
        {
            var manifest = LoadManifest("[{\"language_id\":\"en\",\"resource_id\":\"ulb\"}]");

            Assert.IsNull(manifest.source_translations[0].checking_level);
            Assert.IsNull(manifest.source_translations[0].version);
        }

        /// <summary>
        /// Verify a value that is neither a string nor a number is still rejected
        /// </summary>
        [DataTestMethod]
        [DataRow("true")]
        [DataRow("{}")]
        [DataRow("[3]")]
        public void RejectsNonStringNonNumberCheckingLevel(string checkingLevelJson)
        {
            Assert.ThrowsException<JsonException>(() =>
                LoadManifest($"[{{\"language_id\":\"en\",\"resource_id\":\"ulb\",\"checking_level\":{checkingLevelJson}}}]"));
        }

        /// <summary>
        /// Verify values read from numbers are written back out as strings
        /// </summary>
        [TestMethod]
        public void WritesValuesAsStrings()
        {
            var sourceTranslation = JsonSerializer.Deserialize<SourceTranslations>("{\"checking_level\":3,\"version\":1}");

            var json = JsonSerializer.Serialize(sourceTranslation);

            StringAssert.Contains(json, "\"checking_level\":\"3\"");
            StringAssert.Contains(json, "\"version\":\"1\"");
        }
    }
}
