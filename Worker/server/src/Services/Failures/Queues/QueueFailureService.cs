using Server.Logic;
using ServiceStack.ServiceInterface;

namespace Server.Services.Failures.Queues
{
	public class QueueFailureService: Service
	{
		readonly FailureReporting FailureReporting;

		public QueueFailureService(FailureReporting failureReporting)
		{
			FailureReporting = failureReporting;
		}

		public QueueFailureResponse Post(QueueFailure request)
		{
			FailureReporting.Report(request);
			return new QueueFailureResponse();
		}
	}
}
