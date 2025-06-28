using NetArchTestSample.Data.Repositories;
using NetArchTestSample.Domain.Interfaces;
using NetArchTestSample.Infrastructure.Services;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Book Library API", 
        Version = "v1",
        Description = "A sample API for managing a book library using Clean Architecture"
    });
    
    // Enable XML comments for better API documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register dependencies
builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();
builder.Services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
builder.Services.AddScoped<IBookService, BookService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Library API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Seed some sample data
await SeedSampleData(app.Services);

app.Run();

static async Task SeedSampleData(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Seeding sample data...");
        
        await bookService.CreateBookAsync(
            "Clean Code", 
            "Robert C. Martin", 
            "978-0132350884", 
            new DateTime(2008, 8, 1), 
            464);
            
        await bookService.CreateBookAsync(
            "The Pragmatic Programmer", 
            "Dave Thomas and Andy Hunt", 
            "978-0201616224", 
            new DateTime(1999, 10, 20), 
            352);
            
        await bookService.CreateBookAsync(
            "Design Patterns", 
            "Gang of Four", 
            "978-0201633610", 
            new DateTime(1994, 10, 31), 
            395);
            
        logger.LogInformation("Sample data seeded successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to seed sample data");
    }
}
