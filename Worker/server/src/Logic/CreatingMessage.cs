using System.Collections.Concurrent;
using Server.Entities;
using Server.Services.Messages.Create;
using ServiceStack.OrmLite;

namespace Server.Logic
{
	public class CreatingMessage
	{
		readonly Connections Connections;

		public CreatingMessage(Connections connections)
		{
			Connections = connections;
		}

		public void Create(CreateMessage request)
		{
			var connection = Connections.ConnectToInitializedQueue(request.QueueName);
			connection.Insert(new QueueMessage
			{
				Author = request.Author,
				Content = request.Content,
				Readed = false
			});
			connection.Close();
		}
	}
}
