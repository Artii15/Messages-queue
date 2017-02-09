using Server.Services.Topics.Create;

namespace Server.Logic
{
	public class CreatingTopic
	{
		Connections Connections;
		Locks Locks;

		public CreatingTopic(Connections connections, Locks locks)
		{
			Connections = connections;
			Locks = locks;
		}

		public void Create(CreateTopic request)
		{

		}
	}
}
