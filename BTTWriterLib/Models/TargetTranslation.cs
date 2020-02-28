using System;
using System.Collections.Generic;
using System.Text;

namespace BTTWriterLib.Models
{
    public class TargetTranslation
    {
        public string path { get; set; }
        public string id { get; set; }
        public CommitHash commit_hash { get; set; }
        public string direction { get; set; }
    }

}
