using System;
using System.Collections.Generic;

namespace MiPrimerApi.Models;

public partial class Puesto
{
    public uint IdPuesto { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
