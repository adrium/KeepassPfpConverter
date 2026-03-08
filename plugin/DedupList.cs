using Adrium.KeepassPfpConverter.Objects;
using System.Collections.Generic;

namespace Adrium.KeepassPfpConverter.Plugin
{
	public class DedupList
	{
		private readonly IDictionary<string, IDictionary<string, Tuple>> list =
			new Dictionary<string, IDictionary<string, Tuple>>();

		private class Tuple
		{
			public StoredEntry entry;
			public string title;
		}

		public void Deduplicate(StoredEntry entry, string title)
		{
			var t = new Tuple { entry = entry, title = title };
			var k1 = entry.site + ":" + entry.name;
			var k2 = entry.revision;

			if (!list.ContainsKey(k1)) {
				list.Add(k1, new Dictionary<string, Tuple> { { k2, t } });
				return;
			}

			var ts = list[k1];

			while (ts.ContainsKey(k2)) {
				var dup = ts[k2];

				if (dup.entry.revision.Contains(dup.title))
					{ }
				else
					dup.entry.revision =
						dup.entry.revision == "" ? dup.title : dup.entry.revision + " in " + dup.title;

				if (k2.Contains(title))
					k2 = "copy of " + k2;
				else
					k2 = k2 == "" ? t.title : t.entry.revision + " in " + t.title;
			}

			t.entry.revision = k2;
			ts.Add(k2, t);
		}
	}
}
