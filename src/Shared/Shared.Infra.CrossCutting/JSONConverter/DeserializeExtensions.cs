using System.Text.Json;

namespace Shared.Infra.CrossCutting.JSONConverter
{
    public static class DeserializeExtensions
    {
        public static T Deserialize<T>(this System.Text.Json.JsonElement jsonElement)
        {
            JsonSerializerOptions defaultSerializerSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            defaultSerializerSettings.Converters.Add(new BooleanJSONConverter());

            return JsonSerializer.Deserialize<T>(jsonElement.GetRawText(), defaultSerializerSettings);
        }

    }
}
