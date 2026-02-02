using Adrium.KeepassPfpConverter.Plugin;
using KeePass.Plugins;

namespace PfpConverter
{
	public class PfpConverterExt : Plugin
	{
		public override string UpdateUrl => "https://raw.githubusercontent.com/adrium/KeepassPfpConverter/master/update.txt";

		public override bool Initialize(IPluginHost host)
		{
			host.FileFormatPool.Add(new FormatProvider());
			return true;
		}

	}
}
