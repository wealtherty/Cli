using Humanizer;
using Newtonsoft.Json;

namespace Wealtherty.Cli.CompaniesHouse.Converters;

public class TitleConverter : JsonConverter<string> 
{
    public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToLower().Transform(To.TitleCase));
    }

    public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}