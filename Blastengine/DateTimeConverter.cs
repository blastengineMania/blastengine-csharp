using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blastengine
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && DateTime.TryParse(reader.GetString(), out DateTime date))
            {
                return date;
            }

            throw new JsonException("Invalid date format. Expected a valid date string.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ssK")); // ISO 8601形式でシリアル化
        }
    }
}

