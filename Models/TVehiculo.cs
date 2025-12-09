using System;
using System.Collections.Generic;

namespace ProyectoPrograAvanzada.Models;

public partial class TVehiculo
{
    internal string estado;

    public int IdVehiculo { get; set; }

    public string Placa { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public int Anio { get; set; }

    public string Estado { get; set; } = null!;

    public int IdTipo { get; set; }

    public int IdSucursal { get; set; }

    public virtual TSucursale IdSucursalNavigation { get; set; } = null!;

    public virtual TVehiculosTipo IdTipoNavigation { get; set; } = null!;

    public virtual ICollection<TAlquileresDetalle> TAlquileresDetalles { get; set; } = new List<TAlquileresDetalle>();
}
