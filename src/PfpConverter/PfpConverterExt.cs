using Adrium.KeepassPfpConverter;
using KeePass.Plugins;

namespace PfpConverter
{
	public class PfpConverterExt : Plugin
	{
		public override bool Initialize(IPluginHost host)
		{
			host.FileFormatPool.Add(new PfpFormatProvider());
			return true;
		}

		public override string UpdateUrl {
			get { return "https://raw.githack.com/adrium/KeepassPfpConverter/master/plgx/version"; }
		}
	}
}
