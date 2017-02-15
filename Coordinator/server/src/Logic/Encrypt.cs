using System;
using System.Security.Cryptography;
using System.Text;

namespace Server
{
	public static class Encrypt
	{

		public static bool EncryptToken(long time, String token)
		{
			using (MD5 md5Hash = MD5.Create())
			{
				string key = Environment.GetEnvironmentVariable("KEY");
				string input = key + time.ToString();

				return VerifyMd5Hash(md5Hash, input, token);
			}

		}

		static string GetMd5Hash(MD5 md5Hash, string input)
		{
			byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
			StringBuilder sBuilder = new StringBuilder();

			for (int i = 0; i < data.Length; i++)
				sBuilder.Append(data[i].ToString("x2"));

			return sBuilder.ToString();
		}

		static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
		{
			string hashOfInput = GetMd5Hash(md5Hash, input);

			StringComparer comparer = StringComparer.OrdinalIgnoreCase;

			if (0 == comparer.Compare(hashOfInput, hash))
				return true;

			return false;
		}
	}
}
