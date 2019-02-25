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
	}
}
