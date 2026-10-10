using Microsoft.AspNetCore.Mvc;
using MiPrimerApi.Dtos;
using MiPrimerApi.Models;

namespace MiPrimerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly DbEmpresaContext _context;

        public ClientesController(DbEmpresaContext ctx)
        {
            _context = ctx;
        }

        // SELECT * FROM clientes
        [HttpGet("ListarClientes")]
        public ActionResult<List<Cliente>> ListarClientes()
        {
            return _context.Clientes.ToList();
        }

        // SELECT * FROM clientes WHERE id_cliente = {id}
        [HttpGet("ObtenerCliente/{id}")]
        public ActionResult<Cliente> ObtenerCliente(uint id)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == id);
            if (cliente == null)
            {
                return NotFound($"No existe un cleinte con id {id}");
            }

            return cliente;
        }

        // INSERT INTO clientes
        [HttpPost("CrearCliente")]
        public ActionResult<Cliente> CrearCliente(ClienteDto dto)
        {
            var cliente = new Cliente
            {
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                Telefono = dto.Telefono
            };

            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObtenerCliente), new { id = cliente.IdCliente }, cliente);
        }

        // UPDATE clientes SET ... WHERE id_cliente = {id}
        [HttpPut("ActualizarCliente/{id}")]
        public ActionResult<Cliente> ActualizarCliente(uint id, ClienteDto dto)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == id);
            if (cliente == null)
            {
                return NotFound($"No existe un cliente con id {id}");
            }

            cliente.Nombre = dto.Nombre;
            cliente.Apellidos = dto.Apellidos;
            cliente.Telefono = dto.Telefono;
            _context.SaveChanges();

            return cliente;
        }

        // DELETE FROM clientes WHERE id_cliente = {id}
        [HttpDelete("EliminarCliente/{id}")]
        public ActionResult EliminarCliente(uint id)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == id);
            if (cliente == null)
            {
                return NotFound($"No existe un cliente con id {id}");
            }

            _context.Clientes.Remove(cliente);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
