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
    public class TSucursalesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TSucursalesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TSucursales
        public async Task<IActionResult> Index()
        {
            return View(await _context.TSucursales.ToListAsync());
        }

        // GET: TSucursales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tSucursale = await _context.TSucursales
                .FirstOrDefaultAsync(m => m.IdSucursal == id);
            if (tSucursale == null)
            {
                return NotFound();
            }

            return View(tSucursale);
        }

        // GET: TSucursales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TSucursales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSucursal,Nombre,Telefono,Direccion")] TSucursale tSucursale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tSucursale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tSucursale);
        }

        // GET: TSucursales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tSucursale = await _context.TSucursales.FindAsync(id);
            if (tSucursale == null)
            {
                return NotFound();
            }
            return View(tSucursale);
        }

        // POST: TSucursales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSucursal,Nombre,Telefono,Direccion")] TSucursale tSucursale)
        {
            if (id != tSucursale.IdSucursal)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tSucursale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TSucursaleExists(tSucursale.IdSucursal))
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
            return View(tSucursale);
        }

        // GET: TSucursales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tSucursale = await _context.TSucursales
                .FirstOrDefaultAsync(m => m.IdSucursal == id);
            if (tSucursale == null)
            {
                return NotFound();
            }

            return View(tSucursale);
        }

        // POST: TSucursales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tSucursale = await _context.TSucursales.FindAsync(id);
            if (tSucursale != null)
            {
                _context.TSucursales.Remove(tSucursale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TSucursaleExists(int id)
        {
            return _context.TSucursales.Any(e => e.IdSucursal == id);
        }
    }
}
