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
    public class TEmpleadoesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TEmpleadoesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TEmpleadoes
        public async Task<IActionResult> Index()
        {
            var dbAlquilerVehiculosContext = _context.TEmpleados.Include(t => t.IdRolNavigation).Include(t => t.IdSucursalNavigation);
            return View(await dbAlquilerVehiculosContext.ToListAsync());
        }

        // GET: TEmpleadoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tEmpleado = await _context.TEmpleados
                .Include(t => t.IdRolNavigation)
                .Include(t => t.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);
            if (tEmpleado == null)
            {
                return NotFound();
            }

            return View(tEmpleado);
        }

        // GET: TEmpleadoes/Create
        public IActionResult Create()
        {
            ViewData["IdRol"] = new SelectList(_context.TRoles, "IdRol", "IdRol");
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal");
            return View();
        }

        // POST: TEmpleadoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleado,Nombre,Correo,Telefono,Contrasena,Puesto,IdSucursal,IdRol")] TEmpleado tEmpleado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tEmpleado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRol"] = new SelectList(_context.TRoles, "IdRol", "IdRol", tEmpleado.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tEmpleado.IdSucursal);
            return View(tEmpleado);
        }

        // GET: TEmpleadoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tEmpleado = await _context.TEmpleados.FindAsync(id);
            if (tEmpleado == null)
            {
                return NotFound();
            }
            ViewData["IdRol"] = new SelectList(_context.TRoles, "IdRol", "IdRol", tEmpleado.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tEmpleado.IdSucursal);
            return View(tEmpleado);
        }

        // POST: TEmpleadoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleado,Nombre,Correo,Telefono,Contrasena,Puesto,IdSucursal,IdRol")] TEmpleado tEmpleado)
        {
            if (id != tEmpleado.IdEmpleado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tEmpleado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TEmpleadoExists(tEmpleado.IdEmpleado))
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
            ViewData["IdRol"] = new SelectList(_context.TRoles, "IdRol", "IdRol", tEmpleado.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tEmpleado.IdSucursal);
            return View(tEmpleado);
        }

        // GET: TEmpleadoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tEmpleado = await _context.TEmpleados
                .Include(t => t.IdRolNavigation)
                .Include(t => t.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);
            if (tEmpleado == null)
            {
                return NotFound();
            }

            return View(tEmpleado);
        }

        // POST: TEmpleadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tEmpleado = await _context.TEmpleados.FindAsync(id);
            if (tEmpleado != null)
            {
                _context.TEmpleados.Remove(tEmpleado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TEmpleadoExists(int id)
        {
            return _context.TEmpleados.Any(e => e.IdEmpleado == id);
        }
    }
}
