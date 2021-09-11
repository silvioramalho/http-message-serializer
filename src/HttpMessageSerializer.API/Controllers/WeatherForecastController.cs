using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HttpMessageSerializer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;


        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
		public async Task<IEnumerable<WeatherForecast>> Post()
        {
            var json = new
            {
                id = Guid.NewGuid().ToString(),
                name = "Test serialize anda deserialize"
            };

            string jsonString = JsonSerializer.Serialize(json);
            var payload = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient("APIClient");


            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://08e35dea-b96b-4a6c-8c3d-a47d5f176cbd.mock.pstmn.io/api/forecast");
            requestMessage.Headers.Range = new RangeHeaderValue(0, 1) { Unit = "custom" };
            requestMessage.Content = payload;
            
            var serializer = new MessageContentHttpMessageSerializer();
            var memoryStream = new MemoryStream();
            await serializer.SerializeAsync(requestMessage, memoryStream);
            memoryStream.Position = 0;
            var newRequest = await serializer.DeserializeToRequestAsync(memoryStream);

            var response = await client.SendAsync(newRequest);

            string responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(responseJson);


        }
    }
}
