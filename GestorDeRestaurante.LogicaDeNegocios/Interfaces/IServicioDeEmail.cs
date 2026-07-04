namespace GestorDeRestaurante.LogicaDeNegocios.Interfaces;

public interface IServicioDeEmail
{
    Task EnviarEmail(string emailReceptor, string asunto, string cuerpo);
}
