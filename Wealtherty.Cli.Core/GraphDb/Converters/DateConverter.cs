using Newtonsoft.Json;

namespace Wealtherty.Cli.Core.GraphDb.Converters;

public class DateConverter : JsonConverter<DateTime?>
{
    public override void WriteJson(JsonWriter writer, DateTime? value, JsonSerializer serializer)
    {
        if (!value.HasValue) return;
        writer.WriteRaw("date(");
        writer.WriteValue(value.Value.ToString("yyyy-MM-dd"));
        writer.WriteRaw(")");
    }

    public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}