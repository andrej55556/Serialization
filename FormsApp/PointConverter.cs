using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PointLibFr;
using System;

namespace FormsApp
{
    public class PointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Point).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            if (jsonObject["Z"] != null)
            {
                return jsonObject.ToObject<Point3D>(serializer);
            }
            return jsonObject.ToObject<Point>(serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}