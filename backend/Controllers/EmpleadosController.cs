using Microsoft.AspNetCore.Mvc;
using MiPrimerApi.Dtos;
using MiPrimerApi.Models;

namespace MiPrimerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly DbEmpresaContext _context;

        public EmpleadosController(DbEmpresaContext ctx)
        {
            _context = ctx;
        }

        // SELECT e.*, p.nombre FROM empleados e JOIN puestos p ON e.id_puesto = p.id_puesto
        [HttpGet("ListarEmpleados")]
        public ActionResult<IEnumerable<object>> ListarEmpleados()
        {
            var empleados = _context.Empleados
                .Select(e => new
                {
                    e.IdEmpleado,
                    e.Nombre,
                    e.Apellido,
                    e.IdPuesto,
                    Puesto = e.IdPuestoNavigation.Nombre
                })
                .ToList();

            return empleados;
        }

        // SELECT ... WHERE id_empleado = {id}
        [HttpGet("ObtenerEmpleado/{id}")]
        public ActionResult<object> ObtenerEmpleado(uint id)
        {
            var empleado = _context.Empleados
                .Where(e => e.IdEmpleado == id)
                .Select(e => new
                {
                    e.IdEmpleado,
                    e.Nombre,
                    e.Apellido,
                    e.IdPuesto,
                    Puesto = e.IdPuestoNavigation.Nombre
                })
                .FirstOrDefault();

            if (empleado == null)
            {
                return NotFound($"No existe un empleado con id {id}");
            }

            return empleado;
        }

        // INSERT INTO empleados
        [HttpPost("CrearEmpleado")]
        public ActionResult CrearEmpleado(EmpleadoDto dto)
        {
            if (!_context.Puestos.Any(p => p.IdPuesto == dto.IdPuesto))
            {
                return BadRequest($"No existe un puesto con id {dto.IdPuesto}");
            }

            var empleado = new Empleado
            {
                IdPuesto = dto.IdPuesto,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido
            };

            _context.Empleados.Add(empleado);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObtenerEmpleado), new { id = empleado.IdEmpleado },
                new { empleado.IdEmpleado, empleado.Nombre, empleado.Apellido, empleado.IdPuesto });
        }

        // UPDATE empleados SET ... WHERE id_empleado = {id}
        [HttpPut("ActualizarEmpleado/{id}")]
        public ActionResult ActualizarEmpleado(uint id, EmpleadoDto dto)
        {
            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound($"No existe un empleado con id {id}");
            }

            if (!_context.Puestos.Any(p => p.IdPuesto == dto.IdPuesto))
            {
                return BadRequest($"No existe un puesto con id {dto.IdPuesto}");
            }

            empleado.IdPuesto = dto.IdPuesto;
            empleado.Nombre = dto.Nombre;
            empleado.Apellido = dto.Apellido;
            _context.SaveChanges();

            return Ok(new { empleado.IdEmpleado, empleado.Nombre, empleado.Apellido, empleado.IdPuesto });
        }

        // DELETE FROM empleados WHERE id_empleado = {id}
        [HttpDelete("EliminarEmpleado/{id}")]
        public ActionResult EliminarEmpleado(uint id)
        {
            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound($"No existe un empleado con id {id}");
            }

            _context.Empleados.Remove(empleado);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
