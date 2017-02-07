using ServiceStack.ServiceHost;

namespace Server.Services.Hello
{
	[Route("/hello", "get")]
	public class Hello
	{
		public string Name { get; set; }
	}
}
