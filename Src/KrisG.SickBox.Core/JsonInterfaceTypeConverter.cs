using System;
using Newtonsoft.Json;

namespace KrisG.SickBox.Core
{
    public class JsonInterfaceTypeConverter<TInterface, TConcrete> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<TConcrete>(reader);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TInterface);
        }
    }
}