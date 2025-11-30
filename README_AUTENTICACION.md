# ?? Sistema de Alquiler de Vehículos - Guía de Autenticación

## ?? Características Implementadas

? **Sistema de Login** con correo y contraseña
? **Visualización y ocultación de contraseña** (toggle con icono)
? **Registro de usuarios** (solo para Administradores y Jefes)
? **Autenticación basada en cookies**
? **Autorización por roles**: Empleado, Jefe, Administrador
? **Sesión persistente** con "Recordarme"
? **Hash de contraseñas** con ASP.NET Core Identity
? **Menú dinámico** según rol del usuario
? **Página de acceso denegado**
? **Integración con stored procedures** de la BD

---

## ?? Pasos para Configurar el Sistema

### 1?? Ejecutar Script de Datos Iniciales

Abre SQL Server Management Studio y ejecuta el script:
```
SQL/InsertInitialData.sql
```

Este script creará:
- ? 3 Roles (Empleado, Jefe, Administrador)
- ? 4 Sucursales de ejemplo
- ? 6 Tipos de vehículos con tarifas

### 2?? Iniciar la Aplicación

```bash
dotnet run
```

O presiona F5 en Visual Studio.

### 3?? Crear el Primer Usuario Administrador

1. La aplicación te redirigirá a `/Account/Login`
2. Como no hay usuarios aún, debes crear el primero **temporalmente** modificando la restricción de registro
3. **Opción alternativa**: Insertar manualmente el primer admin en SQL:

```sql
USE DB_AlquilerVehiculos;
GO

-- Obtener IDs
DECLARE @idRolAdmin INT = (SELECT id_rol FROM SC_AlquilerVehiculos.T_Roles WHERE rol = 'Administrador');
DECLARE @idSucursal INT = (SELECT TOP 1 id_sucursal FROM SC_AlquilerVehiculos.T_Sucursales);

-- Insertar administrador
-- Contraseña: Admin123!
-- Hash generado con Identity (debes generarlo desde la app o usar el ejemplo)
EXEC SC_AlquilerVehiculos.SP_EmpleadoInsert
    @nombre = 'Administrador Sistema',
    @correo = 'admin@rentacar.com',
    @telefono = '8888-9999',
    @contrasena_hash = N'AQAAAAIAAYagAAAAELK8qVz5xH0YqX3nR4jJ5xqJ7F5bF9fZ3c0x8v5w2a1b6c9d8e7f0g1h2i3j4k5l6m7n8o9p0q==',
    @puesto = 'Administrador General',
    @id_sucursal = @idSucursal,
    @id_rol = @idRolAdmin;
GO
```

> ?? **NOTA IMPORTANTE**: El hash de ejemplo anterior NO funcionará. Debes:
> - Temporalmente quitar el atributo `[Authorize]` del método `Register` en `AccountController`
> - Registrar el primer admin desde la web
> - Volver a agregar el `[Authorize(Roles = "Administrador,Jefe")]`

### 4?? Iniciar Sesión

1. Ve a `https://localhost:XXXX/Account/Login`
2. Ingresa:
   - **Correo**: admin@rentacar.com
   - **Contraseña**: Admin123!
3. Marca "Recordarme" si deseas sesión persistente por 30 días

---

## ?? Roles y Permisos

### ?? Empleado
- ? Ver y gestionar: Clientes, Vehículos, Sucursales, Tipos de Vehículos
- ? Crear alquileres, detalles y recibos
- ? NO puede gestionar empleados
- ? NO puede ver bitácoras
- ? NO puede registrar usuarios

### ?? Jefe
- ? **TODO** lo que puede hacer un Empleado
- ? Gestionar empleados
- ? Ver bitácoras de auditoría
- ? Registrar nuevos usuarios
- ? Modificar y eliminar alquileres

### ?? Administrador
- ? **ACCESO COMPLETO** al sistema
- ? Control total sobre todos los módulos
- ? Puede registrar usuarios con cualquier rol

---

## ?? Características de la UI

