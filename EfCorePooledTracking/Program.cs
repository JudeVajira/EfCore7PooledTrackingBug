using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContextPool<BlogDbContext>(opt =>
        {
            opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            opt.UseInMemoryDatabase("blog");
        });
        services.AddLogging(conf => conf.SetMinimumLevel(LogLevel.Error));
    })
    .Build();

QueryTrackingBehavior trackingOne, trackingTwo;

await using (var scope = host.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();

    trackingOne = dbContext.ChangeTracker.QueryTrackingBehavior;
}

await using (var scope = host.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();

    trackingTwo = dbContext.ChangeTracker.QueryTrackingBehavior;
}

if (trackingOne == trackingTwo)
{
    Console.WriteLine($"Tracking resets properly in EFCore 6: {trackingOne}");
}
else
{
    Console.WriteLine($"TrackAll in EFCore 7. [trackingOne: {trackingOne}, trackingTwo: {trackingTwo}]");
}

await host.RunAsync();

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {
    }
}