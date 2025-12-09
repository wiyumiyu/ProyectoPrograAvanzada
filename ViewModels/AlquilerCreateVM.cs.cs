namespace ProyectoPrograAvanzada.ViewModels
{
    public class AlquilerCreateVM
    {
        public int IdCliente { get; set; }
        public int IdEmpleado { get; set; }
        public int IdSucursal { get; set; }

        public List<DetalleItem> Detalles { get; set; } = new();

        public class DetalleItem
        {
            public int IdVehiculo { get; set; }
            public decimal TarifaDiaria { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }

            public decimal Subtotal =>
                TarifaDiaria * (decimal)(FechaFin.Date - FechaInicio.Date).TotalDays;
        }

        public decimal Total => Detalles.Sum(x => x.Subtotal);
        public decimal IVA => Total * 0.13m;
    }


}
