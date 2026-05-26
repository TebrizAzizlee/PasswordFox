
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.DeviceHeartbeats;
using NetWorkPassServer.Application.Monitoring;
using NetWorkPassServer.Domain.Devices;
using TS.MediatR;

namespace NetWorkPassServer.Infrastructure.Services.Monitoring;

public sealed class DevicePollingBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<DevicePollingBackgroundService> logger)
        : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    private readonly ILogger<DevicePollingBackgroundService>
        _logger = logger;
    // 🔥 max parallel polling

    private readonly SemaphoreSlim _semaphore =
        new(20);
    // 🔥 fixed interval

    private static readonly TimeSpan PollingInterval =
        TimeSpan.FromSeconds(30);
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Device polling worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            var startedAt =
                DateTime.UtcNow;
            try
            {
                using var scope =
                    _scopeFactory.CreateScope();

                var context =
                    scope.ServiceProvider
                        .GetRequiredService<
                            IPasswordDbContext>();

                var mediator =
                    scope.ServiceProvider
                        .GetRequiredService<ISender>();

                var strategyResolver =
                    scope.ServiceProvider
                        .GetRequiredService<
                            IDeviceMonitoringStrategyResolver>();

                // 🔥 yalnız aktiv monitoring device-lər oxunur

                var devices = await context.Devices
                    .AsNoTracking()
                    .Where(x =>!x.IsDeleted &&
                        x.IsActive &&
                        x.IsMonitoringEnabled)
                    .Select(x => new
                    {
                        x.Id,
                        x.Type,
                        IpAddress = x.IpAddress.Value
                    })
                    .ToListAsync(stoppingToken);

                _logger.LogInformation(
                    "Polling started for {Count} devices",
                    devices.Count);

                // 🔥 concurrency limit



                var tasks = devices.Select(
                      device =>
                          PollDeviceAsync(
                              mediator,
                              strategyResolver,
                              device.Id,
                             device.Type,
                              device.IpAddress,
                              stoppingToken));

                await Task.WhenAll(tasks);
            }
            catch(OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Critical polling worker error");
            }
            var elapsed =
               DateTime.UtcNow - startedAt;

            var delay =
              PollingInterval - elapsed;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(
                    delay,
                    stoppingToken);
            }

           
        }

        _logger.LogInformation(
            "Device polling worker stopped");
    }



    private async Task PollDeviceAsync(
       ISender mediator,
       IDeviceMonitoringStrategyResolver strategyResolver,
       Guid deviceId, 
       DeviceType deviceType,
       string ipAddress,
       CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(
            cancellationToken);

        try
        {
            var strategy =
                strategyResolver.Resolve(
                    deviceType);

            // 🔥 monitoring execute

            var result =
                await strategy.MonitorAsync(
                    ipAddress,
                    cancellationToken);

            await mediator.Send(
               new DeviceHeartbeatReceivedCommand(
                   deviceId,
                   result.IsReachable,
                   result.ErrorMessage,
                   result.CpuUsage,
                   result.DiskUsage,
                   result.MemoryUsage,
                   result.Temperature,
                   result.UptimeSeconds,
                    result.ResponseTimeMs),
               cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Polling failed for device {DeviceId}",
                deviceId);

            await mediator.Send(
                new DeviceHeartbeatReceivedCommand(
                     deviceId,
        false,
        ex.Message,
       
        null,
        null,
        null,
        null,
        null,
        null),
                cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}