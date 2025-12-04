using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoPrograAvanzada.Data;
using ProyectoPrograAvanzada.Services;
using ProyectoPrograAvanzada.ViewModels;
using System.Security.Claims;

namespace ProyectoPrograAvanzada.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly DbAlquilerVehiculosContext _context;

        public AccountController(AuthService authService, DbAlquilerVehiculosContext context)
        {
            _authService = authService;
            _context = context;
        }

        // GET: Account/Login
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken] //protección contra ataques CSRF
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var empleado = await _authService.ValidateUserAsync(model.Email, model.Password);

            if (empleado == null)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos");
                return View(model);
            }

            // Crear claims para la sesión
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, empleado.IdEmpleado.ToString()),
                new Claim(ClaimTypes.Name, empleado.Nombre),
                new Claim(ClaimTypes.Email, empleado.Correo),
                new Claim(ClaimTypes.Role, empleado.IdRolNavigation.Rol),
                new Claim("IdSucursal", empleado.IdSucursal.ToString()),
                new Claim("Sucursal", empleado.IdSucursalNavigation.Nombre),
                new Claim("Puesto", empleado.Puesto)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Redirigir a returnUrl o a Home
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Register
        // Permitir acceso sin autenticación SOLO si no hay empleados en el sistema
        public async Task<IActionResult> Register()
        {
            // Si no hay empleados, permitir registro sin autenticación (para crear el primer admin)
            var hayEmpleados = await _context.TEmpleados.AnyAsync();
            
            if (hayEmpleados && !User.IsInRole("Administrador") && !User.IsInRole("Jefe"))
            {
                return RedirectToAction("AccessDenied");
            }

            ViewData["IdSucursal"] = new SelectList(await _context.TSucursales.ToListAsync(), "IdSucursal", "Nombre");
            ViewData["IdRol"] = new SelectList(await _context.TRoles.ToListAsync(), "IdRol", "Rol");
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Si no hay empleados, permitir registro sin autenticación (para crear el primer admin)
            var hayEmpleados = await _context.TEmpleados.AnyAsync();
            
            if (hayEmpleados && !User.IsInRole("Administrador") && !User.IsInRole("Jefe"))
            {
                return RedirectToAction("AccessDenied");
            }

            if (!ModelState.IsValid)
            {
                ViewData["IdSucursal"] = new SelectList(await _context.TSucursales.ToListAsync(), "IdSucursal", "Nombre", model.IdSucursal);
                ViewData["IdRol"] = new SelectList(await _context.TRoles.ToListAsync(), "IdRol", "Rol", model.IdRol);
                return View(model);
            }

            var result = await _authService.RegisterUserAsync(
                model.Nombre,
                model.Email,
                model.Telefono,
                model.Password,
                model.Puesto,
                model.IdSucursal,
                model.IdRol);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                ViewData["IdSucursal"] = new SelectList(await _context.TSucursales.ToListAsync(), "IdSucursal", "Nombre", model.IdSucursal);
                ViewData["IdRol"] = new SelectList(await _context.TRoles.ToListAsync(), "IdRol", "Rol", model.IdRol);
                return View(model);
            }

            // Si es el primer usuario, redirigir a login
            if (!hayEmpleados)
            {
                TempData["SuccessMessage"] = "Administrador creado exitosamente. Por favor inicia sesión.";
                return RedirectToAction("Login", "Account");
            }

            TempData["SuccessMessage"] = "Usuario registrado exitosamente";
            return RedirectToAction("Index", "TEmpleadoes");
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // GET: Account/AccessDenied
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
