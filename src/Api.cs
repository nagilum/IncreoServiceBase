using Increo.ServiceBase.Models.Api;
using System.Text;
using System.Text.Json;

namespace Increo.ServiceBase
{
    internal static class Api
    {
        /// <summary>
        /// HTTP client.
        /// </summary>
        private static HttpClient Client { get; } = new();

        /// <summary>
        /// Call the given URL.
        /// </summary>
        /// <param name="uri">URL to call.</param>
        /// <param name="httpMethod">HTTP Method to use.</param>
        /// <param name="headers">Headers to include.</param>
        /// <param name="payload">Payload to submit.</param>
        /// <param name="payloadMediaType">Media type for payload.</param>
        /// <param name="ctoken">Cancellation token.</param>
        /// <returns>API response.</returns>
        public static async Task<ApiResponse> CallAsync(
            Uri uri,
            HttpMethod httpMethod,
            Dictionary<string, string>? headers = null,
            object? payload = null,
            string? payloadMediaType = "application/json",
            CancellationToken? ctoken = null)
        {
            var ar = new ApiResponse();

            try
            {
                var req = new HttpRequestMessage
                {
                    Method = httpMethod,
                    RequestUri = uri
                };

                if (headers?.Count > 0)
                {
                    foreach (var (name, value) in headers)
                    {
                        req.Headers.Add(name, value);
                    }
                }

                MemoryStream stream;

                if (payload != null)
                {
                    stream = new();

                    await JsonSerializer.SerializeAsync(
                        stream,
                        payload,
                        cancellationToken:
                            ctoken ?? CancellationToken.None);

                    req.Content = new StringContent(
                        Encoding.UTF8.GetString(stream.ToArray()),
                        Encoding.UTF8,
                        payloadMediaType);
                }

                ar.Started = DateTimeOffset.Now;

                var res = await Client.SendAsync(
                    req,
                    ctoken ?? CancellationToken.None);

                ar.StatusCode = res.StatusCode;
                ar.Ended = DateTimeOffset.Now;
                ar.Duration = ar.Ended - ar.Started;

                stream = new();

                await res.Content.CopyToAsync(
                    stream,
                    ctoken ?? CancellationToken.None);

                ar.Json = Encoding.UTF8.GetString(stream.ToArray());
            }
            catch (Exception ex)
            {
                ar.Exception = ex;
            }

            return ar;
        }
    }
}