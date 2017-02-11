using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Failures.Queues
{
	public class QueueFailureService: IService
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
