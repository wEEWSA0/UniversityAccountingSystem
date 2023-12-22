using AccountingBuildings.Data;
using AccountingBuildings.RabbitMQ;
using AccountingBuildings.Repository;

using Microsoft.EntityFrameworkCore;

using System;

var builder = WebApplication.CreateBuilder(args);

var defaultConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

var connectionString = defaultConnection is null ? builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.") : defaultConnection;

var services = builder.Services;

services.AddControllers();

services.AddNpgsql<ApplicationDbContext>(connectionString);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddScoped<BuildingRepository>();
services.AddScoped<IRabbitMQService, RabbitMQService>();

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

//app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

app.Run();
