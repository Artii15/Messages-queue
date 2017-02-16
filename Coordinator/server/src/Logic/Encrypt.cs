using System;
using System.Security.Cryptography;
using System.Text;

namespace Server
{
	public static class Encrypt
	{

		public static bool EncryptToken(long time, int workerId, string workerAddress, string token)
		{
			using (MD5 md5Hash = MD5.Create())
			{
				var key = Environment.GetEnvironmentVariable("KEY");
				var input = $"{key}{time}{workerId}{workerAddress}";

				return VerifyMd5Hash(md5Hash, input, token);
			}

		}

		static string GetMd5Hash(MD5 md5Hash, string input)
		{
			var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
			var sBuilder = new StringBuilder();

			for (int i = 0; i < data.Length; i++)
				sBuilder.Append(data[i].ToString("x2"));

			return sBuilder.ToString();
		}

		static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
		{
			var hashOfInput = GetMd5Hash(md5Hash, input);

			StringComparer comparer = StringComparer.OrdinalIgnoreCase;

			if (0 == comparer.Compare(hashOfInput, hash))
				return true;

			return false;
		}
	}
}
