using System.Net;
using System.Text;
using System.Text.Json;

namespace Increo.ServiceBase.Models.Api
{
    internal class ApiResponse
    {
        /// <summary>
        /// When the request started.
        /// </summary>
        public DateTimeOffset? Started { get; set; }

        /// <summary>
        /// When the request ended.
        /// </summary>
        public DateTimeOffset? Ended { get; set; }

        /// <summary>
        /// How long the request took.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Response HTTP status code.
        /// </summary>
        public HttpStatusCode? StatusCode { get; set; }

        /// <summary>
        /// Response body.
        /// </summary>
        public string? Json { get; set; }

        /// <summary>
        /// Thrown error.
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Checks if the request is finished and a success.
        /// </summary>
        /// <returns>Success.</returns>
        public bool IsOk()
        {
            return StatusCode == HttpStatusCode.OK ||
                   StatusCode == HttpStatusCode.Created;
        }

        /// <summary>
        /// Convert response body to type.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Instance.</returns>
        public async Task<T?> JsonToAsync<T>(
            CancellationToken? cancellationToken = null)
        {
            try
            {
                if (Json == null)
                {
                    return default;
                }

                var buffer = Encoding.UTF8.GetBytes(Json);
                var stream = new MemoryStream(buffer);

                var obj = await JsonSerializer.DeserializeAsync<T>(
                    stream,
                    cancellationToken: cancellationToken ?? CancellationToken.None);

                return obj;
            }
            catch
            {
                return default;
            }
        }
    }
}