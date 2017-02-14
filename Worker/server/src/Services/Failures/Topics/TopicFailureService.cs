using Server.Logic;
using ServiceStack.ServiceHost;

namespace Server.Services.Failures.Topics
{
	public class TopicFailureService: IService
	{
		readonly FailureReporting FailureReporting;

		public TopicFailureService(FailureReporting failureReporting)
		{
			FailureReporting = failureReporting;
		}

		public TopicFailureResponse Post(TopicFailure request)
		{
			FailureReporting.Report(request);
			return new TopicFailureResponse();
		}
	}
}
