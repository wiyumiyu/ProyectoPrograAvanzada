using System;
using System.Collections.Generic;

namespace ProyectoPrograAvanzada.Models;

public partial class TRecibo
{
    public int IdRecibo { get; set; }

    public DateOnly FechaPago { get; set; }

    public decimal Monto { get; set; }

    public string Metodo { get; set; } = null!;

    public virtual ICollection<TAlquilere> IdAlquilers { get; set; } = new List<TAlquilere>();
}
