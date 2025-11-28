using System;
using System.Collections.Generic;

namespace ProyectoPrograAvanzada.Models;

public partial class TRole
{
    public int IdRol { get; set; }

    public string Rol { get; set; } = null!;

    public virtual ICollection<TEmpleado> TEmpleados { get; set; } = new List<TEmpleado>();
}
