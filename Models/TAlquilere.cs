using System;
using System.Collections.Generic;

namespace ProyectoPrograAvanzada.Models;

public partial class TAlquilere
{
    public int IdAlquiler { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public decimal Iva { get; set; }

    public int IdCliente { get; set; }

    public int IdEmpleado { get; set; }

    public int IdSucursal { get; set; }

    public string Estado { get; set; } = null!;

    public virtual TCliente IdClienteNavigation { get; set; } = null!;

    public virtual TEmpleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual TSucursale IdSucursalNavigation { get; set; } = null!;

    public virtual ICollection<TAlquileresDetalle> TAlquileresDetalles { get; set; } = new List<TAlquileresDetalle>();

    public virtual ICollection<TRecibo> IdRecibos { get; set; } = new List<TRecibo>();
}
