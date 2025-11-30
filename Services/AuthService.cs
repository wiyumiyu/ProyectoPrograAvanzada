using Microsoft.AspNetCore.Identity;
using ProyectoPrograAvanzada.Data;
using ProyectoPrograAvanzada.Models;
using Microsoft.EntityFrameworkCore;

namespace ProyectoPrograAvanzada.Services
{
    public class AuthService
    {
        private readonly DbAlquilerVehiculosContext _context;
        private readonly IPasswordHasher<TEmpleado> _passwordHasher;

        public AuthService(DbAlquilerVehiculosContext context, IPasswordHasher<TEmpleado> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Verifica las credenciales del usuario
        /// </summary>
        public async Task<TEmpleado?> ValidateUserAsync(string email, string password)
        {
            var empleado = await _context.TEmpleados
                .Include(e => e.IdRolNavigation)
                .Include(e => e.IdSucursalNavigation)
                .FirstOrDefaultAsync(e => e.Correo == email);

            if (empleado == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(empleado, empleado.ContraseñaHash, password);
            
            return result == PasswordVerificationResult.Success ? empleado : null;
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        public async Task<(bool Success, string Message, int? IdEmpleado)> RegisterUserAsync(
            string nombre, 
            string correo, 
            string? telefono,
            string password, 
            string puesto, 
            int idSucursal, 
            int idRol)
        {
            // Verificar si el correo ya existe
            var existingUser = await _context.TEmpleados
                .FirstOrDefaultAsync(e => e.Correo == correo);

            if (existingUser != null)
                return (false, "El correo electrónico ya está registrado", null);

            // Crear nuevo empleado temporal para hashear contraseña
            var tempEmpleado = new TEmpleado();
            var hashedPassword = _passwordHasher.HashPassword(tempEmpleado, password);

            // Insertar usando stored procedure
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC SC_AlquilerVehiculos.SP_EmpleadoInsert
                        @nombre = {nombre},
                        @correo = {correo},
                        @telefono = {telefono},
                        @contrasena_hash = {hashedPassword},
                        @puesto = {puesto},
                        @id_sucursal = {idSucursal},
                        @id_rol = {idRol}
                ");

                // Obtener el ID del empleado recién creado
                var nuevoEmpleado = await _context.TEmpleados
                    .FirstOrDefaultAsync(e => e.Correo == correo);

                return (true, "Usuario registrado exitosamente", nuevoEmpleado?.IdEmpleado);
            }
            catch (Exception ex)
            {
                return (false, $"Error al registrar usuario: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Obtiene un empleado por ID
        /// </summary>
        public async Task<TEmpleado?> GetEmpleadoByIdAsync(int id)
        {
            return await _context.TEmpleados
                .Include(e => e.IdRolNavigation)
                .Include(e => e.IdSucursalNavigation)
                .FirstOrDefaultAsync(e => e.IdEmpleado == id);
        }
    }
}
