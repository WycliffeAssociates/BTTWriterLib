using BTTWriterLib;
using BTTWriterLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTTWriterLibTests
{
    public class TestResourceContainer : IResourceContainer
    {
        private BTTWriterManifest manifest;
        private Dictionary<string, string> content;
        private bool isFinished;

        public TestResourceContainer(BTTWriterManifest manifest, Dictionary<string, string> content, bool isFinished)
        {
            this.manifest = manifest;
            this.content = content;
            this.isFinished = isFinished;
        }
        public string GetFile(string fileName)
        {
            if (content.ContainsKey(fileName))
            {
                return content[fileName];
            }
            return null;
        }

        public List<string> GetFiles(bool onlyFinished)
        {
            return content.Keys.ToList();
        }

        public BTTWriterManifest GetManifest()
        {
            return manifest;
        }
    }
}
