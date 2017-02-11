using Server.Logic;

namespace Server.Services.Failures.Topics
{
	public class TopicFailureService
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
