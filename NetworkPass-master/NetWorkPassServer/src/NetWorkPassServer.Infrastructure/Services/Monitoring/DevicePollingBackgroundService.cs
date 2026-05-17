
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.DeviceHeartbeats;
using System.Net.NetworkInformation;

namespace NetWorkPassServer.Infrastructure.Services.Monitoring
{
    public sealed class DevicePollingBackgroundService
     : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DevicePollingBackgroundService(
            IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
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

                foreach (var device in devices)
                {
                    try
                    {
                        using var ping = new Ping();

                        var reply = await ping.SendPingAsync(
                            device.IpAddress,
                            3000);

                        bool isReachable =
                            reply.Status == IPStatus.Success;

                        int? latency =
                            isReachable
                                ? (int)reply.RoundtripTime
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
                        await mediator.Send(
                            new DeviceHeartbeatReceivedCommand(
                                device.Id,
                                false,
                                null,
                                ex.Message
                            ),
                            stoppingToken);
                    }
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(30),
                    stoppingToken);
            }
        }
    }
}
