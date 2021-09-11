using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HttpMessageSerializer.API
{
	public class MessageContentHttpMessageSerializer : IHttpMessageSerializer
	{
        private bool _bufferContent;

        public MessageContentHttpMessageSerializer()
            : this(true)
        {

        }

        public MessageContentHttpMessageSerializer(bool bufferContent)
        {
            _bufferContent = bufferContent;
        }

        public async Task SerializeAsync(HttpResponseMessage response, Stream stream)
        {
            if (response.Content != null)
            {
                if (_bufferContent)
                    await response.Content.LoadIntoBufferAsync().ConfigureAwait(false);
            }
            

            var httpMessageContent = new HttpMessageContent(response);
            var buffer = await httpMessageContent.ReadAsByteArrayAsync();
            stream.Write(buffer, 0, buffer.Length);
        }

        public async Task SerializeAsync(HttpRequestMessage request, Stream stream)
        {
            if (request.Content != null && _bufferContent)
            {
                await request.Content.LoadIntoBufferAsync().ConfigureAwait(false);
            }

            var httpMessageContent = new HttpMessageContent(request);
            var buffer = await httpMessageContent.ReadAsByteArrayAsync().ConfigureAwait(false);
            stream.Write(buffer, 0, buffer.Length);
        }

        public async Task<HttpResponseMessage> DeserializeToResponseAsync(Stream stream)
        {
            var response = new HttpResponseMessage();
            response.Content = new StreamContent(stream);

            var responseMessage = await response.Content.ReadAsHttpResponseMessageAsync().ConfigureAwait(false);
            if (responseMessage.Content != null && _bufferContent)
                await responseMessage.Content.LoadIntoBufferAsync().ConfigureAwait(false);
            return responseMessage;
        }

        public async Task<HttpRequestMessage> DeserializeToRequestAsync(Stream stream)
        {
            var request = new HttpRequestMessage();
            request.Content = new StreamContent(stream);
            request.Content.Headers.Add("Content-Type", "application/http;msgtype=request");

            var requestMessage = await request.Content.ReadAsHttpRequestMessageAsync().ConfigureAwait(false);
            if (requestMessage.Content != null && _bufferContent)
                await requestMessage.Content.LoadIntoBufferAsync().ConfigureAwait(false);
            return requestMessage;
        }
    }
}
