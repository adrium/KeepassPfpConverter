using Adrium.KeepassPfpConverter.Objects;

namespace Adrium.KeepassPfpConverter
{
	public delegate string Hash(string salt, int length);
	public delegate string Cipher(bool encrypt, string iv, string data);
	public delegate string Digest(string data);
	public delegate string Stringify(string bytes, bool lower, bool upper, bool number, bool symbol);
	public delegate string GetPassword(PassEntry entry);
}
