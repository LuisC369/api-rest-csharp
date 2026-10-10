namespace MiPrimerApi.Dtos;

// Datos que se reciben para crear o actualizar un cliente
public class ClienteDto
{
    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Telefono { get; set; } = null!;
}
