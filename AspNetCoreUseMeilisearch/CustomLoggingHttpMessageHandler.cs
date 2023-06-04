using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;

namespace AspNetCoreUseMeilisearch
{
    public class CustomLoggingHttpMessageHandler : DelegatingHandler
    {
        private readonly ILogger<CustomLoggingHttpMessageHandler> _logger;
        public CustomLoggingHttpMessageHandler(ILogger<CustomLoggingHttpMessageHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            StringBuilder sb = new();

            sb.AppendLine($"RequestExternalSystem ");
            sb.AppendLine($"RequestTime:{DateTime.Now.ToString()} ");
            sb.AppendLine($"Method:{request.Method} ");
            sb.AppendLine($"RequestUri:{request.RequestUri} ");
            if (request.Headers.Any())
            {
                var headers = request.Headers.Select(header => header.Key + "=" + string.Join(",", header.Value));
                sb.AppendLine($"Headers:{string.Join(";", headers)}");
            }
            if (request.Content != null)
            {
                string requestBody = await request.Content.ReadAsStringAsync();
                sb.AppendLine($"RequestBody:{requestBody} ");
            }

            var options = new JObject();
            foreach (var item in request.Options)
            {
                try
                {
                    options[item.Key] = JsonConvert.SerializeObject(item.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"序列化HttpRequestOptions异常，Key={item.Key}，RequestUri={request.RequestUri}");
                }
            }
            if (options.Properties().Any())
            {
                sb.AppendLine($"Options:{options.ToString()}");
            }

            var stopWatch = Stopwatch.StartNew();

            var httpResponseMessage = await base.SendAsync(request, cancellationToken);

            sb.AppendLine($"StatusCode:{(int)httpResponseMessage.StatusCode} {stopWatch.ElapsedMilliseconds}ms");

            var reponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            sb.AppendLine($"ReponseBody:{reponseBody} ");

            LogLevel level = LogLevel.Information;
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                level = LogLevel.Error;
            }

            _logger.Log(level, message: sb.ToString());

            return httpResponseMessage;
        }
    }
}
