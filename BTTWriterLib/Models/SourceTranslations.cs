using System;
using System.Text.Json.Serialization;

namespace BTTWriterLib.Models
{
    public class SourceTranslations
    {
        public string language_id { get; set; }
        public string resource_id { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string checking_level { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string version { get; set; }
    }
}
