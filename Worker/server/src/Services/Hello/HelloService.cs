using ServiceStack.ServiceHost;

namespace Server.Services.Hello
{
	public class HelloService: IService
	{
		public HelloResponse get(Hello request)
		{
			return new HelloResponse { Result = request.Name };
		}
	}
}
