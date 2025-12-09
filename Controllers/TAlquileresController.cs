using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPrograAvanzada.Data;
using ProyectoPrograAvanzada.Models;
using ProyectoPrograAvanzada.Services;
using ProyectoPrograAvanzada.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace ProyectoPrograAvanzada.Controllers
{
    [Authorize]
    public class TAlquileresController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TAlquileresController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // ========================================================
        // LISTA
        // ========================================================
        public async Task<IActionResult> Index()
        {
            var data = _context.TAlquileres
                .Include(t => t.IdClienteNavigation)
                .Include(t => t.IdEmpleadoNavigation)
                .Include(t => t.IdSucursalNavigation);

            return View(await data.ToListAsync());
        }

        // ========================================================
        // DETALLE
        // ========================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var alquiler = await _context.TAlquileres
                .Include(a => a.IdClienteNavigation)
                .Include(a => a.IdEmpleadoNavigation)
                .Include(a => a.IdSucursalNavigation)
                .Include(a => a.TAlquileresDetalles)
                .ThenInclude(d => d.IdVehiculoNavigation)
                .FirstOrDefaultAsync(a => a.IdAlquiler == id);

            if (alquiler == null) return NotFound();

            return View(alquiler);
        }

        // ========================================================
        // CREAR (GET)
        // ========================================================
        public IActionResult Create()
        {
            var vm = new AlquilerCreateVM();

            // Empleado del usuario autenticado
            var idEmpleado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            vm.IdEmpleado = idEmpleado;
            
            ViewBag.Clientes = _context.TClientes.ToList();
            ViewBag.Vehiculos = _context.TVehiculos
                .Include(v => v.IdTipoNavigation)
                .Where(v => v.Estado == "Disponible")
                .Select(v => new
                {
                    idVehiculo = v.IdVehiculo,
                    placa = v.Placa,
                    marca = v.Marca,
                    tarifa = v.IdTipoNavigation.TarifaDiaria
                })
                .ToList();


            ViewBag.Sucursales = _context.TSucursales.ToList();
     

            return View(vm);
        }

        // ========================================================
        // CREAR (POST)
        // ========================================================
        [HttpPost]
        public IActionResult Create(AlquilerCreateVM vm)
        {
            if (!vm.Detalles.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un vehículo.");
                return View(vm);
            }

            // 1. Crear encabezado
            var alquiler = new TAlquilere
            {
                //FechaInicio = null,
                FechaFin = null,
                Iva = vm.IVA,
                IdCliente = vm.IdCliente,
                IdEmpleado = vm.IdEmpleado,
                IdSucursal = vm.IdSucursal,
                Estado = "Activo"
            };

            _context.TAlquileres.Add(alquiler);
            _context.SaveChanges();

            // 2. Insertar detalle
            foreach (var det in vm.Detalles)
            {
                var dias = (det.FechaFin.Date - det.FechaInicio.Date).TotalDays;
                if (dias <= 0) dias = 1;

                decimal subtotal = det.TarifaDiaria * (decimal)dias;

                var detalle = new TAlquileresDetalle
                {
                    IdAlquiler = alquiler.IdAlquiler,
                    IdVehiculo = det.IdVehiculo,
                    TarifaDiaria = det.TarifaDiaria,
                    FechaInicio = DateOnly.FromDateTime(det.FechaInicio),
                    FechaFin = DateOnly.FromDateTime(det.FechaFin),
                    Subtotal = subtotal
                };

                _context.TAlquileresDetalles.Add(detalle);

                // Cambiar estado del vehículo
                var veh = _context.TVehiculos.Find(det.IdVehiculo);
                veh.Estado = "Alquilado";
            }

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = alquiler.IdAlquiler });
        }

        // ========================================================
        // EDITAR
        // ========================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tAlquilere = await _context.TAlquileres.FindAsync(id);
            if (tAlquilere == null) return NotFound();

            return View(tAlquilere);
        }

        // ========================================================
        // ELIMINAR
        // ========================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var alquiler = await _context.TAlquileres
                .Include(t => t.IdClienteNavigation)
                .Include(t => t.IdEmpleadoNavigation)
                .Include(t => t.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.IdAlquiler == id);

            if (alquiler == null) return NotFound();

            return View(alquiler);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.TAlquileres.FindAsync(id);

            if (entity != null)
            {
                _context.TAlquileres.Remove(entity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TAlquilereExists(int id)
        {
            return _context.TAlquileres.Any(e => e.IdAlquiler == id);
        }

        // ========================================================
        // (PASO 5) OBTENER TARIFA AUTOMÁTICA
        // ========================================================
        [HttpGet]
        public IActionResult ObtenerTarifa(int idVehiculo)
        {
            var vehiculo = _context.TVehiculos
                .Include(v => v.IdTipoNavigation)
                .FirstOrDefault(v => v.IdVehiculo == idVehiculo);

            if (vehiculo == null)
                return NotFound();

            return Json(new { tarifa = vehiculo.IdTipoNavigation.TarifaDiaria });
        }

        // ========================================================
        // PDF
        // ========================================================
        public IActionResult GenerarRecibo(int idAlquiler)
        {
            var alquiler = _context.TAlquileres
                .Include(a => a.TAlquileresDetalles)
                .Include(a => a.IdClienteNavigation)
                .FirstOrDefault(a => a.IdAlquiler == idAlquiler);

            if (alquiler == null)
                return NotFound();

            var pdf = PdfGeneratorService.GenerarRecibo(alquiler);

            return File(pdf, "application/pdf", $"Recibo_{idAlquiler}.pdf");
        }
    }
}
