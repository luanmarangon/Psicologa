using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Infra.CrossCutting.JSONConverter
{
    public class EncryptIdJSONConverter : JsonConverter<Int32>
    {

        public override Int32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Int32 value = 0;

            if (reader.TokenType == JsonTokenType.String)
            {
                string valueStr = reader.GetString();
                if (!Int32.TryParse(valueStr, out value))
                {
                    if (valueStr != "0")
                    {
                        Int32.TryParse(Criptografia.Descriptografar(valueStr), out value);
                    }
                }

            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.HasValueSequence)
                    value = reader.GetInt32();
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, Int32 value, JsonSerializerOptions options)
        {
            if (writer == null)
                writer.WriteStringValue("");
            else
            {
                writer.WriteStringValue(Criptografia.Criptografar(value.ToString()));
            }
        }

    }
}
