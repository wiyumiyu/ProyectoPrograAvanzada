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
    [Authorize(Roles = "Jefe,Administrador")]
    public class TRolesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TRolesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TRoles
        public async Task<IActionResult> Index()
        {
            return View(await _context.TRoles.ToListAsync());
        }

        // GET: TRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tRole = await _context.TRoles
                .FirstOrDefaultAsync(m => m.IdRol == id);
            if (tRole == null)
            {
                return NotFound();
            }

            return View(tRole);
        }

        // GET: TRoles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRol,Rol")] TRole tRole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tRole);
        }

        // GET: TRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tRole = await _context.TRoles.FindAsync(id);
            if (tRole == null)
            {
                return NotFound();
            }
            return View(tRole);
        }

        // POST: TRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRol,Rol")] TRole tRole)
        {
            if (id != tRole.IdRol)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TRoleExists(tRole.IdRol))
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
            return View(tRole);
        }

        // GET: TRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tRole = await _context.TRoles
                .FirstOrDefaultAsync(m => m.IdRol == id);
            if (tRole == null)
            {
                return NotFound();
            }

            return View(tRole);
        }

        // POST: TRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tRole = await _context.TRoles.FindAsync(id);
            if (tRole != null)
            {
                _context.TRoles.Remove(tRole);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TRoleExists(int id)
        {
            return _context.TRoles.Any(e => e.IdRol == id);
        }
    }
}
