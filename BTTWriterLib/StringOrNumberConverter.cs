using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BTTWriterLib
{
    /// <summary>
    /// Reads a JSON string or number into a string property.
    /// </summary>
    /// <remarks>
    /// Older BTTWriter manifests store some values (such as checking_level) as numbers while newer ones store them as strings.
    /// Numbers are kept exactly as written in the JSON, so 3 becomes "3". Values are always written back out as strings.
    /// </remarks>
    internal sealed class StringOrNumberConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    return reader.HasValueSequence ? Encoding.UTF8.GetString(reader.ValueSequence) : Encoding.UTF8.GetString(reader.ValueSpan);
                default:
                    throw new JsonException($"Expected a string or number but found {reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
