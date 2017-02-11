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
		readonly string ServerAddress;

        public AppHost (string serverAddress)
            : base ("Server HttpListener", typeof (AppHost).Assembly)
        {
			ServerAddress = serverAddress;
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

			var locks = new Locks();
			var connections = new Connections();

			var queuesAndTopicsToRecover = RequestQueuesAndTopicsList();


			container.Register(new CreatingQueue(connections, locks));
			container.Register(new CreatingMessage(connections, locks));
			container.Register(new ReadingMessage(connections, locks));
			container.Register(new DeletingMessage(connections, locks));
			container.Register(new CreatingTopic(connections, locks));
			container.Register(new CreatingAnnouncement(connections, locks));
			container.Register(new ReadingAnnouncement(connections, locks));
			container.Register(new CreatingSubscription(connections));
			container.Register(new DeletingAnnouncement(connections));
			container.Register(new DeletingSubscription(connections));
			container.Register(new DeletingQueue(connections, locks));
			container.Register(new DeletingTopic(connections, locks));
		}

		QueuesAndTopics RequestQueuesAndTopicsList()
		{
			//TODO implement real request
			return new QueuesAndTopics(); //This object will be fetched from coordinator
		}

		void LockAllQueuesToRecover(Locks locks, QueuesAndTopics queuesAndTopics)
		{
			foreach (var queue in queuesAndTopics.Queues)
			{
				Monitor.Enter(locks.TakeQueueLock(queue.Value.Name));
			}
		}

		void LockAllTopicsToRecover(Locks locks, QueuesAndTopics queuesAndTopics)
		{
			foreach (var topic in queuesAndTopics.Topics)
			{
				Monitor.Enter(locks.TakeTopicLock(topic.Value.Name));
			}
		}
    }
}