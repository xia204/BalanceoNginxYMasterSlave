using Microsoft.EntityFrameworkCore;
using Uttt.Micro.Libro.Extensiones;
using Uttt.Micro.Libro.Persistencia;

var builder = WebApplication.CreateBuilder(args);

// Correcci�n: Usa GetConnectionString para obtener las cadenas de conexi�n
var writeConnection = builder.Configuration.GetConnectionString("MasterConnection");
var readConnection = builder.Configuration.GetConnectionString("SlaveConnection");

// Validaci�n opcional
if (string.IsNullOrEmpty(writeConnection))
    throw new InvalidOperationException("MasterConnection string is missing or null");
if (string.IsNullOrEmpty(readConnection))
    throw new InvalidOperationException("SlaveConnection string is missing or null");

Console.WriteLine($"MasterConnection: {writeConnection}");
Console.WriteLine($"SlaveConnection: {readConnection}");

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomServices(builder.Configuration, writeConnection, readConnection);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Habilitar Swagger en todos los entornos para Docker
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();
app.Run();
