using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Adrium.KeepassPfpConverter.Objects;

namespace Adrium.KeepassPfpConverter
{
	internal static class Util
	{
		private static RandomNumberGenerator rng = RandomNumberGenerator.Create();

		public static string GenerateRandom(int length)
		{
			var gen = new byte[length];
			rng.GetBytes(gen);

			var result = Convert.ToBase64String(gen);
			return result;
		}

		public static IList<BaseEntry> GenerateSiteEntries(IList<BaseEntry> entries)
		{
			var result = new List<BaseEntry>();
			var need = new Dictionary<string, int>();
			var have = new Dictionary<string, int>();

			foreach (var baseentry in entries) {
				result.Add(baseentry);
				if (baseentry is SiteEntry site)
					have[site.site] = 1;
				if (baseentry is PassEntry pass)
					need[pass.site] = 1;
			}

			foreach (var item in have)
				need.Remove(item.Key);

			foreach (var item in need) {
				var entry = new SiteEntry { site = item.Key };
				result.Insert(0, entry);
			}

			return result;
		}
	}
}
