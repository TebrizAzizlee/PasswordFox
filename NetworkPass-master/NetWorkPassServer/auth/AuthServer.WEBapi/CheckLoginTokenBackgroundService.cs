using AuthServer.Domain.LoginTokens;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.WEBapi;

public sealed class CheckLoginTokenBackgroundService(IServiceScopeFactory serviceScope, ILogger<CheckLoginTokenBackgroundService> logger) : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    logger.LogInformation("Token cleanup started");

    while (!stoppingToken.IsCancellationRequested)
    {
        try
        {
            using var scope = serviceScope.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ILoginTokenRepository>();
                var affected = await repo.DeactivateExpiredTokensAsync(stoppingToken);
                

            if (affected > 0)
            {
                logger.LogInformation("{count} expired tokens deactivated", affected);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during token cleanup");
        }

        await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
    }
}
}