### Página de Login
- ?? Diseño moderno con gradientes azules
- ??? **Toggle de contraseña** para mostrar/ocultar
- ?? Responsive (se adapta a móviles)
- ? Animación de entrada suave
- ?? Validación de formularios del lado del cliente

### Menú de Navegación
- ?? Muestra nombre, puesto, rol y sucursal del usuario
- ?? Opciones dinámicas según permisos
- ?? Botón de cerrar sesión
- ?? Icono de usuario con dropdown

### Dashboard (Home)
- ?? Tarjetas de acceso rápido a módulos principales
- ?? Panel administrativo (solo para Jefe/Admin)
- ? Efectos hover en las tarjetas
- ?? Colores diferenciados por sección

---

## ?? Estructura Técnica

### Archivos Creados

```
?? ViewModels/
   ??? LoginViewModel.cs          (Modelo para login)
   ??? RegisterViewModel.cs       (Modelo para registro)

?? Services/
   ??? AuthService.cs             (Lógica de autenticación)

?? Controllers/
   ??? AccountController.cs       (Login, Register, Logout)

?? Views/Account/
   ??? Login.cshtml               (Vista de login con toggle)
   ??? Register.cshtml            (Formulario de registro)
   ??? AccessDenied.cshtml        (Página de acceso denegado)

?? SQL/
   ??? InsertInitialData.sql      (Datos iniciales)
```

### Archivos Modificados

```
?? Program.cs                     (Configuración de autenticación)
?? Models/TEmpleado.cs            (ContraseñaHash corregido)
?? Data/DbAlquilerVehiculosContext.cs (Mapeo actualizado)
?? Views/Shared/_Layout.cshtml    (Menú con usuario)
?? Views/Home/Index.cshtml        (Dashboard personalizado)
?? Controllers/HomeController.cs  (Atributo [Authorize])
```

---

## ?? Pruebas Recomendadas

### ? Probar Funcionalidad de Login
1. Intentar login con credenciales incorrectas ? Ver mensaje de error
2. Login exitoso ? Redirige a Home
3. Verificar toggle de contraseña funciona
4. Probar "Recordarme" ? Cerrar navegador y abrir de nuevo

### ? Probar Autorización
1. Como Empleado: Intentar acceder a `/TEmpleadoes` ? Debe negar acceso
2. Como Jefe: Acceder a `/TEmpleadoes` ? Debe permitir
3. Sin login: Acceder a cualquier página ? Redirige a Login

### ? Probar Registro de Usuarios
1. Como Admin/Jefe: Ir a `/Account/Register`
2. Registrar un empleado nuevo
3. Cerrar sesión
4. Iniciar sesión con el nuevo usuario

---

## ?? Notas Importantes

### Contraseñas
- ? Se almacenan hasheadas con **PBKDF2** (ASP.NET Core Identity)
- ? Validación: mínimo 6 caracteres
- ? No se guardan en texto plano

### Sesiones
- ? Duración: 8 horas (sin "Recordarme")
- ?? Con "Recordarme": 30 días
- ?? Sliding expiration: la sesión se renueva con cada petición

### Seguridad
- ?? Cookies HttpOnly (no accesibles desde JavaScript)
- ?? CSRF protection en todos los formularios
- ?? Páginas protegidas con `[Authorize]`

---

## ?? Solución de Problemas

### Error: "No se puede conectar a la base de datos"
? Verifica el connection string en `appsettings.json`
? Asegúrate que SQL Server esté corriendo

### Error: "Correo o contraseña incorrectos"
? Verifica que el usuario exista en la tabla `T_Empleados`
? Confirma que el campo `contrasena_hash` contenga un hash válido

### Error: "Access Denied"
? Verifica que el usuario tenga el rol correcto
? Revisa los atributos `[Authorize(Roles = "...")]` en los controllers

---

## ?? Soporte

Para más información o problemas, contacta al equipo de desarrollo:
- Víctor Julio Rodríguez Cerdas
- Mariana Torres Valverde
- Carlos Esteban Solís Alvarado

---

**¡El sistema está listo para usarse! ??**
