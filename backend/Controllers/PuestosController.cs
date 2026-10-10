using Microsoft.AspNetCore.Mvc;
using MiPrimerApi.Dtos;
using MiPrimerApi.Models;

namespace MiPrimerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PuestosController : ControllerBase
    {
        private readonly DbEmpresaContext _context;

        public PuestosController(DbEmpresaContext ctx)
        {
            _context = ctx;
        }

        // SELECT * FROM puestos
        [HttpGet("ListarPuestos")]
        public ActionResult<List<Puesto>> ListarPuestos()
        {
            return _context.Puestos.ToList();
        }

        // SELECT * FROM puestos WHERE id_puesto = {id}
        [HttpGet("ObtenerPuesto/{id}")]
        public ActionResult<Puesto> ObtenerPuesto(uint id)
        {
            var puesto = _context.Puestos.FirstOrDefault(p => p.IdPuesto == id);
            if (puesto == null)
            {
                return NotFound($"No existe un puesto con id {id}");
            }

            return puesto;
        }

        // INSERT INTO puestos
        [HttpPost("CrearPuesto")]
        public ActionResult<Puesto> CrearPuesto(PuestoDto dto)
        {
            var puesto = new Puesto { Nombre = dto.Nombre };

            _context.Puestos.Add(puesto);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObtenerPuesto), new { id = puesto.IdPuesto }, puesto);
        }

        // UPDATE puestos SET ... WHERE id_puesto = {id}
        [HttpPut("ActualizarPuesto/{id}")]
        public ActionResult<Puesto> ActualizarPuesto(uint id, PuestoDto dto)
        {
            var puesto = _context.Puestos.FirstOrDefault(p => p.IdPuesto == id);
            if (puesto == null)
            {
                return NotFound($"No existe un puesto con id {id}");
            }

            puesto.Nombre = dto.Nombre;
            _context.SaveChanges();

            return puesto;
        }

        // DELETE FROM puestos WHERE id_puesto = {id}
        [HttpDelete("EliminarPuesto/{id}")]
        public ActionResult EliminarPuesto(uint id)
        {
            var puesto = _context.Puestos.FirstOrDefault(p => p.IdPuesto == id);
            if (puesto == null)
            {
                return NotFound($"No existe un pueto con id {id}");
            }

            // La llave foránea FK_empleados_puestos no permite borrar un puesto en uso
            if (_context.Empleados.Any(e => e.IdPuesto == id))
            {
                return Conflict("No se puede eliminar: hay empleados asignados a este puesto");
            }

            _context.Puestos.Remove(puesto);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
