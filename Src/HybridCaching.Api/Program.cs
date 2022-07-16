using EasyCaching.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEasyCaching(options =>
{
    options.WithJson("json-serializer");
    options.UseInMemory(memoryOptions => { memoryOptions.EnableLogging = true; }, "m");
    options.UseRedis(redisOptions =>
    {
        redisOptions.EnableLogging = true;
        redisOptions.DBConfig = new RedisDBOptions
                                {
                                    Configuration = builder.Configuration.GetConnectionString("Redis")
                                };
        redisOptions.SerializerName = "json-serializer";
    }, "r");

    options.UseHybrid(hybridCachingOptions =>
            {
                hybridCachingOptions.EnableLogging = true;
                hybridCachingOptions.TopicName = "hybrid-caching-api--topic";
                hybridCachingOptions.LocalCacheProviderName = "m";
                hybridCachingOptions.DistributedCacheProviderName = "r";
            })
           .WithRedisBus(busOptions =>
            {
                busOptions.Configuration = builder.Configuration.GetConnectionString("Redis");
                busOptions.SerializerName = "json-serializer";
            });
});

var app = builder.Build();

app.UseForwardedHeaders();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();