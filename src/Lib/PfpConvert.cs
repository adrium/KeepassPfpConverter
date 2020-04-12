﻿using System;
using System.Collections.Generic;
using System.IO;
using Adrium.KeepassPfpConverter.Objects;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;

namespace Adrium.KeepassPfpConverter
{
	public static partial class PfpConvert
	{
		private const string APPLICATION = "pfp";
		private const int FORMAT = 2;

		private const string SALT_KEY = "salt";
		private const string HMAC_KEY = "hmac-secret";
		private const string STORAGE_PREFIX = "site:";

		private const string MAC_MESSAGE = "mac check in GCM failed";

		public static IList<BaseEntry> Load(Crypto crypto, Stream stream)
		{
			var reader = new StreamReader(stream);
			var str = reader.ReadToEnd();
			var backup = JsonConvert.DeserializeObject<DeserializedBackup>(str);

			if (!backup.application.Equals(APPLICATION))
				throw new ReaderException("Unsupported format");

			if (backup.format != FORMAT)
				throw new ReaderException("Unsupported version");

			var result = new List<BaseEntry>();

			crypto.SetSalt(backup.data[SALT_KEY]);

			foreach (var item in backup.data) {
				if (item.Key.Equals(SALT_KEY)) {
					continue; // ignore
				}

				var json = "";

				try {
					json = crypto.Decrypt(item.Value);
				} catch (InvalidCipherTextException e) {
					if (e.Message.Equals(MAC_MESSAGE))
						throw new ReaderException("Wrong master password");
					else
						throw;
				}

				if (item.Key.Equals(HMAC_KEY)) {
					json = JsonConvert.DeserializeObject<string>(json);
					crypto.SetHmacSecret(json);
					continue;
				}

				var entry = DeserializeObjectContainingEntries<BaseEntry>(json);
				result.Add(entry);
			}

			return result;

		}

		public static void Save(Crypto crypto, Stream stream, IList<BaseEntry> entries)
		{
			var backup = new DeserializedBackup();

			entries = GenerateSiteEntries(entries);

			if (crypto.GetSalt() == null)
				crypto.GenerateSalt();
			if (crypto.GetHmacSecret() == null)
				crypto.GenerateHmacSecret();

			backup.application = APPLICATION;
			backup.format = FORMAT;
			backup.data = new Dictionary<string, string>();

			string json;
			var jsonsettings = new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore,
			};

			foreach (var baseentry in entries) {
				json = JsonConvert.SerializeObject(baseentry, jsonsettings);
				json = crypto.Encrypt(json);

				var key = STORAGE_PREFIX;

				if (baseentry is SiteEntry site) {
					key += crypto.Digest(site.site);
				}

				if (baseentry is PassEntry pass) {
					if (pass.type == null)
						throw new ArgumentException("No type set");

					key += crypto.Digest(pass.site) + ":";
					key += crypto.Digest(pass.site + "\0" + pass.name + "\0" + pass.revision);
				}

				backup.data.Add(key, json);
			}

			json = crypto.GetHmacSecret();
			json = JsonConvert.SerializeObject(json);
			json = crypto.Encrypt(json);
			backup.data.Add(HMAC_KEY, json);

			backup.data.Add(SALT_KEY, crypto.GetSalt());

			var writer = new StreamWriter(stream);
			var str = JsonConvert.SerializeObject(backup);
			writer.Write(str);
			writer.Flush();
		}

		public static T DeserializeObjectContainingEntries<T>(string json)
		{
			var converter = new JsonConverter();
			var result = JsonConvert.DeserializeObject<T>(json, converter);
			return result;
		}

		public static IList<BaseEntry> GenerateSiteEntries(IList<BaseEntry> entries)
		{
			var result = new List<BaseEntry>();
			var need = new Dictionary<string, string>();
			var have = new Dictionary<string, string>();

			foreach (var baseentry in entries) {
				result.Add(baseentry);
				if (baseentry is SiteEntry site)
					have[site.site] = "ok";
				if (baseentry is PassEntry pass)
					need[pass.site] = "ok";
			}

			foreach (var item in have)
				need.Remove(item.Key);

			foreach (var item in need) {
				var entry = new SiteEntry();
				entry.site = item.Key;
				result.Add(entry);
			}

			return result;
		}

		public class ReaderException : Exception
		{
			public ReaderException(string message) : base(message) { }
		}

		private class DeserializedBackup
		{
			#pragma warning disable 649
			public string application;
			public int format;
			public IDictionary<string, string> data;
			#pragma warning restore 649
		}
	}
}