using System;
using Adrium.KeepassPfpConverter.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Adrium.KeepassPfpConverter
{
	public static partial class PfpConvert
	{
		private class JsonConverter : JsonConverter<BaseEntry>
		{
			public override BaseEntry ReadJson(JsonReader reader, Type objectType, BaseEntry existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				if (reader.TokenType != JsonToken.StartObject)
					throw new InvalidOperationException();

				var obj = JObject.Load(reader);
				var type = obj.Value<string>("type");

				BaseEntry result;

				if (type == null)
					result = obj.ToObject<SiteEntry>();

				else if (type.Equals("stored"))
					result = obj.ToObject<StoredEntry>();

				else
					result = obj.ToObject<GeneratedEntry>();

				return result;
			}

			public override void WriteJson(JsonWriter writer, BaseEntry value, JsonSerializer serializer)
			{
				throw new InvalidOperationException();
			}
		}
	}
}
