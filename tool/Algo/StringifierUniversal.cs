using System;
using System.Collections.Generic;
using System.Text;

namespace Adrium.KeepassPfpConverter.Algo
{
	public class StringifierUniversal
	{
		private readonly IEnumerable<Settings> charsettings;

		public StringifierUniversal(IEnumerable<Settings> charsettings)
		{
			this.charsettings = charsettings;
		}

		public string Stringify(string bytes)
		{
			var array = Convert.FromBase64String(bytes);
			var resultsb = new StringBuilder(array.Length);

			foreach (var s in charsettings)
				s.Count = 0;

			foreach (var value in array) {
				var sum = 0;
				var max = 0;
				var cnt = 0;

				foreach (var s in charsettings) {
					cnt = Math.Max(0, s.Min - s.Count);
					max = Math.Max(max, cnt);
					sum = sum + cnt;
				}

				cnt = 0;
				foreach (var s in charsettings) {
					s.Enabled = s.Count < s.Max && (sum < array.Length - resultsb.Length || s.Min - s.Count == max);
					cnt += s.Enabled ? s.Charset.Length : 0;
				}

				var index = cnt > 0 ? value % cnt : 0;
				foreach (var s in charsettings) {
					if (s.Enabled) {
						if (index < s.Charset.Length) {
							resultsb.Append(s.Charset[index]);
							s.Count++;
							break;
						}
						index -= s.Charset.Length;
					}
				}
			}

			return resultsb.ToString();
		}

		public class Settings
		{
			public readonly string Charset = "";
			public readonly int Min = 0;
			public readonly int Max = int.MaxValue;
			internal int Count;
			internal bool Enabled;
			public Settings(string charset) { Charset = charset; }
			public Settings(string charset, int min) { Charset = charset; Min = min; }
			public Settings(string charset, int min, int max) { Charset = charset; Min = min; Max = max; }
		}
	}
}
