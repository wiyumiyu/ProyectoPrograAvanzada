using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoPrograAvanzada.Data;
using ProyectoPrograAvanzada.Models;

namespace ProyectoPrograAvanzada.Controllers
{
    public class TVehiculoesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TVehiculoesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TVehiculoes
        public async Task<IActionResult> Index()
        {
            var dbAlquilerVehiculosContext = _context.TVehiculos.Include(t => t.IdSucursalNavigation).Include(t => t.IdTipoNavigation);
            return View(await dbAlquilerVehiculosContext.ToListAsync());
        }

        // GET: TVehiculoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tVehiculo = await _context.TVehiculos
                .Include(t => t.IdSucursalNavigation)
                .Include(t => t.IdTipoNavigation)
                .FirstOrDefaultAsync(m => m.IdVehiculo == id);
            if (tVehiculo == null)
            {
                return NotFound();
            }

            return View(tVehiculo);
        }

        // GET: TVehiculoes/Create
        public IActionResult Create()
        {
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre");
            ViewData["IdTipo"] = new SelectList(_context.TVehiculosTipos, "IdTipo", "Descripcion");
            return View();
        }

        // POST: TVehiculoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVehiculo,Placa,Marca,Modelo,Anio,Estado,IdTipo,IdSucursal")] TVehiculo tVehiculo)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(tVehiculo);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}

            //_context.Add(tVehiculo);
            //await _context.SaveChangesAsync();
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC SC_AlquilerVehiculos.SP_VehiculoInsert 
                @placa       = {tVehiculo.Placa},
                @marca       = {tVehiculo.Marca},
                @modelo      = {tVehiculo.Modelo},
                @anio        = {tVehiculo.Anio},
                @estado      = {tVehiculo.Estado},
                @id_tipo     = {tVehiculo.IdTipo},
                @id_sucursal = {tVehiculo.IdSucursal}
               ");

            return RedirectToAction(nameof(Index));


            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tVehiculo.IdSucursal);
            ViewData["IdTipo"] = new SelectList(_context.TVehiculosTipos, "IdTipo", "IdTipo", tVehiculo.IdTipo);
            return View(tVehiculo);
        }

        // GET: TVehiculoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tVehiculo = await _context.TVehiculos.FindAsync(id);
            if (tVehiculo == null)
            {
                return NotFound();
            }
            //ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tVehiculo.IdSucursal);
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", tVehiculo.IdSucursal);
            ViewData["IdTipo"] = new SelectList(_context.TVehiculosTipos, "IdTipo", "Descripcion", tVehiculo.IdTipo);
            return View(tVehiculo);
        }

        // POST: TVehiculoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVehiculo,Placa,Marca,Modelo,Anio,Estado,IdTipo,IdSucursal")] TVehiculo tVehiculo)
        {
            if (id != tVehiculo.IdVehiculo)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(tVehiculo);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!TVehiculoExists(tVehiculo.IdVehiculo))
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
                //_context.Update(tVehiculo);
                //await _context.SaveChangesAsync();

                            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC SC_AlquilerVehiculos.SP_VehiculoUpdate
                    @id_vehiculo = {tVehiculo.IdVehiculo},
                    @placa       = {tVehiculo.Placa},
                    @marca       = {tVehiculo.Marca},
                    @modelo      = {tVehiculo.Modelo},
                    @anio        = {tVehiculo.Anio},
                    @estado      = {tVehiculo.Estado},
                    @id_tipo     = {tVehiculo.IdTipo},
                    @id_sucursal = {tVehiculo.IdSucursal}
            ");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TVehiculoExists(tVehiculo.IdVehiculo))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tVehiculo.IdSucursal);
            ViewData["IdTipo"] = new SelectList(_context.TVehiculosTipos, "IdTipo", "IdTipo", tVehiculo.IdTipo);
            return View(tVehiculo);
        }

        // GET: TVehiculoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tVehiculo = await _context.TVehiculos
                .Include(t => t.IdSucursalNavigation)
                .Include(t => t.IdTipoNavigation)
                .FirstOrDefaultAsync(m => m.IdVehiculo == id);
            if (tVehiculo == null)
            {
                return NotFound();
            }

            return View(tVehiculo);
        }

        // POST: TVehiculoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tVehiculo = await _context.TVehiculos.FindAsync(id);
            if (tVehiculo != null)
            {
                // _context.TVehiculos.Remove(tVehiculo);

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
             EXEC SC_AlquilerVehiculos.SP_VehiculoDelete 
               @id_vehiculo = {id}
                    ");
            }

            //await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        private bool TVehiculoExists(int id)
        {
            return _context.TVehiculos.Any(e => e.IdVehiculo == id);
        }
    }
}
