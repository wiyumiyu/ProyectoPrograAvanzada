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

        // GET: TEmpleadoes/Create - Redirigir a Account/Register
        public IActionResult Create()
        {
            TempData["InfoMessage"] = "Para crear un nuevo empleado, utiliza el sistema de registro de usuarios.";
            return RedirectToAction("Register", "Account");
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
            ViewData["IdRol"] = new SelectList(_context.TRoles, "IdRol", "Rol", tEmpleado.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", tEmpleado.IdSucursal);
            return View(tEmpleado);
        }

        // POST: TEmpleadoes/Edit/5
        // NOTA: NO se permite cambiar la contraseña desde aquí, solo datos básicos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleado,Nombre,Correo,Telefono,Puesto,IdSucursal,IdRol")] TEmpleado tEmpleado)
        {
            if (id != tEmpleado.IdEmpleado)
            {
                return NotFound();
            }

            try
            {
                // Obtener el empleado actual para mantener su hash de contraseña
                var empleadoActual = await _context.TEmpleados.AsNoTracking().FirstOrDefaultAsync(e => e.IdEmpleado == id);
                if (empleadoActual == null)
                {
                    return NotFound();
                }

                // Ejecutar SP con la contraseña actual (sin cambios)
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC SC_AlquilerVehiculos.SP_EmpleadoUpdate
                        @id_empleado = {tEmpleado.IdEmpleado},
                        @nombre      = {tEmpleado.Nombre},
                        @correo      = {tEmpleado.Correo},
                        @telefono    = {tEmpleado.Telefono},
                        @contrasena_hash  = {empleadoActual.ContraseñaHash},
                        @puesto      = {tEmpleado.Puesto},
                        @id_sucursal = {tEmpleado.IdSucursal},
                        @id_rol      = {tEmpleado.IdRol}
                ");

                TempData["SuccessMessage"] = "Empleado actualizado exitosamente";
                return RedirectToAction(nameof(Index));
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar: {ex.Message}");
                ViewData["IdRol"] = new SelectList(_context.TRoles, "IdRol", "Rol", tEmpleado.IdRol);
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", tEmpleado.IdSucursal);
                return View(tEmpleado);
            }
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
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC SC_AlquilerVehiculos.SP_EmpleadoDelete
                        @id_empleado = {id}
                ");

                TempData["SuccessMessage"] = "Empleado eliminado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"No se puede eliminar el empleado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool TEmpleadoExists(int id)
        {
            return _context.TEmpleados.Any(e => e.IdEmpleado == id);
        }
    }
}
