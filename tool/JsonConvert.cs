using System;
using Adrium.KeepassPfpConverter.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// https://github.com/JamesNK/Newtonsoft.Json/issues/1331
namespace Adrium.KeepassPfpConverter
{
	public class JsonConvert
	{
		private const string TYPE_KEY = "type";
		private const string STORED_TYPE = "stored";

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
			private bool block; // Prevent recursion
			public override bool CanRead => base.CanRead && (block = !block);
			public override bool CanWrite => base.CanWrite && (block = !block);

			public override BaseEntry ReadJson(JsonReader reader, Type objectType, BaseEntry existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				block = true;
				var obj = JObject.Load(reader);
				var str = obj.Value<string>(TYPE_KEY);

				var type = str == null ? typeof(SiteEntry) :
					STORED_TYPE.Equals(str) ? typeof(StoredEntry) :
					typeof(GeneratedEntry);

				var result = (BaseEntry)serializer.Deserialize(obj.CreateReader(), type);
				return result;
			}

			public override void WriteJson(JsonWriter writer, BaseEntry value, JsonSerializer serializer)
			{
				block = true;
				var obj = JObject.FromObject(value, serializer);
				if (value is StoredEntry)
					obj.AddFirst(new JProperty(TYPE_KEY, STORED_TYPE));
				obj.WriteTo(writer);
			}
		}
	}
}
