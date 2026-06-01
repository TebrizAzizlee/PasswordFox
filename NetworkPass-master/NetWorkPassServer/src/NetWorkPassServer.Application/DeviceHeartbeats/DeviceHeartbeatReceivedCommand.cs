
using GenericRepository;
using NetWorkPassServer.Application.Alerts;
using NetWorkPassServer.Application.Services;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.DeviceHeartbeats;
using NetWorkPassServer.Domain.DeviceMetricss;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.DeviceHeartbeats;

public sealed record DeviceHeartbeatReceivedCommand(
    Guid DeviceId,
    bool IsReachable,
    string? ErrorMessage,
    double? CpuUsage,
    double? DiskUsage,
    double? MemoryUsage,
    double? Temperature,
    long? UptimeSeconds,
    long? ResponseTimeMs
) : IRequest<ServiceResult>;

internal sealed class DeviceHeartbeatReceivedCommandHandler(
    IDeviceRepository deviceRepository,
    IDeviceHeartbeatRepository heartbeatRepository,
    IDeviceMetricRepository metricRepository,
    IBranchStatsService branchStats,
    IAlertService alertService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        DeviceHeartbeatReceivedCommand,
        ServiceResult>
{
    public async Task<ServiceResult> Handle(
        DeviceHeartbeatReceivedCommand request,
        CancellationToken cancellationToken)
    {
        var device =
            await deviceRepository
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.DeviceId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (device is null)
        {
            return ServiceResult.Failure(
                "Tapılmadı",
                "Device tapılmadı",
                HttpStatusCode.NotFound);
        }

        var utcNow =
            DateTime.UtcNow;

        var oldStatus =
            device.Status;

        // =====================================================
        // METRIC CHECK
        // =====================================================

        var hasMetrics =
            request.CpuUsage.HasValue ||
            request.MemoryUsage.HasValue ||
            request.DiskUsage.HasValue ||
            request.Temperature.HasValue;

        // =====================================================
        // HEARTBEAT STATUS
        // =====================================================

        if (request.IsReachable)
        {
            device.MarkHeartbeatSuccess(
                request.ResponseTimeMs,
                utcNow);

            // yalnız metric varsa update et

            if (hasMetrics)
            {
                device.UpdateMetrics(
                    request.CpuUsage,
                    request.MemoryUsage,
                    request.Temperature,
                    request.UptimeSeconds ?? 0);

                device.EvaluateHealthStatus(
                    utcNow);
                await unitOfWork.SaveChangesAsync(
                        cancellationToken);
                await unitOfWork.SaveChangesAsync(
                  cancellationToken);
            }
        }
        else
        {
            device.MarkHeartbeatFailure(
                utcNow);
        }

        // =====================================================
        // HEARTBEAT HISTORY
        // =====================================================

        var heartbeat =
            new DeviceHeartbeat(
                request.DeviceId,
                device.Status,
                request.IsReachable,
                request.ResponseTimeMs,
                utcNow,
                request.ErrorMessage);

        await heartbeatRepository.AddAsync(
            heartbeat,
            cancellationToken);

        // =====================================================
        // METRIC HISTORY
        // =====================================================

        if (request.IsReachable &&
            hasMetrics)
        {
            var latestMetric =
                await metricRepository
                    .GetLatestAsync(
                        device.Id,
                        cancellationToken);

            var shouldPersistMetric =
                latestMetric is null ||

                Math.Abs(
                    (latestMetric.CpuUsage ?? 0) -
                    (request.CpuUsage ?? 0)) >= 5 ||

                Math.Abs(
                    (latestMetric.MemoryUsage ?? 0) -
                    (request.MemoryUsage ?? 0)) >= 5 ||

                Math.Abs(
                    (latestMetric.Temperature ?? 0) -
                    (request.Temperature ?? 0)) >= 3 ||

                utcNow -
                latestMetric.OccurredAtUtc >=
                TimeSpan.FromMinutes(1);

            if (shouldPersistMetric)
            {
                var metric =
                    DeviceMetric.Create(
                        device.Id,
                        utcNow,
                        request.CpuUsage,
                        request.MemoryUsage,
                        request.DiskUsage,
                        request.Temperature,
                        request.UptimeSeconds,
                        request.ResponseTimeMs);

                await metricRepository.AddAsync(
                    metric,
                    cancellationToken);
            }
        }

        // =====================================================
        // CPU ALERT
        // =====================================================

        if (request.CpuUsage.HasValue &&
            request.CpuUsage.Value >= 90)
        {
            await alertService.ProcessAsync(
                new AlertContext
                {
                    Device = device,

                    Type =
                        AlertType.HighCpuUsage,

                    Severity =
                        AlertSeverity.Warning,

                    Source =
                        AlertSource.System,

                    Title =
                        "High CPU Usage",

                    Message =
                        $"{device.Name.Value} CPU usage yüksəkdir ({request.CpuUsage}%)",

                    Fingerprint =
                        $"device:{device.Id}:high-cpu"
                },
                cancellationToken);
        }
        else if (
            request.CpuUsage.HasValue &&
            request.CpuUsage.Value < 80)
        {
            await alertService.ResolveAsync(
                $"device:{device.Id}:high-cpu",
                cancellationToken);
        }

        // =====================================================
        // MEMORY ALERT
        // =====================================================

        if (request.MemoryUsage.HasValue &&
            request.MemoryUsage.Value >= 90)
        {
            await alertService.ProcessAsync(
                new AlertContext
                {
                    Device = device,

                    Type =
                        AlertType.HighMemoryUsage,

                    Severity =
                        AlertSeverity.Warning,

                    Source =
                        AlertSource.System,

                    Title =
                        "High Memory Usage",

                    Message =
                        $"{device.Name.Value} memory usage yüksəkdir ({request.MemoryUsage}%)",

                    Fingerprint =
                        $"device:{device.Id}:high-memory"
                },
                cancellationToken);
        }
        else if (
            request.MemoryUsage.HasValue &&
            request.MemoryUsage.Value < 80)
        {
            await alertService.ResolveAsync(
                $"device:{device.Id}:high-memory",
                cancellationToken);
        }

        // =====================================================
        // TEMPERATURE ALERT
        // =====================================================

        if (request.Temperature.HasValue &&
            request.Temperature.Value >= 80)
        {
            await alertService.ProcessAsync(
                new AlertContext
                {
                    Device = device,

                    Type =
                        AlertType.HighTemperature,

                    Severity =
                        AlertSeverity.Critical,

                    Source =
                        AlertSource.System,

                    Title =
                        "High Temperature",

                    Message =
                        $"{device.Name.Value} yüksək temperaturdadır ({request.Temperature}°C)",

                    Fingerprint =
                        $"device:{device.Id}:high-temperature"
                },
                cancellationToken);
        }
        else if (
            request.Temperature.HasValue &&
            request.Temperature.Value < 70)
        {
            await alertService.ResolveAsync(
                $"device:{device.Id}:high-temperature",
                cancellationToken);
        }

        // =====================================================
        // OFFLINE ALERTS
        // =====================================================
        if (!request.IsReachable)
        {
            await alertService.ProcessAsync(
                new AlertContext
                {
                    Device = device,

                    Type =
                        AlertType.DeviceOffline,

                    Severity =
                        AlertSeverity.Critical,

                    Source =
                        AlertSource.System,

                    Title =
                        "Device Offline",

                    Message =
                        $"{device.Name.Value} offline oldu",

                    Fingerprint =
                        $"device:{device.Id}:offline"
                },
                cancellationToken);
        }


        //OFFLINE RECOVERY

        if (oldStatus ==DeviceStatus.Offline && device.Status ==  DeviceStatus.Online)



        {
                await alertService.ResolveAsync(
                    $"device:{device.Id}:offline",
                    cancellationToken);
            }

        if (oldStatus != device.Status)
        {
            await branchStats.RecalculateAsync(
                device.BranchId,
                cancellationToken);
        }
        await unitOfWork.SaveChangesAsync(
          cancellationToken);

        return ServiceResult.Success();
    }

        // =====================================================
        // COMMIT
        // =====================================================

      
    }

