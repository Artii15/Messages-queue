using Funq;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.ServiceInterface.Admin;
using ServiceStack.Logging;
using System;
using System.IO;
using Server.Logic;

namespace Server
{
    public class AppHost : AppHostHttpListenerBase
    {
		readonly bool m_debugEnabled = true;

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

			var locks = new Locks();
			var connections = new Connections();

			container.Register(new CreatingQueue(connections, locks));
			container.Register(new CreatingMessage(connections, locks));
			container.Register(new ReadingMessage(connections, locks));
			container.Register(new DeletingMessage(connections, locks));
		}
    }
}