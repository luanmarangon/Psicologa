using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Infra.CrossCutting.JSONConverter
{
    public class DateTimeJSONConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            DateTime value = DateTime.MinValue;

            if (reader.TokenType == JsonTokenType.String)
            {
                string valueStr = reader.GetString();
                if (!DateTime.TryParse(valueStr, new System.Globalization.CultureInfo("pt-br", false), System.Globalization.DateTimeStyles.None, out value))
                {
                    if (valueStr != "0")
                    {
                        DateTime.TryParse(valueStr, new System.Globalization.CultureInfo("pt-br", false), System.Globalization.DateTimeStyles.None, out value);
                    }
                }
            }
            else 
            {
                if (reader.HasValueSequence)
                    value = reader.GetDateTime();
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (writer == null || value == DateTime.MinValue)
                writer.WriteStringValue("");
            else
            {
                if (value.Hour == 0 && value.Minute == 0 && value.Second == 0 )
                    writer.WriteStringValue(value.ToString("dd/MM/yyyy")); 
                else writer.WriteStringValue(value.ToString("dd/MM/yyyy HH:mm:ss"));
            }
        }
    }
}
