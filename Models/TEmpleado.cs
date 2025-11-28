using System;
using System.Collections.Generic;

namespace ProyectoPrograAvanzada.Models;

public partial class TEmpleado
{
    public int IdEmpleado { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public string Contrasena { get; set; } = null!;

    public string Puesto { get; set; } = null!;

    public int IdSucursal { get; set; }

    public int IdRol { get; set; }

    public virtual TRole IdRolNavigation { get; set; } = null!;

    public virtual TSucursale IdSucursalNavigation { get; set; } = null!;

    public virtual ICollection<TAlquilere> TAlquileres { get; set; } = new List<TAlquilere>();
}
