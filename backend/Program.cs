using MiPrimerApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbEmpresaContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("conexion"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("conexion")));
});

//para qeu conecte
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // En desarrollo no se redirige a HTTPS para que el frontend pueda llamar a http://localhost:5149
    app.UseHttpsRedirection();
}

app.UseCors("frontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
