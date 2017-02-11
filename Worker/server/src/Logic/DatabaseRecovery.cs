using System.Collections.Concurrent;
using System.IO;
using Server.Services.Databases.Queues;
using Server.Services.Databases.Topics;

namespace Server.Logic
{
	public class DatabaseRecovery
	{
		Locks Locks;

		public DatabaseRecovery(Locks locks)
		{
			Locks = locks;
		}

		public void Recover(QueueDatabase request)
		{
			Recover(new RecoveryDescriptor
			{
				DB = request.DatabaseFile,
				DBName = request.Name,
				Locks = Locks.QueuesRecoveryLocks,
				PathToDbFile = Connections.PathToDbFile(Connections.QueuesDir, request.Name)
			});
		}

		public void Recover(TopicDatabase request)
		{
			Recover(new RecoveryDescriptor
			{
				DB = request.DatabaseFile,
				DBName = request.Name,
				Locks = Locks.TopicsRecoveryLocks,
				PathToDbFile = Connections.PathToDbFile(Connections.TopicsDir, request.Name)
			});
		}

		void Recover(RecoveryDescriptor recoveryDescriptor)
		{
			File.Delete(recoveryDescriptor.PathToDbFile);
			File.WriteAllBytes(recoveryDescriptor.PathToDbFile, recoveryDescriptor.DB);

			byte dummyValue;
			recoveryDescriptor.Locks.TryRemove(recoveryDescriptor.DBName, out dummyValue);
		}
	}

	struct RecoveryDescriptor
	{
		public string PathToDbFile { get; set; }
		public ConcurrentDictionary<string, byte> Locks { get; set; }
		public byte[] DB { get; set; }
		public string DBName { get; set; }
	}
}
