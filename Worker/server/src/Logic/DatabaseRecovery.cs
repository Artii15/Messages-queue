using System.Collections.Concurrent;
using System.IO;

namespace Server.Logic
{
	public class DatabaseRecovery
	{
		Locks Locks;

		public DatabaseRecovery(Locks locks)
		{
			Locks = locks;
		}

		public void RecoverQueue(string name, Stream db)
		{
			Recover(new RecoveryDescriptor
			{
				Db = db,
				DbName = name,
				Locks = Locks.QueuesRecoveryLocks,
				PathToDbFile = Connections.PathToDbFile(Connections.QueuesDir, name)
			});
		}

		public void RecoverTopic(string name, Stream db)
		{
			Recover(new RecoveryDescriptor
			{
				Db = db,
				DbName = name,
				Locks = Locks.TopicsRecoveryLocks,
				PathToDbFile = Connections.PathToDbFile(Connections.TopicsDir, name)
			});
		}

		void Recover(RecoveryDescriptor recoveryDescriptor)
		{
			File.Delete(recoveryDescriptor.PathToDbFile);
			using (var dbFile = File.Create(recoveryDescriptor.PathToDbFile))
			{
				recoveryDescriptor.Db.CopyTo(dbFile);
			}

			byte dummyValue;
			recoveryDescriptor.Locks.TryRemove(recoveryDescriptor.DbName, out dummyValue);
		}
	}

	struct RecoveryDescriptor
	{
		public string PathToDbFile { get; set; }
		public ConcurrentDictionary<string, byte> Locks { get; set; }
		public Stream Db { get; set; }
		public string DbName { get; set; }
	}
}
