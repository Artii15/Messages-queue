using Funq;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.MiniProfiler;
using ServiceStack.MiniProfiler.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.Validation;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.ServiceInterface.Admin;
using ServiceStack.Logging;
using System;
using Server.Storage;
using Server.Storage.Files;
using Server.Logic;
using System.Threading;
using System.Collections.Concurrent;

namespace Server
{
    public class AppHost : AppHostHttpListenerBase
    {
		private readonly bool m_debugEnabled = true;

        public AppHost ()
            : base ("Server HttpListener", typeof (AppHost).Assembly)
        {
        }

		public override void Configure (Container container)
        {
			RequestFilters.Add((req, resp, requestDto) => {
				ILog log = LogManager.GetLogger(GetType());
				log.Info (string.Format ("REQ {0}: {1} {2} {3} {4} {5}", DateTimeOffset.Now.Ticks, req.HttpMethod, 
					req.OperationName, req.RemoteIp, req.RawUrl, req.UserAgent));
			});
			ResponseFilters.Add((req, resp, dto) => {
				ILog log = LogManager.GetLogger(GetType());
				log.Info (string.Format ("RES {0}: {1} {2}", DateTimeOffset.Now.Ticks, resp.StatusCode,
					resp.ContentType));
			});

            JsConfig.DateHandler = JsonDateHandler.ISO8601;        

            Plugins.Add (new AuthFeature (() => new AuthUserSession (),
                                          new IAuthProvider[] {new CredentialsAuthProvider ()})
                );
            Plugins.Add (new RegistrationFeature ());
			Plugins.Add(new SessionFeature());
			Plugins.Add(new RequestLogsFeature());

			container.Register<ICacheClient> (new MemoryCacheClient ());

            container.Register<IDbConnectionFactory> (
				new OrmLiteConnectionFactory (@"Data Source=db.sqlite;Version=3;",
					SqliteOrmLiteDialectProvider.Instance)
                    {
                        ConnectionFilter = x => new ProfiledDbConnection (x, Profiler.Current)
                    });

            //Use OrmLite DB Connection to persist the UserAuth and AuthProvider info
            container.Register<IUserAuthRepository> (c => new OrmLiteAuthRepository (c.Resolve<IDbConnectionFactory> ()));

			var messagesLocks = new ConcurrentDictionary<string, ReaderWriterLockSlim>();
			var waitOnMessageEvents = new ConcurrentDictionary<string, ManualResetEventSlim>();

			var storageConfig = new FileStorageConfig { RootPath = "./MQ" };
			var storagePaths = new Paths(storageConfig);
			container.Register<QueuesStorage>(new FileQueuesStorage(storagePaths));
			container.Register<MessagesStorage>(new FileMessagesStorage(storagePaths, 
			                                                            messagesLocks, 
			                                                            waitOnMessageEvents));
			container.RegisterAutoWired<CreatingMessage>();

            
			Plugins.Add (new ValidationFeature ());

            var config = new EndpointHostConfig ();

            if (m_debugEnabled)
            {
                config.DebugMode = true; //Show StackTraces in service responses during development
                config.WriteErrorsToResponse = true;
                config.ReturnsInnerException = true;
            }

            SetConfig (config);
        }
    }
}