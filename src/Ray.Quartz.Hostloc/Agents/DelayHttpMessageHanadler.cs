using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ray.Quartz.Hostloc.Agents
{
    public class DelayHttpMessageHandler : DelegatingHandler
    {
        private readonly ILogger<DelayHttpMessageHandler> _logger;
        private readonly HttpClientCustomOptions _options;

        public DelayHttpMessageHandler(IOptions<HttpClientCustomOptions> options, ILogger<DelayHttpMessageHandler> logger)
        {
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("休眠{seconds}秒", _options.RandomDelaySecondsBetweenCalls);
            await Task.Delay(TimeSpan.FromSeconds(_options.RandomDelaySecondsBetweenCalls), cancellationToken);

            var response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
