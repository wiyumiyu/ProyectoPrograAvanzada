using System;
using System.Collections.Generic;

namespace ProyectoPrograAvanzada.Models;

public partial class TSucursale
{
    public int IdSucursal { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Telefono { get; set; }

    public string Direccion { get; set; } = null!;

    public virtual ICollection<TAlquilere> TAlquileres { get; set; } = new List<TAlquilere>();

    public virtual ICollection<TEmpleado> TEmpleados { get; set; } = new List<TEmpleado>();

    public virtual ICollection<TVehiculo> TVehiculos { get; set; } = new List<TVehiculo>();
}
