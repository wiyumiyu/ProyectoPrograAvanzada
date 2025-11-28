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
    public class TBitacorasController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TBitacorasController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TBitacoras
        public async Task<IActionResult> Index()
        {
            return View(await _context.TBitacoras.ToListAsync());
        }

        // GET: TBitacoras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBitacora = await _context.TBitacoras
                .FirstOrDefaultAsync(m => m.IdBitacora == id);
            if (tBitacora == null)
            {
                return NotFound();
            }

            return View(tBitacora);
        }

        // GET: TBitacoras/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TBitacoras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdBitacora,Tabla,Operacion,ClavePrimaria,ValoresAntes,ValoresDespues,UsuarioSql,Fecha")] TBitacora tBitacora)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(tBitacora);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}

            _context.Add(tBitacora);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


            return View(tBitacora);
        }

        // GET: TBitacoras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBitacora = await _context.TBitacoras.FindAsync(id);
            if (tBitacora == null)
            {
                return NotFound();
            }
            return View(tBitacora);
        }

        // POST: TBitacoras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdBitacora,Tabla,Operacion,ClavePrimaria,ValoresAntes,ValoresDespues,UsuarioSql,Fecha")] TBitacora tBitacora)
        {
            if (id != tBitacora.IdBitacora)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(tBitacora);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!TBitacoraExists(tBitacora.IdBitacora))
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
                _context.Update(tBitacora);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TBitacoraExists(tBitacora.IdBitacora))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));



            return View(tBitacora);
        }

        // GET: TBitacoras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBitacora = await _context.TBitacoras
                .FirstOrDefaultAsync(m => m.IdBitacora == id);
            if (tBitacora == null)
            {
                return NotFound();
            }

            return View(tBitacora);
        }

        // POST: TBitacoras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tBitacora = await _context.TBitacoras.FindAsync(id);
            if (tBitacora != null)
            {
                _context.TBitacoras.Remove(tBitacora);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TBitacoraExists(int id)
        {
            return _context.TBitacoras.Any(e => e.IdBitacora == id);
        }
    }
}
