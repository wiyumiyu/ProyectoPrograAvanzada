using System;
using System.Collections.Generic;

namespace ProyectoPrograAvanzada.Models;

public partial class TAlquileresDetalle
{
    public int IdDetalle { get; set; }

    public int IdAlquiler { get; set; }

    public int IdVehiculo { get; set; }

    public decimal TarifaDiaria { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public decimal Subtotal { get; set; }

    public virtual TAlquilere IdAlquilerNavigation { get; set; } = null!;

    public virtual TVehiculo IdVehiculoNavigation { get; set; } = null!;
    
}
