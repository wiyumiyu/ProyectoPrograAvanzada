using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly PdfGeneratorService _pdf;

        public TAlquileresController(DbAlquilerVehiculosContext context, PdfGeneratorService pdf)
        {
            _context = context;
            _pdf = pdf;
        }

        // ========================================================
        // LISTA
        // ========================================================
        public async Task<IActionResult> Index()
        {
            var data = await _context.TAlquileres
                .Include(t => t.IdClienteNavigation)
                .Include(t => t.IdEmpleadoNavigation)
                .Include(t => t.IdSucursalNavigation)
                .Include(t => t.TAlquileresDetalles)
                .ToListAsync();

            return View(data);
        }

        // ========================================================
        // DETALLES
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

            vm.IdEmpleado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            ViewBag.Clientes = _context.TClientes.ToList();
            ViewBag.Empleados = _context.TEmpleados.ToList();
            ViewBag.Sucursales = _context.TSucursales.ToList();

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

            var alquiler = new TAlquilere
            {
                FechaFin = null,
                Iva = vm.IVA,
                IdCliente = vm.IdCliente,
                IdEmpleado = vm.IdEmpleado,
                IdSucursal = vm.IdSucursal,
                Estado = "Activo"
            };

            _context.TAlquileres.Add(alquiler);
            _context.SaveChanges();

            foreach (var det in vm.Detalles)
            {
                var dias = (det.FechaFin.Date - det.FechaInicio.Date).TotalDays;
                if (dias <= 0) dias = 1;

                var detalle = new TAlquileresDetalle
                {
                    IdAlquiler = alquiler.IdAlquiler,
                    IdVehiculo = det.IdVehiculo,
                    TarifaDiaria = det.TarifaDiaria,
                    FechaInicio = DateOnly.FromDateTime(det.FechaInicio),
                    FechaFin = DateOnly.FromDateTime(det.FechaFin),
                    Subtotal = det.TarifaDiaria * (decimal)dias
                };

                _context.TAlquileresDetalles.Add(detalle);

                var veh = _context.TVehiculos.Find(det.IdVehiculo);
                veh.Estado = "Alquilado";
            }

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = alquiler.IdAlquiler });
        }

        // ========================================================
        // EDITAR (GET)
        // ========================================================
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var alquiler = _context.TAlquileres
                .Include(a => a.IdClienteNavigation)
                .Include(a => a.IdEmpleadoNavigation)
                .Include(a => a.IdSucursalNavigation)
                .Include(a => a.TAlquileresDetalles)
                .ThenInclude(d => d.IdVehiculoNavigation)
                .FirstOrDefault(a => a.IdAlquiler == id);

            if (alquiler == null) return NotFound();

            ViewBag.Clientes = new SelectList(_context.TClientes, "IdCliente", "Nombre", alquiler.IdCliente);
            ViewBag.Empleados = new SelectList(_context.TEmpleados, "IdEmpleado", "Nombre", alquiler.IdEmpleado);
            ViewBag.Sucursales = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", alquiler.IdSucursal);

            ViewBag.VehiculosDisponibles = _context.TVehiculos
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

            return View(alquiler);
        }

        // ========================================================
        // EDITAR (POST)
        // ========================================================
        [HttpPost]
        public IActionResult Edit(TAlquilere model, List<TAlquileresDetalle>? nuevos)
        {
            var alquiler = _context.TAlquileres
                .Include(a => a.TAlquileresDetalles)
                .FirstOrDefault(a => a.IdAlquiler == model.IdAlquiler);

            if (alquiler == null) return NotFound();

            // Actualizar encabezado
            alquiler.IdCliente = model.IdCliente;
            alquiler.IdEmpleado = model.IdEmpleado;
            alquiler.IdSucursal = model.IdSucursal;
            alquiler.Iva = model.Iva;
            alquiler.Estado = model.Estado;

            // AGREGAR NUEVOS DETALLES
            if (nuevos != null)
            {
                foreach (var det in nuevos)
                {
                    if (det.IdVehiculo == 0) continue;

                    int dias = (int)(det.FechaFin.ToDateTime(TimeOnly.MinValue) -
                                     det.FechaInicio.ToDateTime(TimeOnly.MinValue)).TotalDays;

                    if (dias <= 0) dias = 1;

                    var detalle = new TAlquileresDetalle
                    {
                        IdAlquiler = alquiler.IdAlquiler,
                        IdVehiculo = det.IdVehiculo,
                        TarifaDiaria = det.TarifaDiaria,
                        FechaInicio = det.FechaInicio,
                        FechaFin = det.FechaFin,
                        Subtotal = det.TarifaDiaria * dias
                    };

                    _context.TAlquileresDetalles.Add(detalle);

                    var veh = _context.TVehiculos.Find(det.IdVehiculo);
                    if (veh != null) veh.Estado = "Alquilado";
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Details", new { id = alquiler.IdAlquiler });
        }

        // ========================================================
        // ELIMINAR DETALLE
        // ========================================================
        public IActionResult DeleteDetalle(int id, int idAlquiler)
        {
            var detalle = _context.TAlquileresDetalles
                .FirstOrDefault(d => d.IdDetalle == id);

            if (detalle == null) return NotFound();

            var vehiculo = _context.TVehiculos.FirstOrDefault(v => v.IdVehiculo == detalle.IdVehiculo);
            if (vehiculo != null)
                vehiculo.Estado = "Disponible";

            _context.TAlquileresDetalles.Remove(detalle);
            _context.SaveChanges();

            return RedirectToAction("Edit", new { id = idAlquiler });
        }

        // ========================================================
        // ELIMINAR ALQUILER (GET)
        // ========================================================
        public async Task<IActionResult> Delete(int? id)
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
        // ELIMINAR ALQUILER (POST)
        // ========================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alquiler = await _context.TAlquileres
                .Include(a => a.TAlquileresDetalles)
                .FirstOrDefaultAsync(a => a.IdAlquiler == id);

            if (alquiler == null) return NotFound();

            // Liberar todos los vehículos asociados
            foreach (var d in alquiler.TAlquileresDetalles)
            {
                var veh = _context.TVehiculos.FirstOrDefault(v => v.IdVehiculo == d.IdVehiculo);
                if (veh != null)
                    veh.Estado = "Disponible";

                _context.TAlquileresDetalles.Remove(d);
            }

            _context.TAlquileres.Remove(alquiler);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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

            var pdf = _pdf.GenerarRecibo(alquiler);

            return File(pdf, "application/pdf", $"Recibo_{idAlquiler}.pdf");
        }
    }
}
