using Funq;
using ServiceStack.MiniProfiler;
using ServiceStack.MiniProfiler.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.PostgreSQL;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.Validation;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.ServiceInterface.Admin;
using ServiceStack.Logging;
using System;

namespace Server
{
    public class AppHost : AppHostHttpListenerBase
    {
		private readonly bool m_debugEnabled = true;

		private const string ENV_PG_IP = "PG_IP";
		private const string ENV_PG_USER = "PG_USER";
		private const string ENV_PG_DB = "PG_DB";
		private const string ENV_PG_PASS = "PG_PASS";
		private const string ENV_PG_PORT = "PG_PORT";

		string m_pgIp;
		string m_pgUser;
		string m_pgDb;
		string m_pgPass;
		string m_pgPort;
		string m_pgConnString;

        public AppHost ()
            : base ("Server HttpListener", typeof (AppHost).Assembly)
        {
        }

		public override void Configure (Container container)
        {
			LoadConfigEnv();

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

            Plugins.Add (new AuthFeature (() => new AuthUserSession (),
                                          new IAuthProvider[] {new CredentialsAuthProvider ()})
                );
            Plugins.Add (new RegistrationFeature ());
			Plugins.Add(new SessionFeature());
			Plugins.Add(new RequestLogsFeature());

			container.Register<IDbConnectionFactory>(
				new OrmLiteConnectionFactory(m_pgConnString, PostgreSQLDialectProvider.Instance)
				{
					ConnectionFilter = x => new ProfiledDbConnection(x, Profiler.Current)
				});

			//Use OrmLite DB Connection to persist the UserAuth and AuthProvider info
			container.Register<IUserAuthRepository>(c => new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));


			Plugins.Add (new ValidationFeature ());

            var config = new EndpointHostConfig ();

            if (m_debugEnabled)
            {
                config.DebugMode = true; //Show StackTraces in service responses during development
                config.WriteErrorsToResponse = true;
                config.ReturnsInnerException = true;
            }

            SetConfig (config);
			CreateMissingTables(container);
        }

		void LoadConfigEnv()
		{
			/*m_pgIp = Environment.GetEnvironmentVariable(ENV_PG_IP);
			m_pgUser = Environment.GetEnvironmentVariable(ENV_PG_USER);
			m_pgDb = Environment.GetEnvironmentVariable(ENV_PG_DB);
			m_pgPass = Environment.GetEnvironmentVariable(ENV_PG_PASS);
			m_pgPort = Environment.GetEnvironmentVariable(ENV_PG_PORT);*/
			m_pgIp = "localhost";
			m_pgUser = "test";
			m_pgDb = "test";
			m_pgPass = "test";
			m_pgPort = "5432";

			m_pgConnString = string.Format("User ID={0};Password={1};Host={2};Port={3};Database={4};SSL=True",
				m_pgUser, m_pgPass, m_pgIp, m_pgPort, m_pgDb);
		}

		private void CreateMissingTables(Container container)
		{
			var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>();
			authRepo.CreateMissingTables();
		}
    }
}