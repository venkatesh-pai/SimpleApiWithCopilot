using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SimpleApiWithCopilot.Middlewares
{
    public class HttpLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpLoggingMiddleware> _logger;

        public HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log the request
            await LogRequestAsync(context);

            // Use a memory stream to capture the response
            var originalResponseBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            try
            {
                // Call the next middleware in the pipeline
                await _next(context);

                // Log the response
                await LogResponseAsync(context);

                // Copy the response back to the original stream
                await responseBodyStream.CopyToAsync(originalResponseBodyStream);
            }
            finally
            {
                context.Response.Body = originalResponseBodyStream;
            }
        }

        private async Task LogRequestAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            // Read the request body
            var bodyAsText = await ReadStreamAsync(context.Request.Body);
            _logger.LogInformation($"HTTP Request Information: \n" +
                                   $"Method: {context.Request.Method} \n" +
                                   $"Path: {context.Request.Path} \n" +
                                   $"Headers: {string.Join("; ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))} \n" +
                                   $"Body: {bodyAsText}");

            // Reset the stream position
            context.Request.Body.Position = 0;
        }

        private async Task LogResponseAsync(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await ReadStreamAsync(context.Response.Body);
            _logger.LogInformation($"HTTP Response Information: \n" +
                                   $"Status Code: {context.Response.StatusCode} \n" +
                                   $"Headers: {string.Join("; ", context.Response.Headers.Select(h => $"{h.Key}: {h.Value}"))} \n" +
                                   $"Body: {bodyAsText}");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
        }

        private async Task<string> ReadStreamAsync(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var text = await reader.ReadToEndAsync();
            stream.Seek(0, SeekOrigin.Begin);
            return text;
        }
    }
}
