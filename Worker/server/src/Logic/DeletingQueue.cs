using Server.Services.Queues.Delete;

namespace Server.Logic
{
	public class DeletingQueue
	{
		readonly Connections Connections;
		Locks Locks;

		public DeletingQueue(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Delete(DeleteQueue request)
		{
			var connection = Connections.ConnectToInitializedQueue(request.QueueName);
			connection.Close();
		}
	}
}
