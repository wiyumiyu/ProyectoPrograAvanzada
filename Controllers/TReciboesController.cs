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
    public class TReciboesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TReciboesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TReciboes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TRecibos.ToListAsync());
        }

        // GET: TReciboes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tRecibo = await _context.TRecibos
                .FirstOrDefaultAsync(m => m.IdRecibo == id);
            if (tRecibo == null)
            {
                return NotFound();
            }

            return View(tRecibo);
        }

        // GET: TReciboes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TReciboes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRecibo,FechaPago,Monto,Metodo")] TRecibo tRecibo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tRecibo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tRecibo);
        }

        // GET: TReciboes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tRecibo = await _context.TRecibos.FindAsync(id);
            if (tRecibo == null)
            {
                return NotFound();
            }
            return View(tRecibo);
        }

        // POST: TReciboes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRecibo,FechaPago,Monto,Metodo")] TRecibo tRecibo)
        {
            if (id != tRecibo.IdRecibo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tRecibo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TReciboExists(tRecibo.IdRecibo))
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
            return View(tRecibo);
        }

        // GET: TReciboes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tRecibo = await _context.TRecibos
                .FirstOrDefaultAsync(m => m.IdRecibo == id);
            if (tRecibo == null)
            {
                return NotFound();
            }

            return View(tRecibo);
        }

        // POST: TReciboes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tRecibo = await _context.TRecibos.FindAsync(id);
            if (tRecibo != null)
            {
                _context.TRecibos.Remove(tRecibo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TReciboExists(int id)
        {
            return _context.TRecibos.Any(e => e.IdRecibo == id);
        }
    }
}
