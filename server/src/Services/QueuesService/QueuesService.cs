using System;
using ServiceStack.ServiceInterface;

namespace Server.Services.QueuesService
{
	public class QueuesService: Service
	{
		public object Post(QueuesRequest req)
		{
			return new QueuesResponse ();
		}
	}
}

