using System;
using System.Collections.Generic;
using System.Text;

namespace BTTWriterLib.Models
{

    public class BTTWriterManifest
    {
        public int package_version { get; set; }
        public string format { get; set; }
        public Generator generator { get; set; }
        public TargetLanguage target_language { get; set; }
        /// <summary>
        /// Unix Epoch timestamp (int)
        /// </summary>
        public Int64 timestamp { get; set; }
        public IdNameCombo project { get; set; }
        public IdNameCombo type { get; set; }
        public IdNameCombo resource { get; set; }
        public SourceTranslations[] source_translations { get; set; }
        public TargetTranslation[] target_translations { get; set; }
        public string[] translators { get; set; }
        public string[] finished_chunks { get; set; }
    }
}
