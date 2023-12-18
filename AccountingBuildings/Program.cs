using AccountingBuildings.Data;
using AccountingBuildings.Repository;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var defaultConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

var connectionString = defaultConnection is null ? builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.") : defaultConnection;

var services = builder.Services;

services.AddControllers(); // todo перепроверить, приступить ко второму микро-сервису

services.AddNpgsql<ApplicationDbContext>(connectionString);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddScoped<BuildingRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

app.Run();
