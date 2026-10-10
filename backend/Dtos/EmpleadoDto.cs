namespace MiPrimerApi.Dtos;

// Datos que se reciben para crear o actualizar un empleado
public class EmpleadoDto
{
    public uint IdPuesto { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;
}
