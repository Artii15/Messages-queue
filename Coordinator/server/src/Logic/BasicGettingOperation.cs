using System;
using System.Data;
using System.Net;
using RestSharp;

namespace Server
{
	public abstract class BasicGettingOperation : BasicOperation
	{
		public BasicGettingOperation(IDbConnection dbConnection) : base(dbConnection)
		{
		}

		protected abstract void swapWorkers(ICollection collection);

		IRestResponse<T> PropagateRequest<T>(IRestRequest request, Worker worker) where T : new()
		{
			var client = new RestClient(worker.Address);
			client.Timeout = TIMEOUT;
			return client.Execute<T>(request);
		}

		protected T PropageteRequestToWorkers<T>(RestRequest request, ICollection collection, Worker worker, Worker coworker) where T: new()
		{
			IRestResponse<T> response;

			if (WorkerQueries.IsWorkerAlive(DBConnection, worker.Id))
			{
				response = PropagateRequest<T>(request, worker);
				if (response.ResponseStatus == ResponseStatus.TimedOut ||
					response.ResponseStatus == ResponseStatus.Error)
				{
					response = PropagateRequest<T>(request, coworker);
					swapWorkers(collection);
				}
			}
			else
			{
				response = PropagateRequest<T>(request, coworker);
				swapWorkers(collection);
			}
			if (response.StatusCode != HttpStatusCode.OK)
				throw new BadRequestException();

			return response.Data;
		}
	}
}
