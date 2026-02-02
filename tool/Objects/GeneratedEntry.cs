namespace Adrium.KeepassPfpConverter.Objects
{
	public class GeneratedEntry : PassEntry
	{
		public string type = PfpConvert.GENERATED_PFP_TYPE;
		public int length = 16;
		public bool lower = true;
		public bool upper = true;
		public bool number = true;
		public bool symbol = true;
	}
}
