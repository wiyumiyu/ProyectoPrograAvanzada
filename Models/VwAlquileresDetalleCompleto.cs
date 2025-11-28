using System;
using System.Collections.Generic;

namespace ProyectoPrograAvanzada.Models;

public partial class VwAlquileresDetalleCompleto
{
    public int IdAlquiler { get; set; }

    public string Cliente { get; set; } = null!;

    public string Placa { get; set; } = null!;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public decimal Subtotal { get; set; }
}
