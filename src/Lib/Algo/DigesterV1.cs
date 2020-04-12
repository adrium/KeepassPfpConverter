using System;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace Adrium.KeepassPfpConverter.Algo
{
	public class DigesterV1
	{
		private readonly byte[] hmacBytes;

		public DigesterV1(string hmac)
		{
			hmacBytes = Convert.FromBase64String(hmac);
		}

		public string Digest(string data)
		{
			var bytes = Encoding.UTF8.GetBytes(data);

			var hmac = new HMac(new Sha256Digest());

			hmac.Init(new KeyParameter(hmacBytes));
			var digest = new byte[hmac.GetMacSize()];

			hmac.BlockUpdate(bytes, 0, bytes.Length);
			hmac.DoFinal(digest, 0);

			var result = Convert.ToBase64String(digest);

			return result;
		}
	}
}
