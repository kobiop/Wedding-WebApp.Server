
using WeddingUI.Server.Models;
using WeddingUI.Server.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

Env.Load();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDBSettings>(options =>
{
    // Retrieve the MongoDB connection string and database name from environment variables
    options.connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
    options.databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");
});
builder.Services.AddSingleton<PhotoService>();
builder.Services.AddSingleton<CounterService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://thankful-plant-0caf1081e.4.azurestaticapps.net") // Frontend origin
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
