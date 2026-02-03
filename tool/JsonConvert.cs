using System;
using Adrium.KeepassPfpConverter.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// https://github.com/JamesNK/Newtonsoft.Json/issues/1331
// https://www.tutorialpedia.org/blog/json-net-serialize-deserialize-derived-types/
// https://github.com/Azure/durabletask/blob/main/src/DurableTask.Core/Serializing/JsonCreationConverter.cs
namespace Adrium.KeepassPfpConverter
{
	public class JsonConvert
	{
		private const string TYPE_KEY = "type";

		private readonly JsonSerializerSettings settings;

		public JsonConvert() : this(new JsonSerializerSettings()) { }

		public JsonConvert(JsonSerializerSettings settings)
		{
			settings.NullValueHandling = NullValueHandling.Ignore;
			settings.Converters.Add(new EntryConverter());
			this.settings = settings;
		}

		public string Serialize(object o)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(o, settings);
		}

		public T Deserialize<T>(string json)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, settings);
		}

		private class EntryConverter : JsonConverter<BaseEntry>
		{
			public override bool CanWrite => false;

			public override BaseEntry ReadJson(JsonReader reader, Type objectType, BaseEntry existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				var obj = JObject.Load(reader);
				var type = obj.Value<string>(TYPE_KEY);

				BaseEntry result;
				if (type == null)
					result = new SiteEntry();
				else if (type == PfpConvert.STORED_TYPE)
					result = new StoredEntry();
				else
					result = new GeneratedEntry();

				serializer.Populate(obj.CreateReader(), result);
				return result;
			}

			public override void WriteJson(JsonWriter writer, BaseEntry value, JsonSerializer serializer)
			{
				throw new NotSupportedException();
			}
		}
	}
}
