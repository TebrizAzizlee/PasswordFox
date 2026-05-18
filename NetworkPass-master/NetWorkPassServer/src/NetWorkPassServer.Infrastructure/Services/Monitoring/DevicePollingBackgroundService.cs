using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.DeviceHeartbeats;
using NetWorkPassServer.Application.Devices;
using System.Net.NetworkInformation;
using TS.MediatR;

namespace NetWorkPassServer.Infrastructure.Monitoring.BackgroundServices;

public sealed class DevicePollingBackgroundService
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ILogger<DevicePollingBackgroundService>
        _logger;

    public DevicePollingBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<DevicePollingBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;

        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Device polling worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
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
                        .GetRequiredService<IMediator>();

                // 🔥 yalnız aktiv monitoring device-lər

                var devices = await context.Devices
                    .AsNoTracking()
                    .Where(x =>
                        x.IsActive &&
                        x.IsMonitoringEnabled)
                    .Select(x => new
                    {
                        x.Id,
                        IpAddress = x.IpAddress.Value
                    })
                    .ToListAsync(stoppingToken);

                _logger.LogInformation(
                    "Polling started for {Count} devices",
                    devices.Count);

                // 🔥 concurrency limit

                using var semaphore =
                    new SemaphoreSlim(20);

                var tasks = devices.Select(async device =>
                {
                    await semaphore.WaitAsync(
                        stoppingToken);

                    try
                    {
                        using var ping = new Ping();

                        var reply =
                            await ping.SendPingAsync(
                                device.IpAddress,
                                3000);

                        bool isReachable =
                            reply.Status ==
                            IPStatus.Success;

                        long? latency =
                            isReachable
                                ? reply.RoundtripTime
                                : null;

                        await mediator.Send(
                            new DeviceHeartbeatReceivedCommand(
                                device.Id,
                                isReachable,
                                latency,
                                isReachable
                                    ? null
                                    : reply.Status.ToString()
                            ),
                            stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Polling failed for device {DeviceId}",
                            device.Id);

                        await mediator.Send(
                            new DeviceHeartbeatReceivedCommand(
                                device.Id,
                                false,
                                null,
                                ex.Message
                            ),
                            stoppingToken);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Critical polling worker error");
            }

            // 🔥 polling interval

            await Task.Delay(
                TimeSpan.FromSeconds(30),
                stoppingToken);
        }

        _logger.LogInformation(
            "Device polling worker stopped");
    }
}