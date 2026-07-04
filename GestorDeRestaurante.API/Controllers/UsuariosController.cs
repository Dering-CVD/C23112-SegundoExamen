using GestorDeRestaurante.Dominio.Dtos;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeRestaurante.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        public IServicioDeUsuario Usuarios { get; set; }

        public UsuariosController(IServicioDeUsuario usuarios)
        {
            Usuarios = usuarios;
        }

        [HttpGet("ListarUsuarios")]
        public async Task<IEnumerable<ListarUsuarioDto>> ListarUsuarios(string? filtroNombre = null)
        {
            return await Usuarios.ListarUsuariosAsync(filtroNombre);
        }

        [HttpGet("ObtenerUsuarioAEditar")]
        public async Task<EditarUsuarioDto?> ObtenerUsuarioAEditar(int id)
        {
            return await Usuarios.ObtenerUsuarioAEditarAsync(id);
        }

        [HttpPost("CrearUsuario")]
        public async Task<IActionResult> CrearUsuario(CrearUsuarioDto dto)
        {
            var resultado = await Usuarios.CrearUsuarioAsync(dto);
            return resultado.Exitoso ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
        }

        [HttpPut("EditarUsuario")]
        public async Task<IActionResult> EditarUsuario(int id, EditarUsuarioDto dto)
        {
            var resultado = await Usuarios.EditarUsuarioAsync(id, dto);
            return resultado.Exitoso ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
        }
    }
}