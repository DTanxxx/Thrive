using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class RandomConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Type type = typeof(Random);
        FieldInfo seedArrayInfo = type.GetField("_seedArray", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo inextInfo = type.GetField("_inext", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo inextpInfo = type.GetField("_inextp", BindingFlags.NonPublic | BindingFlags.Instance);

        if (seedArrayInfo != null && inextInfo != null && inextpInfo != null)
        {
            int[] seedArray = (int[])seedArrayInfo.GetValue((Random)value);
            int inext = (int)inextInfo.GetValue((Random)value);
            int inextp = (int)inextpInfo.GetValue((Random)value);

            writer.WriteStartObject();

            writer.WritePropertyName("seedArray");
            serializer.Serialize(writer, seedArray);

            writer.WritePropertyName("inext");
            serializer.Serialize(writer, inext);

            writer.WritePropertyName("inextp");
            serializer.Serialize(writer, inextp);

            writer.WriteEndObject();
        }
        else
        {
            throw new NullReferenceException("One or more fields did not exist in the type or did not match the " +
                "specified binding constraints");
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartObject)
        {
            return default(Random);
        }

        var item = JObject.Load(reader);

        try
        {
            var random = new Random();

            Type type = typeof(Random);
            FieldInfo seedArrayInfo = type.GetField("_seedArray", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo inextInfo = type.GetField("_inext", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo inextpInfo = type.GetField("_inextp", BindingFlags.NonPublic | BindingFlags.Instance);

            int[] seedArray = item["seedArray"].Value<int[]>() ?? new int[] { 0 };
            int? inext = item["inext"].Value<int>();
            int? inextp = item["inextp"].Value<int>();

            if (seedArray.Length == 1 || inext == null || inextp == null)
            {
                throw new NullReferenceException("Can't read Random (one or more properties are missing");
            }

            if (seedArrayInfo == null && inextInfo == null && inextpInfo == null)
            {
                throw new NullReferenceException("One or more fields did not exist in the type or did not match the " +
                "specified binding constraints");
            }

            seedArrayInfo.SetValue(random, seedArray);
            inextInfo.SetValue(random, inext.Value);
            inextpInfo.SetValue(random, inextp.Value);

            return random;
        }
        catch (NullReferenceException e)
        {
            throw new JsonException("Can't read Random (missing property)", e);
        }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Random);
    }
}
