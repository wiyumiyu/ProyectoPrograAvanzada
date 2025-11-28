using System;
using System.Collections.Generic;

namespace ProyectoPrograAvanzada.Models;

public partial class TBitacora
{
    public int IdBitacora { get; set; }

    public string Tabla { get; set; } = null!;

    public string Operacion { get; set; } = null!;

    public string ClavePrimaria { get; set; } = null!;

    public string? ValoresAntes { get; set; }

    public string? ValoresDespues { get; set; }

    public string UsuarioSql { get; set; } = null!;

    public DateTime Fecha { get; set; }
}
