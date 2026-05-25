using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Infra.CrossCutting.JSONConverter
{
    public class Int32JSONConverter : JsonConverter<Int32>
    {
        public override Int32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Int32 value = 0;
            if (reader.TokenType == JsonTokenType.String)
            {
                string stringValue = reader.GetString();
                int.TryParse(stringValue, out value);
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                value = reader.GetInt32();
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, Int32 value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
