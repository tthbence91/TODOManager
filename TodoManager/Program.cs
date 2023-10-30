using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using TodoManager;
using TodoManager.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ValidationFilterAttribute>();

// Add configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);


// Set up CosmosDb Client
var cosmosDbSettings = builder.Configuration.GetSection("CosmosDbSettings").Get<CosmosDbSettings>();
builder.Services.AddSingleton(sp => new CosmosClient(cosmosDbSettings.Endpoint, cosmosDbSettings.MasterKey));
builder.Services.AddSingleton<ITodoRepository>(sp =>
{
    if (cosmosDbSettings != null)
        return new TodoRepository(
            sp.GetRequiredService<CosmosClient>(),
            cosmosDbSettings.DatabaseName,
            cosmosDbSettings.ContainerName
        );
    return null!;
});

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddControllers();
builder.Services.AddMvcCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers().WithOpenApi();

app.Run();
