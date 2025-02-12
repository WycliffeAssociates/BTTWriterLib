using System.Text.Json.Serialization;
using BTTWriterLib.Models;

namespace BTTWriterLib
{
    [JsonSerializable(typeof(BTTWriterManifest))]
    internal partial class JsonContext: JsonSerializerContext
    {
        
    }
}