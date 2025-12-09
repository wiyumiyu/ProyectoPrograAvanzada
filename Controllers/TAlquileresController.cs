using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoPrograAvanzada.Data;
using ProyectoPrograAvanzada.Models;
using ProyectoPrograAvanzada.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: TAlquileres
        public async Task<IActionResult> Index()
        {
            var dbAlquilerVehiculosContext = _context.TAlquileres.Include(t => t.IdClienteNavigation).Include(t => t.IdEmpleadoNavigation).Include(t => t.IdSucursalNavigation);
            return View(await dbAlquilerVehiculosContext.ToListAsync());
        }

        // GET: TAlquileres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquilere = await _context.TAlquileres
                .Include(t => t.IdClienteNavigation)
                .Include(t => t.IdEmpleadoNavigation)
                .Include(t => t.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.IdAlquiler == id);
            if (tAlquilere == null)
            {
                return NotFound();
            }

            return View(tAlquilere);
        }

        // GET: TAlquileres/Create
        //public IActionResult Create()
        //{
        //    ViewData["IdCliente"] = new SelectList(_context.TClientes, "IdCliente", "IdCliente");
        //    ViewData["IdEmpleado"] = new SelectList(_context.TEmpleados, "IdEmpleado", "IdEmpleado");
        //    ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal");
        //    return View();
        //}


        public IActionResult Create()
        {
            var vm = new AlquilerCreateVM();

            ViewBag.Clientes = _context.TClientes.ToList();
            ViewBag.Empleados = _context.TEmpleados.ToList();
            ViewBag.Vehiculos = _context.TVehiculos
                                        .Where(v => v.estado == "Disponible")
                                        .ToList();
            ViewBag.Sucursales = _context.TSucursales.ToList();

            return View(vm);
        }


        // POST: TAlquileres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> Create([Bind("IdAlquiler,FechaInicio,FechaFin,Iva,IdCliente,IdEmpleado,IdSucursal,Estado")] TAlquilere tAlquilere)
        //        {
        //            //if (ModelState.IsValid)
        //            //{

        //            //}

        //            //            _context.Add(tAlquilere);
        //            //          await _context.SaveChangesAsync();
        //            //        return RedirectToAction(nameof(Index));

        //            await _context.Database.ExecuteSqlInterpolatedAsync($@"
        //    EXEC SC_AlquilerVehiculos.SP_AlquilerInsert
        //        @fecha_inicio = {tAlquilere.FechaInicio},
        //        @fecha_fin    = {tAlquilere.FechaFin},
        //        @iva          = {tAlquilere.Iva},
        //        @id_cliente   = {tAlquilere.IdCliente},
        //        @id_empleado  = {tAlquilere.IdEmpleado},
        //        @id_sucursal  = {tAlquilere.IdSucursal},
        //        @estado       = {tAlquilere.Estado}
        //");

        //            return RedirectToAction(nameof(Index));

        //            ViewData["IdCliente"] = new SelectList(_context.TClientes, "IdCliente", "IdCliente", tAlquilere.IdCliente);
        //            ViewData["IdEmpleado"] = new SelectList(_context.TEmpleados, "IdEmpleado", "IdEmpleado", tAlquilere.IdEmpleado);
        //            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tAlquilere.IdSucursal);
        //            return View(tAlquilere);
        //        }

        [HttpPost]
        public IActionResult Create(AlquilerCreateVM vm)
        {
            if (!vm.Detalles.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un vehículo.");
                return View(vm);
            }

            // 1. Crear el encabezado con fechas NULL
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
                decimal subtotal = det.TarifaDiaria *
                                   (decimal)(det.FechaFin - det.FechaInicio).TotalDays;

                var detalle = new TAlquileresDetalle
                {
                    IdAlquiler = alquiler.IdAlquiler,
                    IdVehiculo = det.IdVehiculo,
                    TarifaDiaria = det.TarifaDiaria,
                    FechaInicio = det.FechaInicio,
                    FechaFin = det.FechaFin,
                    Subtotal = subtotal
                };

                _context.TAlquileresDetalles.Add(detalle);

                // actualizar estado del vehículo
                var veh = _context.TVehiculos.Find(det.IdVehiculo);
                veh.Estado = "Alquilado";
            }

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = alquiler.IdAlquiler });
        }



        // GET: TAlquileres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquilere = await _context.TAlquileres.FindAsync(id);
            if (tAlquilere == null)
            {
                return NotFound();
            }
            ViewData["IdCliente"] = new SelectList(_context.TClientes, "IdCliente", "IdCliente", tAlquilere.IdCliente);
            ViewData["IdEmpleado"] = new SelectList(_context.TEmpleados, "IdEmpleado", "IdEmpleado", tAlquilere.IdEmpleado);
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tAlquilere.IdSucursal);
            return View(tAlquilere);
        }

        // POST: TAlquileres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAlquiler,FechaInicio,FechaFin,Iva,IdCliente,IdEmpleado,IdSucursal,Estado")] TAlquilere tAlquilere)
        {
            if (id != tAlquilere.IdAlquiler)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{

            //}
            try
            {
                //  _context.Update(tAlquilere);
                // await _context.SaveChangesAsync();

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
    EXEC SC_AlquilerVehiculos.SP_AlquilerUpdate
        @id_alquiler = {tAlquilere.IdAlquiler},
        @fecha_inicio = {tAlquilere.FechaInicio},
        @fecha_fin    = {tAlquilere.FechaFin},
        @iva          = {tAlquilere.Iva},
        @id_cliente   = {tAlquilere.IdCliente},
        @id_empleado  = {tAlquilere.IdEmpleado},
        @id_sucursal  = {tAlquilere.IdSucursal},
        @estado       = {tAlquilere.Estado}
");

                return RedirectToAction(nameof(Index));

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TAlquilereExists(tAlquilere.IdAlquiler))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

            ViewData["IdCliente"] = new SelectList(_context.TClientes, "IdCliente", "IdCliente", tAlquilere.IdCliente);
            ViewData["IdEmpleado"] = new SelectList(_context.TEmpleados, "IdEmpleado", "IdEmpleado", tAlquilere.IdEmpleado);
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tAlquilere.IdSucursal);
            return View(tAlquilere);
        }

        // GET: TAlquileres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquilere = await _context.TAlquileres
                .Include(t => t.IdClienteNavigation)
                .Include(t => t.IdEmpleadoNavigation)
                .Include(t => t.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.IdAlquiler == id);
            if (tAlquilere == null)
            {
                return NotFound();
            }

            return View(tAlquilere);
        }

        // POST: TAlquileres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tAlquilere = await _context.TAlquileres.FindAsync(id);
            if (tAlquilere != null)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
    EXEC SC_AlquilerVehiculos.SP_AlquilerDelete
        @id_alquiler = {id}
");

                return RedirectToAction(nameof(Index));
            }

           // await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TAlquilereExists(int id)
        {
            return _context.TAlquileres.Any(e => e.IdAlquiler == id);
        }
    }
}
