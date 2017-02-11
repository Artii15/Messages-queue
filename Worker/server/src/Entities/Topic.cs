namespace Server.Entities
{
	public class Topic: MessagesContainer
	{
		public string Name { get; set; }
		public string Cooperator { get; set; }

		public string GetCooperator()
		{
			return Cooperator;
		}

		public string GetName()
		{
			return Name;
		}
	}
}
