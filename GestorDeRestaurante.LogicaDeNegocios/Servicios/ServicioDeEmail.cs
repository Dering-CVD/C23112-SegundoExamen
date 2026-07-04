using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace GestorDeRestaurante.LogicaDeNegocios.Servicios;

public class ServicioDeEmail : IServicioDeEmail
{
    private readonly IConfiguration configuracion;
    public ServicioDeEmail(IConfiguration configuracion)
    {
        this.configuracion = configuracion;
    }
    public async Task EnviarEmail(string emailReceptor, string asunto, string cuerpo)
    {
        var emailEmisor = configuracion.GetValue<string>("CONFIGURACION_EMAIL:EMAIL");
        var contrasena = configuracion.GetValue<string>("CONFIGURACION_EMAIL:CONTRASENA");
        var host = configuracion.GetValue<string>("CONFIGURACION_EMAIL:HOST");
        var puerto = configuracion.GetValue<int>("CONFIGURACION_EMAIL:PUERTO");

        using var smtpCliente = new SmtpClient(host, puerto);

        smtpCliente.EnableSsl = true;
        smtpCliente.UseDefaultCredentials = false;
        smtpCliente.Credentials = new System.Net.NetworkCredential(emailEmisor, contrasena);

        var mensaje = new MailMessage(emailEmisor!, emailReceptor, asunto, cuerpo);

        await smtpCliente.SendMailAsync(mensaje);
    }
}
