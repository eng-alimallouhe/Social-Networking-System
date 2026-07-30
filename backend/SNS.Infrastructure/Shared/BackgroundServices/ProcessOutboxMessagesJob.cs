using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SNS.Application.Shared.Events;
using SNS.Domain.Shared.Entities;
using SNS.Infrastructure.Persistence;
using System.Text.Json;

namespace SNS.Infrastructure.Shared.BackgroundServices;

public class ProcessOutboxMessagesJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ProcessOutboxMessagesJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SNSDbContext>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var messages = await dbContext.Set<OutboxMessage>()
                    .Where(m => m.ProcessedOnUtc == null)
                    .OrderBy(m => m.OccurredOnUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                if (messages.Any())
                {
                    foreach (var message in messages)
                    {
                        try
                        {
                            var eventType = Type.GetType(message.Type);
                            if (eventType == null)
                            {
                                message.MarkFailed(error: "Type not found.");
                                continue;
                            }

                            var domainEvent = JsonSerializer.Deserialize(message.Content, eventType);
                            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(eventType);
                            var notification = Activator.CreateInstance(notificationType, domainEvent);

                            await mediator.Publish((INotification)notification!, stoppingToken);

                            // Success
                            message.MarkProcessed();
                        }
                        catch (Exception ex)
                        {
                            // CRITICAL FIX: The message failed, but we MUST mark it processed 
                            // so the loop doesn't get stuck on it forever!
                            message.MarkFailed(error: ex.Message);
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                    await Task.Delay(200, stoppingToken);
                }
                else
                {
                    await Task.Delay(5000, stoppingToken);
                }
            }
            catch
            {
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
