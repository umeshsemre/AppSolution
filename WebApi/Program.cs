using Application.Interfaces.Product;
using FluentMigrator.Runner;
using Persistence.Interfaces.Product;
using Persistence.Interfaces;
using Persistence;
using Persistence.Migrations;
using Persistence.Services.Product;
using Persistence.Services;
using Application.Services.Product;
using Persistence.Interfaces.Categories;
using Application.Interfaces.Categories;
using Application.Services.Categories;
using Persistence.Services.Categories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 1. Add FluentMigrator to DI
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer() // Use .AddPostgres() or .AddMySql() for other DBs
        .WithGlobalConnectionString(
            builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(typeof(InitialProductTables).Assembly).For.Migrations()
    )
    .AddLogging(lb => lb.AddFluentMigratorConsole());

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryServices, CategoryServices>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp(); // This runs the Up() methods
}
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       //  Enable Swagger middleware
    // Enable Swagger UI
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // This makes Swagger UI the default page
    });
}


app.UseHttpsRedirection();
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
