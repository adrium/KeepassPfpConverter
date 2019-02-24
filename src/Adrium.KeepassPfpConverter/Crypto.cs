using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Text;

namespace Adrium.KeepassPfpConverter
{
	public class Crypto
	{
		const int N = 32768;
		const int r = 8;
		const int p = 1;

		const int AES_KEY_SIZE = 256;
		const int TAG_LENTH = 128;

		private KeyParameter key;

		public Crypto(string masterPassword, string salt)
		{
			var passbytes = Encoding.UTF8.GetBytes(masterPassword);
			var saltbytespre = Convert.FromBase64String(salt);
			var saltbytes = DoubleEncodeBytes(saltbytespre); // WTF

			key = new KeyParameter(Hash(passbytes, saltbytes, AES_KEY_SIZE / 8));
		}

		public string Decrypt(string data)
		{
			var dataarray = data.Split('_');
			var iv  = Convert.FromBase64String(dataarray[0]);
			var input = Convert.FromBase64String(dataarray[1]);

			var cipher = new GcmBlockCipher(new AesEngine());
			var parameters = new AeadParameters(key, TAG_LENTH, iv);
			cipher.Init(false, parameters);

			var result = Encoding.UTF8.GetString(Process(cipher, input));

			return result;
		}

		public static byte[] Hash(byte[] P, byte[] S, int length)
		{
			return SCrypt.Generate(P, S, N, r, p, length);
		}

		private byte[] Process(IAeadBlockCipher cipher, byte[] input)
		{
			var result = new byte[cipher.GetOutputSize(input.Length)];

			var count = cipher.ProcessBytes(input, 0, input.Length, result, 0);
			cipher.DoFinal(result, count);

			return result;
		}

		private byte[] DoubleEncodeBytes(byte[] input)
		{
			var chars = new char[input.Length];

			for (var i = 0; i < input.Length; i++) {
				chars[i] = Convert.ToChar(input[i]);
			}

			var result = Encoding.UTF8.GetBytes(chars);
			return result;
		}
	}
}
