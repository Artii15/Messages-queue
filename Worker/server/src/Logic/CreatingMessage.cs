using System.Collections.Concurrent;
using Server.Entities;
using Server.Services.Messages.Create;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class CreatingMessage
	{
		readonly ConcurrentDictionary<string, IDbConnectionFactory> Queues;

		public CreatingMessage(ConcurrentDictionary<string, IDbConnectionFactory> queues)
		{
			Queues = queues;
		}

		public void Create(CreateMessage request)
		{
			IDbConnectionFactory queueConnectionFactory;
			if (Queues.TryGetValue(request.QueueName, out queueConnectionFactory))
			{
				var connection = queueConnectionFactory.Open();
				connection.Insert(new QueueMessage()
				{
					Author = request.Author,
					Content = request.Content,
					Readed = false
				});
			}
		}
	}
}
