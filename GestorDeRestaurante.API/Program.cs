using GestorDeRestaurante.AccesoADatos.Contexto;
using GestorDeRestaurante.API;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using GestorDeRestaurante.LogicaDeNegocios.Servicios;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DbContexto>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IServicioDeAutenticacion, ServicioDeAutenticacion>();
builder.Services.AddScoped<IServicioDeMenu, ServicioDeMenu>();
builder.Services.AddScoped<IServicioDeRecetas, ServicioDeRecetas>();
builder.Services.AddScoped<IServicioDeIngrediente, ServicioDeIngrediente>();
builder.Services.AddScoped<IServicioDeFacturacion, ServicioDeFacturacion>();
builder.Services.AddScoped<IServicioDeUsuario, ServicioDeUsuario>();
builder.Services.AddScoped<IServicioDeCocina, ServicioDeCocina>();
builder.Services.AddTransient<IServicioDeEmail, ServicioDeEmail>();
builder.Services.AddScoped<IServicioDePedido, ServicioDePedido>();

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