using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KrisG.SickBox.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KrisG.SickBox.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    var downloader = DefaultDownloader.Create();
                    downloader.Execute();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exception caught at worker root");
                }
                finally
                {
                    _logger.LogInformation($"Worker finished cycle at: {DateTimeOffset.Now}, sleeping for an hour");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
