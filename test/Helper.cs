using System.Collections.Generic;
using System.Linq;

namespace Adrium.KeepassPfpConverter.Test
{
	public static class Helper
	{
		public static IDictionary<string, object> ToDict(this object source)
		{
			return source.GetType().GetFields().ToDictionary(p => p.Name, p => p.GetValue(source));
		}
	}
}
