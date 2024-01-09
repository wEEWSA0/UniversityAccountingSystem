using AccountingBuildings.Data;
using AccountingBuildings.RabbitMQ;
using AccountingBuildings.Repository;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

ConfigureServices(builder.Configuration);

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddScoped<BuildingRepository>();

services.AddScoped<IRabbitMQService, RabbitMQService>();

var app = builder.Build();

MigrateDatabase(app.Services);

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
    o.RoutePrefix = "";
});

app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

app.UseRouting();
app.MapControllers();

app.Run();

void MigrateDatabase(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var dbContext = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
}

void ConfigureServices(ConfigurationManager configuration)
{
    #region ConfigureDatabase
    var defaultConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

    var connectionString = defaultConnection is null ? configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.") : defaultConnection;

    services.AddNpgsql<ApplicationDbContext>(connectionString);
    #endregion

    #region ConfigureRabbitMQ
    var rabbitHost = Environment.GetEnvironmentVariable("RabbitMQ__Host");
    var rabbitPort = Environment.GetEnvironmentVariable("RabbitMQ__Port");
    var rabbitUserName = Environment.GetEnvironmentVariable("RabbitMQ__UserName");
    var rabbitPassword = Environment.GetEnvironmentVariable("RabbitMQ__Password");
    var rabbitQueue = Environment.GetEnvironmentVariable("RabbitMQ__Queue");

    services.Configure<RabbitMQOptions>(options =>
    {
        options.HostName = rabbitHost ?? configuration.GetValue<string>("RabbitMQ:Host");
        options.Port = rabbitPort is null ? configuration.GetValue<int>("RabbitMQ:Port") : int.Parse(rabbitPort!);
        options.UserName = rabbitUserName ?? configuration.GetValue<string>("RabbitMQ:UserName");
        options.Password = rabbitPassword ?? configuration.GetValue<string>("RabbitMQ:Password");
        options.Queue = rabbitQueue ?? configuration.GetValue<string>("RabbitMQ:Queue");
    });
    #endregion
}