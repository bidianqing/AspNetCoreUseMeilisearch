using AspNetCoreUseMeilisearch;
using Domain.Interfaces;
using Infrastructure;
using Meilisearch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IMeilisearchService, MeilisearchService>();

builder.Services.AddTransient<CustomLoggingHttpMessageHandler>();
builder.Services.AddHttpClient("meilisearch", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MeilisearchOptions:BaseAddress"]);
}).AddHttpMessageHandler<CustomLoggingHttpMessageHandler>();

builder.Services.AddScoped(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("meilisearch");

    return new MeilisearchClient(httpClient);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
