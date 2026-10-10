using System;
using System.Collections.Generic;

namespace MiPrimerApi.Models;

public partial class Empleado
{
    public uint IdEmpleado { get; set; }

    public uint IdPuesto { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public virtual Puesto IdPuestoNavigation { get; set; } = null!;
}
