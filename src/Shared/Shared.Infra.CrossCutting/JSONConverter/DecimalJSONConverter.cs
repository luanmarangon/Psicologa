using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Infra.CrossCutting.JSONConverter
{
    public class DecimalJSONConverter : JsonConverter<Decimal>
    {
        public override Decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Decimal value = 0;

            if (reader.TokenType == JsonTokenType.String)
            {
                string valueStr = reader.GetString();
                if (!Decimal.TryParse(valueStr, NumberStyles.Any, new System.Globalization.CultureInfo("pt-br", false), out value))
                {
                    if (valueStr != "0")
                    {
                        Decimal.TryParse(valueStr, NumberStyles.Any, new System.Globalization.CultureInfo("pt-br", false), out value);
                    }
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
               value = reader.GetDecimal();
            }

            return value;
        }

      

        public override void Write(Utf8JsonWriter writer, Decimal value, JsonSerializerOptions options)
        {
            if (writer == null)
                writer.WriteStringValue("");
            else writer.WriteStringValue(value.ToString("N2", new System.Globalization.CultureInfo("pt-br", false)));
        }
    }
}
