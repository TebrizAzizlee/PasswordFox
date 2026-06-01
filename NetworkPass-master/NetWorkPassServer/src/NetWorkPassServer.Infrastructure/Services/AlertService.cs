
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Alerts;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Services;
using NetWorkPassServer.Domain.Alerts;




namespace NetWorkPassServer.Infrastructure.Services;

internal sealed class AlertService(
    IPasswordDbContext context
    
   )
    : IAlertService
{

    public async Task ProcessAsync(AlertContext contextData, CancellationToken cancellationToken)
    {
        //existing open alert
        var existingAlert =
             await context.Alerts.FirstOrDefaultAsync(x => !x.IsDeleted &&
             x.Fingerprint==contextData.Fingerprint &&
             x.Status!=AlertStatus.Resolved, cancellationToken);
        Console.WriteLine(
    $"Fingerprint: {contextData.Fingerprint}");

        Console.WriteLine(
            $"Found Alert: {existingAlert != null}");
        //dublicate
        if (existingAlert is not null)
        {
            existingAlert.IncrementOccurrence();
            await context.SaveChangesAsync(
    cancellationToken);
            return;
        }
        //create new alert

        var alert = new Alert(
            contextData.Device.Id,
            contextData.Device.BranchId,
            contextData.Type,
            contextData.Severity,
            contextData.Source,
            contextData.Message,
            contextData.Title,
            DateTime.UtcNow,
            contextData.Fingerprint
            );
        await context.Alerts.AddAsync(alert,cancellationToken);
        await context.SaveChangesAsync(
     cancellationToken);

    }

    public async Task ResolveAsync(
       string fingerprint,
       CancellationToken cancellationToken)
    {
        
        var alert =
            await context.Alerts
                .FirstOrDefaultAsync(
                    x =>
                        !x.IsDeleted &&
                        x.Fingerprint ==
                            fingerprint &&
                        x.Status !=
                            AlertStatus.Resolved,
                    cancellationToken);
       
        if (alert is null)
        {
            return;
        }
       
        alert.Resolve(
            null,
            "Automatically resolved");
        Console.WriteLine(alert.Status);
        Console.WriteLine(alert.ResolvedAt);
        await context.SaveChangesAsync(
    cancellationToken);
    }
}


