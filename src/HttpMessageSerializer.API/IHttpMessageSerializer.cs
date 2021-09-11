using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpMessageSerializer.API
{
    public interface IHttpMessageSerializer
    {
        Task SerializeAsync(HttpResponseMessage response, Stream stream);
        Task SerializeAsync(HttpRequestMessage request, Stream stream);
        Task<HttpResponseMessage> DeserializeToResponseAsync(Stream stream);
        Task<HttpRequestMessage> DeserializeToRequestAsync(Stream stream);
    }
}
