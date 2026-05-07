using AuthServer.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Services;
public sealed class RefreshTokenCleanupService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
			try
			{
				using var scope = scopeFactory.CreateScope();
				var context=scope.ServiceProvider.GetRequiredService<AuthServerDbContext>();
				var cutoff = DateTimeOffset.UtcNow.AddDays(-30);
				await context.Set<LoginToken>()
					.Where(x=>x.RevokedAt!=null && x.ExpiresAt<cutoff).ExecuteDeleteAsync(stoppingToken);
			}
			catch (Exception ex)
			{

                Console.WriteLine(
                  $"TOKEN CLEANUP ERROR: {ex}");
            }
            await Task.Delay(
                TimeSpan.FromHours(24),
                stoppingToken);
        }
    }
}

