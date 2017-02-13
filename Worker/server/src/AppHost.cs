using Funq;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.ServiceInterface.Admin;
using ServiceStack.Logging;
using System;
using System.IO;
using Server.Logic;
using Server.Entities;
using RestSharp;
using System.Configuration;

namespace Server
{
    public class AppHost : AppHostHttpListenerBase
    {
		readonly bool m_debugEnabled = true;
		QueuesAndTopics QueuesAndTopicsToRecover = new QueuesAndTopics();
		readonly Locks Locks = new Locks();
		readonly Connections Connections = new Connections();
		readonly string ListenAddress;

		public AppHost (string listenAddress)
            : base ("Server HttpListener", typeof (AppHost).Assembly)
        {
			ListenAddress = listenAddress;
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

			var propagators = new Propagators();

			container.Register(new CreatingQueue(Connections, Locks, propagators));
			container.Register(new CreatingMessage(Connections, Locks, propagators));
			container.Register(new ReadingMessage(Connections, Locks));
			container.Register(new DeletingMessage(Connections, Locks, propagators));
			container.Register(new CreatingTopic(Connections, Locks, propagators));
			container.Register(new CreatingAnnouncement(Connections, Locks, propagators));
			container.Register(new ReadingAnnouncement(Connections, Locks));
			container.Register(new CreatingSubscription(Connections, Locks, propagators));
			container.Register(new DeletingAnnouncement(Connections, Locks, propagators));
			container.Register(new DeletingSubscription(Connections, Locks, propagators));
			container.Register(new DeletingQueue(Connections, Locks, propagators));
			container.Register(new DeletingTopic(Connections, Locks, propagators));
			container.Register(new FailureReporting(Locks, propagators));
			container.Register(new DatabaseRecovery(Locks));
		}

		void RequestQueuesAndTopicsList()
		{
			var client = new RestClient(ConfigurationManager.AppSettings["CoordinatorAddress"]);
			var request = new RestRequest(ConfigurationManager.AppSettings["QueuesAndTopicsPath"], Method.GET);
			request.AddQueryParameter("WorkerId", ConfigurationManager.AppSettings["Id"]);
			request.RequestFormat = DataFormat.Json;
			var response = client.Execute<QueuesAndTopics>(request);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				QueuesAndTopicsToRecover = response.Data;
			}
		}

		void LockAllQueuesToRecover()
		{
			var queuesRecoveryLocks = Locks.QueuesRecoveryLocks;
			foreach (var queue in QueuesAndTopicsToRecover.Queues)
			{
				queuesRecoveryLocks.TryAdd(queue.Value.Name, new byte());
			}
		}

		void LockAllTopicsToRecover()
		{
			var topicsRecoveryLocks = Locks.TopicsRecoveryLocks;
			foreach (var topic in QueuesAndTopicsToRecover.Topics)
			{
				topicsRecoveryLocks.TryAdd(topic.Value.Name, new byte());
			}
		}

		public void BeginRecovery()
		{
			var recoveryController = new RecoveryController(ListenAddress, Locks);
			recoveryController.BeginRecovery(QueuesAndTopicsToRecover);
		}
    }
}