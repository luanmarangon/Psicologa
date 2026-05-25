using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Infra.CrossCutting.JSONConverter
{
    public class BooleanJSONConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string stringValue = reader.GetString();
                if (bool.TryParse(stringValue, out bool value))
                {
                    return value;
                }
            }
            else if (reader.TokenType == JsonTokenType.False || reader.TokenType == JsonTokenType.True)
            {
                return reader.GetBoolean();
            }

            throw new System.Text.Json.JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Boolean value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}
