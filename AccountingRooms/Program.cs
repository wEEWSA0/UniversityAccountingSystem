using AccountingRooms.Data;
using AccountingRooms.RabbitMQ;
using AccountingRooms.Repository;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var defaultConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

var connectionString = defaultConnection is null ? builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.") : defaultConnection;

var services = builder.Services;

services.AddControllers();

services.AddNpgsql<ApplicationDbContext>(connectionString);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddHostedService<RabbitMqListener>();
services.AddScoped<RabbitMQHandler>();
services.AddScoped<RoomRepository>();
services.AddScoped<BuildingRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    dbContext.Database.Migrate();
}

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

//app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

app.Run();


/// Ctrl+C BuildingController Methods
/// Test it
/// Change e.delete to field IsActive = false
/// Add Rabbit mq:
/// 1. Send data
/// 2. Recive data
/// 3. Actions on recive
/// 4. Implement additional table to RoomDb "Building" which added values by rabbit mq