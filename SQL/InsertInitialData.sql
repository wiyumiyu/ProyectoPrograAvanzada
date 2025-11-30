USE DB_AlquilerVehiculos;
GO

-- ============================================================
-- SCRIPT PARA INSERTAR DATOS INICIALES
-- ============================================================

-- Verificar si ya existen datos
IF NOT EXISTS (SELECT 1 FROM SC_AlquilerVehiculos.T_Roles)
BEGIN
    PRINT 'Insertando Roles iniciales...';
    
    -- Insertar Roles
    INSERT INTO SC_AlquilerVehiculos.T_Roles (rol) VALUES ('Empleado');
    INSERT INTO SC_AlquilerVehiculos.T_Roles (rol) VALUES ('Jefe');
    INSERT INTO SC_AlquilerVehiculos.T_Roles (rol) VALUES ('Administrador');
    
    PRINT 'Roles insertados correctamente.';
END
ELSE
BEGIN
    PRINT 'Los roles ya existen, saltando inserción.';
END
GO

-- Insertar Sucursales
IF NOT EXISTS (SELECT 1 FROM SC_AlquilerVehiculos.T_Sucursales)
BEGIN
    PRINT 'Insertando Sucursales iniciales...';
    
    EXEC SC_AlquilerVehiculos.SP_SucursalInsert 
        @nombre = 'Sucursal Central San José',
        @telefono = '2222-3333',
        @direccion = 'Avenida Central, San José';

    EXEC SC_AlquilerVehiculos.SP_SucursalInsert 
        @nombre = 'Sucursal Alajuela',
        @telefono = '2433-4444',
        @direccion = 'Centro de Alajuela';

    EXEC SC_AlquilerVehiculos.SP_SucursalInsert 
        @nombre = 'Sucursal Cartago',
        @telefono = '2551-5555',
        @direccion = 'Frente al Parque Central, Cartago';

    EXEC SC_AlquilerVehiculos.SP_SucursalInsert 
        @nombre = 'Sucursal Heredia',
        @telefono = '2237-6666',
        @direccion = 'Calle Real, Heredia';
    
    PRINT 'Sucursales insertadas correctamente.';
END
ELSE
BEGIN
    PRINT 'Las sucursales ya existen, saltando inserción.';
END
GO

-- Insertar Usuario Administrador por defecto
-- Contraseña: Admin123! 
-- Hash generado con ASP.NET Core Identity: AQAAAAIAAYagAAAAELK8qVz5xH0YqX3nR4jJ5xqJ7F5bF9fZ3c0x8v5w2a1b6c9d8e7f0g1h2i3j4k5l6m7n8o9p0q==
-- NOTA: Este es solo un ejemplo, el hash real se generará al registrar el usuario desde la aplicación

IF NOT EXISTS (SELECT 1 FROM SC_AlquilerVehiculos.T_Empleados WHERE correo = 'admin@rentacar.com')
BEGIN
    PRINT 'Insertando Usuario Administrador...';
    
    -- Nota: Debes registrar al administrador desde la aplicación web
    -- O ejecutar esto después de registrar un usuario para obtener el hash correcto
    PRINT 'IMPORTANTE: Debes crear el usuario administrador desde la aplicación web usando el formulario de registro.';
    PRINT 'Usuario sugerido:';
    PRINT '  Email: admin@rentacar.com';
    PRINT '  Contraseña: Admin123!';
    PRINT '  Rol: Administrador';
END
ELSE
BEGIN
    PRINT 'El usuario administrador ya existe.';
END
GO

-- Insertar Tipos de Vehículos
IF NOT EXISTS (SELECT 1 FROM SC_AlquilerVehiculos.T_VehiculosTipos)
BEGIN
    PRINT 'Insertando Tipos de Vehículos...';
    
    EXEC SC_AlquilerVehiculos.SP_VehiculoTipoInsert 
        @descripcion = 'Sedan',
        @tarifa_diaria = 35000.00;

    EXEC SC_AlquilerVehiculos.SP_VehiculoTipoInsert 
        @descripcion = 'SUV',
        @tarifa_diaria = 55000.00;

    EXEC SC_AlquilerVehiculos.SP_VehiculoTipoInsert 
        @descripcion = 'Compacto',
        @tarifa_diaria = 25000.00;

    EXEC SC_AlquilerVehiculos.SP_VehiculoTipoInsert 
        @descripcion = 'Pickup',
        @tarifa_diaria = 60000.00;

    EXEC SC_AlquilerVehiculos.SP_VehiculoTipoInsert 
        @descripcion = 'Van',
        @tarifa_diaria = 70000.00;

    EXEC SC_AlquilerVehiculos.SP_VehiculoTipoInsert 
        @descripcion = 'Deportivo',
        @tarifa_diaria = 120000.00;
    
    PRINT 'Tipos de Vehículos insertados correctamente.';
END
ELSE
BEGIN
    PRINT 'Los tipos de vehículos ya existen, saltando inserción.';
END
GO

PRINT '';
PRINT '============================================================';
PRINT 'DATOS INICIALES CARGADOS CORRECTAMENTE';
PRINT '============================================================';
PRINT '';
PRINT 'Próximos pasos:';
PRINT '1. Ejecutar la aplicación web';
PRINT '2. Ir a /Account/Login';
PRINT '3. Registrar el primer usuario administrador';
PRINT '   - Debe tener rol "Administrador"';
PRINT '   - Sugerido: admin@rentacar.com / Admin123!';
PRINT '';
PRINT '============================================================';
GO
