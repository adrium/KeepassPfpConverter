using System;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Adrium.KeepassPfpConverter.Algo
{
	public class CipherV1
	{
		private const int AES_KEY_SIZE = 256;
		private const int TAG_LENTH = 128;

		private readonly byte[] keyBytes;

		public CipherV1(Hash hash, string salt)
		{
			var saltBytes = Convert.FromBase64String(salt);
			var key = hash(GetArrayAsString(saltBytes), AES_KEY_SIZE / 8);
			keyBytes = Convert.FromBase64String(key);
		}

		public string Process(bool encrypt, string ivstr, string datastr)
		{
			return encrypt ? Encrypt(ivstr, datastr) : Decrypt(ivstr, datastr);
		}

		private string Decrypt(string ivstr, string datastr)
		{
			var iv = Convert.FromBase64String(ivstr);
			var data = Convert.FromBase64String(datastr);
			var result = Encoding.UTF8.GetString(Process(false, iv, data));
			return result;
		}

		private string Encrypt(string ivstr, string datastr)
		{
			var iv = Convert.FromBase64String(ivstr);
			var data = Encoding.UTF8.GetBytes(datastr);
			var result = Convert.ToBase64String(Process(true, iv, data));
			return result;
		}

		private byte[] Process(bool encrypt, byte[] iv, byte[] input)
		{
			var cipher = new GcmBlockCipher(new AesEngine());
			var parameters = new AeadParameters(new KeyParameter(keyBytes), TAG_LENTH, iv);

			cipher.Init(encrypt, parameters);
			var result = new byte[cipher.GetOutputSize(input.Length)];
			var count = cipher.ProcessBytes(input, 0, input.Length, result, 0);

			cipher.DoFinal(result, count);

			return result;
		}

		private string GetArrayAsString(byte[] input)
		{
			var chars = new char[input.Length];

			for (var i = 0; i < input.Length; i++)
				chars[i] = Convert.ToChar(input[i]);

			var result = new string(chars);
			return result;
		}
	}
}
