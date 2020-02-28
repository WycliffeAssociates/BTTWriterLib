using BTTWriterLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BTTWriterLib
{
    public interface IResourceContainer
    {
        BTTWriterManifest GetManifest();
        List<string> GetFiles(bool onlyFinished);
        string GetFile(string fileName);
    }
}
