using Funq;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.ServiceInterface.Admin;
using ServiceStack.Logging;
using System;
using System.IO;
using Server.Logic;
using Server.Entities;
using System.Threading;

namespace Server
{
    public class AppHost : AppHostHttpListenerBase
    {
		readonly bool m_debugEnabled = true;
		readonly QueuesAndTopics QueuesAndTopicsToRecover = new QueuesAndTopics();
		readonly Locks Locks = new Locks();
		readonly Connections Connections = new Connections();

		public AppHost ()
            : base ("Server HttpListener", typeof (AppHost).Assembly)
        {
        }

		public override void Configure (Container container)
        {
			RequestFilters.Add((req, resp, requestDto) => {
				var log = LogManager.GetLogger(GetType());
				log.Info (string.Format ("REQ {0}: {1} {2} {3} {4} {5}", DateTimeOffset.Now.Ticks, req.HttpMethod, 
					req.OperationName, req.RemoteIp, req.RawUrl, req.UserAgent));
			});
			ResponseFilters.Add((req, resp, dto) => {
				var log = LogManager.GetLogger(GetType());
				log.Info (string.Format ("RES {0}: {1} {2}", DateTimeOffset.Now.Ticks, resp.StatusCode,
					resp.ContentType));
			});

			JsConfig.DateHandler = JsonDateHandler.ISO8601;
			Plugins.Add(new RequestLogsFeature());

			ConfigureQueues(container);

            var config = new EndpointHostConfig ();

            if (m_debugEnabled)
            {
                config.DebugMode = true;
                config.WriteErrorsToResponse = true;
                config.ReturnsInnerException = true;
            }

            SetConfig (config);
        }

		void ConfigureQueues(Container container)
		{
			Directory.CreateDirectory("queues");
			Directory.CreateDirectory("topics");

			RequestQueuesAndTopicsList();
			LockAllQueuesToRecover();
			LockAllTopicsToRecover();

			container.Register(new CreatingQueue(Connections, Locks));
			container.Register(new CreatingMessage(Connections, Locks));
			container.Register(new ReadingMessage(Connections, Locks));
			container.Register(new DeletingMessage(Connections, Locks));
			container.Register(new CreatingTopic(Connections, Locks));
			container.Register(new CreatingAnnouncement(Connections, Locks));
			container.Register(new ReadingAnnouncement(Connections, Locks));
			container.Register(new CreatingSubscription(Connections));
			container.Register(new DeletingAnnouncement(Connections));
			container.Register(new DeletingSubscription(Connections));
			container.Register(new DeletingQueue(Connections, Locks));
			container.Register(new DeletingTopic(Connections, Locks));
			container.Register(new FailureReporting(Connections, Locks));
		}

		void RequestQueuesAndTopicsList()
		{
			//TODO implement real request
		}

		void LockAllQueuesToRecover()
		{
			var queuesRecoveryLocks = Locks.QueuesRecoveryLocks;
			foreach (var queue in QueuesAndTopicsToRecover.Queues)
			{
				queuesRecoveryLocks.TryAdd(queue.Value.Name, new ManualResetEventSlim(false));
			}
		}

		void LockAllTopicsToRecover()
		{
			var topicsRecoveryLocks = Locks.TopicsRecoveryLocks;
			foreach (var topic in QueuesAndTopicsToRecover.Topics)
			{
				topicsRecoveryLocks.TryAdd(topic.Value.Name, new ManualResetEventSlim(false));
			}
		}

		public void BeginRecovery()
		{
			var recoveryController = new RecoveryController(Locks);
			recoveryController.BeginRecovery(QueuesAndTopicsToRecover);
		}
    }
}