using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Security.Cryptography;
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

		private const int IV_LENGTH = 12;
		private const int HMAC_LENGTH = 32;
		private const int SALT_LENGTH = 16;

		private byte[] hmacBytes;
		private byte[] passBytes;
		private byte[] saltBytes;
		private byte[] keyBytes;

		public static string GenerateRandom(int length)
		{
			var rng = RandomNumberGenerator.Create();
			var gen = new byte[length];
			rng.GetBytes(gen);

			var result = Convert.ToBase64String(gen);
			return result;
		}

		public void SetMasterPassword(string masterPassword)
		{
			passBytes = Encoding.UTF8.GetBytes(masterPassword);
			keyBytes = null;
		}

		public void SetSalt(string salt)
		{
			saltBytes = Convert.FromBase64String(salt);
			keyBytes = Hash(GetArrayAsString(saltBytes), AES_KEY_SIZE / 8);
		}

		public void SetHmacSecret(string hmac)
		{
			hmacBytes = Convert.FromBase64String(hmac);
		}

		public string GetSalt()
		{
			var result = saltBytes == null ? null : Convert.ToBase64String(saltBytes);
			return result;
		}

		public string GetHmacSecret()
		{
			var result = hmacBytes == null ? null : Convert.ToBase64String(hmacBytes);
			return result;
		}

		public void GenerateSalt()
		{
			SetSalt(GenerateRandom(SALT_LENGTH));
		}

		public void GenerateHmacSecret()
		{
			SetHmacSecret(GenerateRandom(HMAC_LENGTH));
		}

		public string Decrypt(string data)
		{
			var dataarray = data.Split('_');
			var iv = Convert.FromBase64String(dataarray[0]);
			var input = Convert.FromBase64String(dataarray[1]);

			var result = Encoding.UTF8.GetString(Process(false, iv, input));

			return result;
		}

		public string Encrypt(string data)
		{
			return Encrypt(data, GenerateRandom(IV_LENGTH));
		}

		public string Encrypt(string data, string ivstr)
		{
			var iv = Convert.FromBase64String(ivstr);
			var input = Encoding.UTF8.GetBytes(data);

			var enc = Convert.ToBase64String(Process(true, iv, input));
			var result = ivstr + "_" + enc;

			return result;
		}

		public byte[] Hash(string salt, int length)
		{
			ValidatePass();
			var S = Encoding.UTF8.GetBytes(salt);
			return SCrypt.Generate(passBytes, S, N, r, p, length);
		}

		public byte[] Digest(string data)
		{
			ValidateHmac();

			var bytes = Encoding.UTF8.GetBytes(data);

			var hmac = new HMac(new Sha256Digest());

			hmac.Init(new KeyParameter(hmacBytes));
			var result = new byte[hmac.GetMacSize()];

			hmac.BlockUpdate(bytes, 0, bytes.Length);
			hmac.DoFinal(result, 0);

			return result;
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

		private void ValidateHmac()
		{
			if (hmacBytes == null)
				throw new InvalidOperationException("HMAC needed");
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
