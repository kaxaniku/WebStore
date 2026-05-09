using Hangfire;
using MassTransit;
using Webstore.CatalogInfrastructure.Repositories;
using WebStore.Contracts.Catalog.Category;

namespace WebStore.BackWorker.Consumers;

public class CategoryCreatedConsumer : IConsumer<CategoryCreated>
{
    private readonly IBackgroundJobClient _hangfire;
    private readonly ILogger<CategoryCreatedConsumer> _logger;
    private readonly CatalogDbContext _db;

    public CategoryCreatedConsumer(IBackgroundJobClient hangfire, ILogger<CategoryCreatedConsumer> logger, CatalogDbContext db)
    {
        _hangfire = hangfire;
        _logger = logger;
        _db = db;
    }

    public async Task Consume(ConsumeContext<CategoryCreated> context)
    {
        var message = context.Message;
        _logger.LogInformation("Message received for Category: {Id}. Scheduling background job...", message.Id);

        _hangfire.Schedule<CategoryCreatedConsumer>(
            x => x.ProcessCategoryAsync(message.Id),
            TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    [Queue("default")]
    public async Task ProcessCategoryAsync(int id)
    {
        _logger.LogInformation("[Hangfire Job] Starting database work for Category {Id}", id);

        await Task.Delay(1000);
        _logger.LogInformation("[Hangfire Job] Successfully processed Category {Id}", id);
    }
}