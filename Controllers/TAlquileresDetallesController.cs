using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoPrograAvanzada.Data;
using ProyectoPrograAvanzada.Models;

namespace ProyectoPrograAvanzada.Controllers
{
    [Authorize]
    public class TAlquileresDetallesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TAlquileresDetallesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TAlquileresDetalles
        public async Task<IActionResult> Index()
        {
            var dbAlquilerVehiculosContext = _context.TAlquileresDetalles.Include(t => t.IdAlquilerNavigation).Include(t => t.IdVehiculoNavigation);
            return View(await dbAlquilerVehiculosContext.ToListAsync());
        }

        // GET: TAlquileresDetalles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquileresDetalle = await _context.TAlquileresDetalles
                .Include(t => t.IdAlquilerNavigation)
                .Include(t => t.IdVehiculoNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalle == id);
            if (tAlquileresDetalle == null)
            {
                return NotFound();
            }

            return View(tAlquileresDetalle);
        }

        // GET: TAlquileresDetalles/Create
        public IActionResult Create()
        {
            ViewData["IdAlquiler"] = new SelectList(_context.TAlquileres, "IdAlquiler", "IdAlquiler");
            ViewData["IdVehiculo"] = new SelectList(_context.TVehiculos, "IdVehiculo", "IdVehiculo");
            return View();
        }

        // POST: TAlquileresDetalles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDetalle,IdAlquiler,IdVehiculo,TarifaDiaria,FechaInicio,FechaFin,Subtotal")] TAlquileresDetalle tAlquileresDetalle)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(tAlquileresDetalle);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}

            //_context.Add(tAlquileresDetalle);
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
    EXEC SC_AlquilerVehiculos.SP_AlquilerDetalleInsert
        @id_alquiler   = {tAlquileresDetalle.IdAlquiler},
        @id_vehiculo   = {tAlquileresDetalle.IdVehiculo},
        @tarifa_diaria = {tAlquileresDetalle.TarifaDiaria},
        @fecha_inicio  = {tAlquileresDetalle.FechaInicio},
        @fecha_fin     = {tAlquileresDetalle.FechaFin},
        @subtotal      = {tAlquileresDetalle.Subtotal}
");
            return RedirectToAction(nameof(Index));
            ViewData["IdAlquiler"] = new SelectList(_context.TAlquileres, "IdAlquiler", "IdAlquiler", tAlquileresDetalle.IdAlquiler);
            ViewData["IdVehiculo"] = new SelectList(_context.TVehiculos, "IdVehiculo", "IdVehiculo", tAlquileresDetalle.IdVehiculo);
            return View(tAlquileresDetalle);
        }

        // GET: TAlquileresDetalles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquileresDetalle = await _context.TAlquileresDetalles.FindAsync(id);
            if (tAlquileresDetalle == null)
            {
                return NotFound();
            }
            ViewData["IdAlquiler"] = new SelectList(_context.TAlquileres, "IdAlquiler", "IdAlquiler", tAlquileresDetalle.IdAlquiler);
            ViewData["IdVehiculo"] = new SelectList(_context.TVehiculos, "IdVehiculo", "IdVehiculo", tAlquileresDetalle.IdVehiculo);
            return View(tAlquileresDetalle);
        }

        // POST: TAlquileresDetalles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDetalle,IdAlquiler,IdVehiculo,TarifaDiaria,FechaInicio,FechaFin,Subtotal")] TAlquileresDetalle tAlquileresDetalle)
        {
            if (id != tAlquileresDetalle.IdDetalle)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(tAlquileresDetalle);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!TAlquileresDetalleExists(tAlquileresDetalle.IdDetalle))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}
            try
            {
                //_context.Update(tAlquileresDetalle);
                //await _context.SaveChangesAsync();


                await _context.Database.ExecuteSqlInterpolatedAsync($@"
    EXEC SC_AlquilerVehiculos.SP_AlquilerDetalleUpdate
        @id_detalle   = {tAlquileresDetalle.IdDetalle},
        @id_alquiler  = {tAlquileresDetalle.IdAlquiler},
        @id_vehiculo  = {tAlquileresDetalle.IdVehiculo},
        @tarifa_diaria = {tAlquileresDetalle.TarifaDiaria},
        @fecha_inicio  = {tAlquileresDetalle.FechaInicio},
        @fecha_fin     = {tAlquileresDetalle.FechaFin},
        @subtotal      = {tAlquileresDetalle.Subtotal}
");

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TAlquileresDetalleExists(tAlquileresDetalle.IdDetalle))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

            ViewData["IdAlquiler"] = new SelectList(_context.TAlquileres, "IdAlquiler", "IdAlquiler", tAlquileresDetalle.IdAlquiler);
            ViewData["IdVehiculo"] = new SelectList(_context.TVehiculos, "IdVehiculo", "IdVehiculo", tAlquileresDetalle.IdVehiculo);
            return View(tAlquileresDetalle);
        }

        // GET: TAlquileresDetalles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquileresDetalle = await _context.TAlquileresDetalles
                .Include(t => t.IdAlquilerNavigation)
                .Include(t => t.IdVehiculoNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalle == id);
            if (tAlquileresDetalle == null)
            {
                return NotFound();
            }

            return View(tAlquileresDetalle);
        }

        // POST: TAlquileresDetalles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tAlquileresDetalle = await _context.TAlquileresDetalles.FindAsync(id);
            if (tAlquileresDetalle != null)
            {


                //_context.TAlquileresDetalles.Remove(tAlquileresDetalle);

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
    EXEC SC_AlquilerVehiculos.SP_AlquilerDetalleDelete
        @id_detalle = {id}
");

                return RedirectToAction(nameof(Index));
            }

            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TAlquileresDetalleExists(int id)
        {
            return _context.TAlquileresDetalles.Any(e => e.IdDetalle == id);
        }
    }
}
