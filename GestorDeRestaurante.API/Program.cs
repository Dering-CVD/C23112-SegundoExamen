using SistemaAlquilerPlaya.AccesoADatos.Contexto;
using SistemaAlquilerPlaya.API;
using SistemaAlquilerPlaya.LogicaDeNegocios.Interfaces;
using SistemaAlquilerPlaya.LogicaDeNegocios.Servicios;
using Microsoft.EntityFrameworkCore;
using SistemaAlquilerPlaya.AccesoADatos.Interfaces;
using SistemaAlquilerPlaya.AccesoADatos.Repositorio;
using SistemaAlquilerPlaya.AccesoADatos.Servicios;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DbContexto>(options => options.UseSqlServer(connectionString));


builder.Services.AddScoped<IArticuloRepositorio, ArticuloRepositorio>();
builder.Services.AddScoped<IAlquilerRepositorio, AlquilerRepositorio>();
builder.Services.AddScoped<IArticuloBL, ArticuloBL>();
builder.Services.AddScoped<IAlquilerBL, AlquilerBL>();

builder.Services.AddControllers().AddJsonOptions(opciones => opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodos", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/openapi/v1.json", "API REST Gestor Restaurante");
    });
}

app.UseCors("PermitirTodos");
app.UseMiddleware<ApiKeyMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();