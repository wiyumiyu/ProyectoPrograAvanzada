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
    public class TClientesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TClientesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TClientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TClientes.ToListAsync());
        }

        // GET: TClientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tCliente = await _context.TClientes
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (tCliente == null)
            {
                return NotFound();
            }

            return View(tCliente);
        }

        // GET: TClientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TClientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCliente,Nombre,Cedula,Telefono,Email,Direccion")] TCliente tCliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tCliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tCliente);
        }

        // GET: TClientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tCliente = await _context.TClientes.FindAsync(id);
            if (tCliente == null)
            {
                return NotFound();
            }
            return View(tCliente);
        }

        // POST: TClientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCliente,Nombre,Cedula,Telefono,Email,Direccion")] TCliente tCliente)
        {
            if (id != tCliente.IdCliente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tCliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TClienteExists(tCliente.IdCliente))
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
            return View(tCliente);
        }

        // GET: TClientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tCliente = await _context.TClientes
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (tCliente == null)
            {
                return NotFound();
            }

            return View(tCliente);
        }

        // POST: TClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tCliente = await _context.TClientes.FindAsync(id);
            if (tCliente != null)
            {
                _context.TClientes.Remove(tCliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TClienteExists(int id)
        {
            return _context.TClientes.Any(e => e.IdCliente == id);
        }
    }
}
