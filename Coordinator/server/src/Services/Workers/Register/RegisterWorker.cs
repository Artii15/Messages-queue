using ServiceStack.ServiceHost;

namespace Server
{
	[Route("/workers", "post")]
	[Route("/workers/{Id}", "put")]
	[Restrict(InternalOnly = true)]
	public class RegisterWorker
	{
		public int? Id { get; set; }
		public string Address { get; set; }
	}
}
