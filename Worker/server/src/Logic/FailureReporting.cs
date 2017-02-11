using Server.Services.Failures.Queues;
using Server.Services.Failures.Topics;

namespace Server.Logic
{
	public class FailureReporting
	{
		Connections Connections;
		Locks Locks;

		public FailureReporting(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void ReportQueueFailure(QueueFailure request)
		{

		}

		public void ReportTopicFailure(TopicFailure request)
		{

		}
	}
}
