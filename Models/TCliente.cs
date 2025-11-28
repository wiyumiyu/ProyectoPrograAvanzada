using System;
using System.Collections.Generic;

namespace ProyectoPrograAvanzada.Models;

public partial class TCliente
{
    public int IdCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? Direccion { get; set; }

    public virtual ICollection<TAlquilere> TAlquileres { get; set; } = new List<TAlquilere>();
}
