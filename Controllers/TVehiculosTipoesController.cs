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
    public class TVehiculosTipoesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TVehiculosTipoesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TVehiculosTipoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TVehiculosTipos.ToListAsync());
        }

        // GET: TVehiculosTipoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tVehiculosTipo = await _context.TVehiculosTipos
                .FirstOrDefaultAsync(m => m.IdTipo == id);
            if (tVehiculosTipo == null)
            {
                return NotFound();
            }

            return View(tVehiculosTipo);
        }

        // GET: TVehiculosTipoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TVehiculosTipoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipo,Descripcion,TarifaDiaria")] TVehiculosTipo tVehiculosTipo)
        {
            // Código original del scaffolding:
            /*
            if (ModelState.IsValid)
            {
                _context.Add(tVehiculosTipo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tVehiculosTipo);
            */

            // Nuevo código usando Stored Procedure SP_VehiculoTipoInsert:
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC SC_AlquilerVehiculos.SP_VehiculoTipoInsert
                    @descripcion   = {tVehiculosTipo.Descripcion},
                    @tarifa_diaria = {tVehiculosTipo.TarifaDiaria}
            ");

            return RedirectToAction(nameof(Index));
        }

        // GET: TVehiculosTipoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tVehiculosTipo = await _context.TVehiculosTipos.FindAsync(id);
            if (tVehiculosTipo == null)
            {
                return NotFound();
            }
            return View(tVehiculosTipo);
        }

        // POST: TVehiculosTipoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipo,Descripcion,TarifaDiaria")] TVehiculosTipo tVehiculosTipo)
        {
            if (id != tVehiculosTipo.IdTipo)
            {
                return NotFound();
            }

            // Código original del scaffolding:
            /*
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tVehiculosTipo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TVehiculosTipoExists(tVehiculosTipo.IdTipo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tVehiculosTipo);
            */

            // Nuevo código usando Stored Procedure SP_VehiculoTipoUpdate:
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC SC_AlquilerVehiculos.SP_VehiculoTipoUpdate
                    @id_tipo       = {tVehiculosTipo.IdTipo},
                    @descripcion   = {tVehiculosTipo.Descripcion},
                    @tarifa_diaria = {tVehiculosTipo.TarifaDiaria}
            ");

            return RedirectToAction(nameof(Index));
        }

        // GET: TVehiculosTipoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tVehiculosTipo = await _context.TVehiculosTipos
                .FirstOrDefaultAsync(m => m.IdTipo == id);
            if (tVehiculosTipo == null)
            {
                return NotFound();
            }

            return View(tVehiculosTipo);
        }

        // POST: TVehiculosTipoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Código original del scaffolding:
            /*
            var tVehiculosTipo = await _context.TVehiculosTipos.FindAsync(id);
            if (tVehiculosTipo != null)
            {
                _context.TVehiculosTipos.Remove(tVehiculosTipo);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            */

            // Nuevo código usando Stored Procedure SP_VehiculoTipoDelete:
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC SC_AlquilerVehiculos.SP_VehiculoTipoDelete
                    @id_tipo = {id}
            ");

            return RedirectToAction(nameof(Index));
        }

        private bool TVehiculosTipoExists(int id)
        {
            return _context.TVehiculosTipos.Any(e => e.IdTipo == id);
        }
    }
}
