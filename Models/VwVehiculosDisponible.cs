using System;
using System.Collections.Generic;

namespace ProyectoPrograAvanzada.Models;

public partial class VwVehiculosDisponible
{
    public int IdVehiculo { get; set; }

    public string Placa { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string Sucursal { get; set; } = null!;
}
