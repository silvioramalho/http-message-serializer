using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ttpMessageSerializer.API
{
	public class APIService
	{
		public HttpClient Client { get; }

		public APIService(HttpClient client)
		{
			client.BaseAddress = new Uri("https://08e35dea-b96b-4a6c-8c3d-a47d5f176cbd.mock.pstmn.io/api/forecast");
			//client.DefaultRequestHeaders.Add("Authorization", "YOUR_API_TOKEN");

			Client = client;
		}

	}
}