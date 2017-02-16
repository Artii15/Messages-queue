using System;
using System.Security.Cryptography;
using System.Text;

namespace Server
{
	public class TokenGenerator
	{
		readonly string Key;
		readonly MD5 Hasher = MD5.Create();

		public TokenGenerator(string key)
		{
			Key = key;
		}

		public TimeToken Generate(int workerId, string workerAddress)
		{
			var time = DateTime.UtcNow.ToBinary();
			return new TimeToken
			{
				Time = time,
				Token = GetMd5Hash($"{Key}{time}{workerId}{workerAddress}")
			};
		}

		string GetMd5Hash(string input)
		{
			var data = Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
			var sBuilder = new StringBuilder();

			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			return sBuilder.ToString();
		}
	}

	public struct TimeToken
	{
		public long Time { get; set; }
		public string Token { get; set; }
	}
}
