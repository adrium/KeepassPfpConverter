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
		private const int N = 32768;
		private const int r = 8;
		private const int p = 1;

		private const int AES_KEY_SIZE = 256;
		private const int TAG_LENTH = 128;

		private byte[] hmacBytes;
		private byte[] passBytes;
		private byte[] keyBytes;

		public void SetMasterPassword(string masterPassword)
		{
			passBytes = Encoding.UTF8.GetBytes(masterPassword);
			keyBytes = null;
		}

		public void SetSalt(string salt)
		{
			var saltstring = GetArrayAsString(Convert.FromBase64String(salt));
			keyBytes = Hash(saltstring, AES_KEY_SIZE / 8);
		}

		public void SetHmacSecret(string hmac)
		{
			hmacBytes = Convert.FromBase64String(hmac);
		}

		public string Decrypt(string data)
		{
			var dataarray = data.Split('_');
			var iv = Convert.FromBase64String(dataarray[0]);
			var input = Convert.FromBase64String(dataarray[1]);

			var result = Encoding.UTF8.GetString(Process(false, iv, input));

			return result;
		}

		public byte[] Hash(string salt, int length)
		{
			ValidatePass();
			var S = Encoding.UTF8.GetBytes(salt);
			return SCrypt.Generate(passBytes, S, N, r, p, length);
		}

		private byte[] Process(bool encrypt, byte[] iv, byte[] input)
		{
			ValidateKey();

			var cipher = new GcmBlockCipher(new AesEngine());
			var parameters = new AeadParameters(new KeyParameter(keyBytes), TAG_LENTH, iv);

			cipher.Init(encrypt, parameters);
			var result = new byte[cipher.GetOutputSize(input.Length)];
			var count = cipher.ProcessBytes(input, 0, input.Length, result, 0);

			cipher.DoFinal(result, count);

			return result;
		}

		private static string GetArrayAsString(byte[] input)
		{
			var chars = new char[input.Length];

			for (var i = 0; i < input.Length; i++) {
				chars[i] = Convert.ToChar(input[i]);
			}

			var result = new string(chars);
			return result;
		}

		private void ValidatePass()
		{
			if (passBytes == null)
				throw new InvalidOperationException("Password needed");
		}

		private void ValidateKey()
		{
			if (keyBytes == null)
				throw new InvalidOperationException("Key needed");
		}
	}
}
