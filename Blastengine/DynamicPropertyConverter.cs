using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blastengine
{
    public class DynamicPropertyConverter<T> : JsonConverter<T>
    {
        private readonly HashSet<string> _propertiesToSerialize;

        public DynamicPropertyConverter(IEnumerable<string> propertiesToSerialize)
        {
            _propertiesToSerialize = new HashSet<string>(propertiesToSerialize);
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Deserialization is not implemented.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // 指定されたプロパティのみをシリアライズ
            foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (_propertiesToSerialize.Contains(property.Name))
                {
                    var jsonPropertyNameAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
                    var propertyName = jsonPropertyNameAttribute != null ? jsonPropertyNameAttribute.Name : property.Name;

                    var propertyValue = property.GetValue(value);
                    writer.WritePropertyName(propertyName);
                    JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);
                }
            }

            writer.WriteEndObject();
        }
    }
}