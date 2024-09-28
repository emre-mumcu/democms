using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using src.App_Data;
using src.App_Data.Types;
using src.App_Lib.Configuration.Ext;
using src.App_Lib.Extensions;

namespace src.App_Lib.Cache
{
    public class CacheCleanJob : IHostedService, IDisposable
    {

        private int executionCount = 0;
        private readonly ILogger<CacheCleanJob> _logger;
        private Timer? _timer = null;
        IOptions<DataOptions> _options;
        private readonly IServiceProvider _serviceProvider;

        public CacheCleanJob(ILogger<CacheCleanJob> logger, IServiceProvider serviceProvider, IOptions<DataOptions> options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _options = options;

        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(3600));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);

            CacheReset cacheReset = new CacheReset(_options, _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>());
            cacheReset.ResetCache(EnumCacheNames.Cache1.GetEnumDescription());


        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}