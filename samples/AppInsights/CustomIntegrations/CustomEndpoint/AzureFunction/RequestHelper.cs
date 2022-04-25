using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace CustomTelemetryHandler
{
    /// <summary>
    /// Helper class for extension methods facilitating HttpRequest operations.
    /// </summary>
    public static class RequestHelper
    {
        private const string GZipEncoding = "gzip";
        private const string DeflateEncoding = "deflate";

        /// <summary>
        /// Returns request body's content, handling gzip/deflate encoding.
        /// </summary>
        /// <param name="request">The HttpRequest object.</param>
        /// <returns>The request's body as string.</returns>
        public static Task<string> GetBodyAsStringAsync(this HttpRequest request)
        {
            request.Headers.TryGetValue("Content-Encoding", out var value);
            if (value.Equals(GZipEncoding))
            {
                return request.Body.DecompressGzipAsync();
            }
            else if (value.Equals(DeflateEncoding))
            {
                return request.Body.DecomporessDeflateAsync();
            }
            else
            {
                return request.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Decompresses Gzip stream and returns string content.
        /// </summary>
        private static Task<string> DecompressGzipAsync(this Stream stream)
        {
            using (var decompressStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                using (var sr = new StreamReader(decompressStream))
                {
                    return sr.ReadToEndAsync();
                }
            }
        }

        /// <summary>
        /// Decompresses Deflate stream and returns string content.
        /// </summary>
        private static Task<string> DecomporessDeflateAsync(this Stream stream)
        {
            using (var decompressStream = new DeflateStream(stream, CompressionMode.Decompress))
            {
                using (var sr = new StreamReader(decompressStream))
                {
                    return sr.ReadToEndAsync();
                }
            }
        }
    }
}