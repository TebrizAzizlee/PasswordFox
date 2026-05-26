using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetWorkPassServer.Domain.VpnTunnelHeartbeats;
using NetWorkPassServer.Infrastructure.Context;
using System.Net.NetworkInformation;

namespace NetWorkPassServer.Infrastructure.Services.Monitoring;
internal sealed class VpnMonitoringWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<VpnMonitoringWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope =
                    scopeFactory.CreateScope();

                var context =
                    scope.ServiceProvider
                        .GetRequiredService<
                            PasswordDbContext>();

                var tunnels = await context.VpnTunnels
                    .Where(x =>
                        x.IsActive &&
                        x.IsMonitoringEnabled)
                    .ToListAsync(stoppingToken);

                foreach (var tunnel in tunnels)
                {
                    try
                    {
                        using var ping = new Ping();

                        var reply =
                            await ping.SendPingAsync(
                                tunnel.RemoteIpAddress.Value,
                                3000);

                        var isReachable =
                            reply.Status ==
                            IPStatus.Success;

                        long? latency =
                            isReachable
                                ? reply.RoundtripTime
                                : null;

                        var oldStatus =
                            tunnel.Status;

                        if (isReachable)
                        {
                            tunnel.MarkConnected(
                                latency);
                        }
                        else
                        {
                            tunnel.MarkDisconnected();
                        }

                        var heartbeat =
                            new VpnTunnelHeartbeat(
                                tunnel.Id,
                                tunnel.Status,
                                isReachable,
                                latency,
                                isReachable
                                    ? null
                                    : reply.Status.ToString());

                        await context
                            .VpnTunnelHeartbeats
                            .AddAsync(
                                heartbeat,
                                stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(
                            ex,
                            "VPN monitor error: {Tunnel}",
                            tunnel.TunnelName);

                        tunnel.MarkDisconnected();
                    }
                }

                await context.SaveChangesAsync(
                    stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogCritical(
                    ex,
                    "VPN monitoring worker crashed");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(30),
                stoppingToken);
        }
    }
}
