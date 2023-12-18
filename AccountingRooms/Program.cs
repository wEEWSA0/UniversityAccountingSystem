using AccountingRooms.Data;
using AccountingRooms.Repository;

var builder = WebApplication.CreateBuilder(args);

var defaultConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

var connectionString = defaultConnection is null ? builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.") : defaultConnection;

var services = builder.Services;

services.AddControllers();

services.AddNpgsql<ApplicationDbContext>(connectionString);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddScoped<RoomRepository>();

var app = builder.Build();

//if (!app.Environment.IsDevelopment())
//{
//    app.UseHsts();
//}

app.UseSwagger();
app.UseSwaggerUI(o => 
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
    o.RoutePrefix = "";
});

Console.WriteLine("Ia wrode rabotau");

//app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

app.Run();
