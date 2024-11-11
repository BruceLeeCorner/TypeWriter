using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TypeWriter
{
    public class ColorJsonConverter : JsonConverter<System.Windows.Media.Color>
    {
        public override System.Windows.Media.Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                .ConvertFromString(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, System.Windows.Media.Color value, JsonSerializerOptions options)
        {
            writer.WriteRawValue($"\"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}\"");
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(System.Windows.Media.Color);
        }
    }

    public class ColorJsonConverter2 : Newtonsoft.Json.JsonConverter<Color>
    {
        public override Color ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return (System.Windows.Media.Color)System.Windows.Media.ColorConverter
              .ConvertFromString(reader.Value as string);
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Color value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteRawValue($"\"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}\"");
        }
    }

}
