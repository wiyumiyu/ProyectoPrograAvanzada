using System;
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

        // ======================================================
        // LISTA
        // ======================================================
        public async Task<IActionResult> Index()
        {
            var empleados = _context.TEmpleados
                .Include(e => e.IdRolNavigation)
                .Include(e => e.IdSucursalNavigation);

            return View(await empleados.ToListAsync());
        }

        // ======================================================
        // DETALLES
        // ======================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var empleado = await _context.TEmpleados
                .Include(e => e.IdRolNavigation)
                .Include(e => e.IdSucursalNavigation)
                .FirstOrDefaultAsync(e => e.IdEmpleado == id);

            if (empleado == null) return NotFound();

            return View(empleado);
        }

        // ======================================================
        // CREAR (redirige a Account/Register)
        // ======================================================
        public IActionResult Create()
        {
            TempData["InfoMessage"] =
                "Para crear un nuevo empleado utilice el módulo de registro de usuarios.";
            return RedirectToAction("Register", "Account");
        }

        // ======================================================
        // EDITAR (GET)
        // ======================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var empleado = await _context.TEmpleados.FindAsync(id);
            if (empleado == null) return NotFound();

            ViewData["IdRol"] = new SelectList(_context.TRoles, "IdRol", "Rol", empleado.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", empleado.IdSucursal);

            return View(empleado);
        }

        // ======================================================
        // EDITAR (POST)
        // ======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    [Bind("IdEmpleado,Nombre,Correo,Telefono,Puesto,IdSucursal,IdRol")]
    TEmpleado tEmpleado)
        {
            if (id != tEmpleado.IdEmpleado)
            {
                return NotFound();
            }

            // Obtener estado actual desde BD
            var empleadoActual = await _context.TEmpleados
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEmpleado == id);

            if (empleadoActual == null)
            {
                return NotFound();
            }

            const int ROL_ADMIN = 3;

            // 🚫 BLOQUEO: No permitir quitar rol Administrador
            if (empleadoActual.IdRol == ROL_ADMIN && tEmpleado.IdRol != ROL_ADMIN)
            {
                TempData["ErrorMessage"] =
                    "No está permitido quitar el rol Administrador a este empleado.";

                ViewData["IdRol"] = new SelectList(_context.TRoles, "IdRol", "Rol", empleadoActual.IdRol);
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", empleadoActual.IdSucursal);

                return View(tEmpleado);
            }

            try
            {
                // Ejecutar SP conservando contraseña
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC SC_AlquilerVehiculos.SP_EmpleadoUpdate
                @id_empleado = {tEmpleado.IdEmpleado},
                @nombre      = {tEmpleado.Nombre},
                @correo      = {tEmpleado.Correo},
                @telefono    = {tEmpleado.Telefono},
                @contrasena_hash = {empleadoActual.ContraseñaHash},
                @puesto      = {tEmpleado.Puesto},
                @id_sucursal = {tEmpleado.IdSucursal},
                @id_rol      = {tEmpleado.IdRol}
        ");

                TempData["SuccessMessage"] = "Empleado actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al actualizar: {ex.Message}";

                ViewData["IdRol"] = new SelectList(_context.TRoles, "IdRol", "Rol", tEmpleado.IdRol);
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", tEmpleado.IdSucursal);

                return View(tEmpleado);
            }
        }

        // ======================================================
        // ELIMINAR (GET)
        // ======================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var empleado = await _context.TEmpleados
                .Include(e => e.IdRolNavigation)
                .Include(e => e.IdSucursalNavigation)
                .FirstOrDefaultAsync(e => e.IdEmpleado == id);

            if (empleado == null) return NotFound();

            // 🚫 No permitir eliminar administradores
            if (empleado.IdRolNavigation.Rol == "Administrador")
            {
                TempData["ErrorMessage"] =
                    "No está permitido eliminar usuarios Administradores.";
                return RedirectToAction(nameof(Index));
            }

            return View(empleado);
        }

        // ======================================================
        // ELIMINAR (POST)
        // ======================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Obtener empleado a eliminar
            var empleado = await _context.TEmpleados
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEmpleado == id);

            if (empleado == null)
            {
                TempData["ErrorMessage"] = "Empleado no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // 🚫 BLOQUEO TOTAL DE ADMINISTRADORES
            if (empleado.IdRol == 3) // Administrador
            {
                TempData["ErrorMessage"] =
                    "No está permitido eliminar empleados con rol Administrador.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC SC_AlquilerVehiculos.SP_EmpleadoDelete
                @id_empleado = {id}
        ");

                TempData["SuccessMessage"] = "Empleado eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    $"No se pudo eliminar el empleado: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        // ======================================================
        private bool TEmpleadoExists(int id)
        {
            return _context.TEmpleados.Any(e => e.IdEmpleado == id);
        }
    }
}
