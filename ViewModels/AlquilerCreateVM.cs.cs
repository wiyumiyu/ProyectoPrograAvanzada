namespace ProyectoPrograAvanzada.ViewModels
{
    public class AlquilerCreateVM
    {
        // Encabezado
        public int IdCliente { get; set; }
        public int IdEmpleado { get; set; }
        public int IdSucursal { get; set; }

        // Detalle dinámico
        public List<DetalleItem> Detalles { get; set; } = new();

        public class DetalleItem
        {
            public int IdVehiculo { get; set; }
            public decimal TarifaDiaria { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }

            public decimal Subtotal =>
                TarifaDiaria * (decimal)(FechaFin - FechaInicio).TotalDays;
        }

        public decimal Total => Detalles.Sum(x => x.Subtotal);
        public decimal IVA => Total * 0.13m;   // o lo cambiamos si querés
    }

}
