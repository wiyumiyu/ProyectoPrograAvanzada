/* ============================================================
   BASE DE DATOS Alquiler Vehículos
   Desarrollo:
		Víctor Julio Rodríguez Cerdas
		Mariana Torres Valverde
		Carlos Esteban Solís Alvarado
	Surpervisor:
		Manuel Sanabría Montoya
	Autorizó:
		Manuel Sanabría Montoya
	Versión:
		2.0
	Descripción:
		Base de datos diseñada para administrar sucursales, empleados, 
		clientes, vehículos, alquileres y recibos de un sistema de renta de autos, 
		asegurando integridad de datos y auditoría de operaciones. 
		
   ============================================================ */

USE MASTER --  Moverse a la base de datos MASTER
GO


CREATE DATABASE DB_AlquilerVehiculos --  Crear la base de datos principal del sistema
GO

USE DB_AlquilerVehiculos -- Cambiar el contexto a la base recién creada
GO

CREATE SCHEMA SC_AlquilerVehiculos -- Crear el esquema principal del proyecto
GO
/* ============================================================
   LOGINS Y USERS
   ============================================================ */

-- Login para empleados
CREATE LOGIN Login_Empleado 
WITH PASSWORD = 'Empleado2025*', CHECK_POLICY=ON, CHECK_EXPIRATION=ON;
GO

-- Login para jefe
CREATE LOGIN Login_Jefe 
WITH PASSWORD = 'Jefe2025*', CHECK_POLICY=ON, CHECK_EXPIRATION=ON;
GO

-- Login para administrador técnico
CREATE LOGIN Login_AdminApp
WITH PASSWORD = 'AdminApp2025*', CHECK_POLICY=ON, CHECK_EXPIRATION=ON;
GO




/* USUARIOS EN LA BD */
USE DB_AlquilerVehiculos;
GO

CREATE USER User_Empleado FOR LOGIN Login_Empleado;
CREATE USER User_Jefe FOR LOGIN Login_Jefe;
CREATE USER User_AdminApp FOR LOGIN Login_AdminApp;
GO


/* ============================================================
   ROLES PERSONALIZADOS DEL SISTEMA
   ============================================================ */

-- Rol empleados
CREATE ROLE rol_empleado;
GO

-- Rol jefe
CREATE ROLE rol_jefe;
GO

-- Rol administrador de aplicación
CREATE ROLE rol_adminApp;
GO





-- EXECUTE AS LOGIN = 'Login_AdminApp';

/* ============================================================
   TABLAS ORDENADAS DE INDEPENDIENTES → DEPENDIENTES
   ============================================================ */


/* ============================================================
   1. Tabla de sucursales (independiente)
   ============================================================ */
CREATE TABLE SC_AlquilerVehiculos.T_Sucursales (
    id_sucursal INT IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    telefono VARCHAR(20),
    direccion VARCHAR(150) NOT NULL
)
GO

-- Agregar PK a T_Sucursales
ALTER TABLE SC_AlquilerVehiculos.T_Sucursales
ADD CONSTRAINT PK_Sucursales PRIMARY KEY(id_sucursal)
GO


/* ============================================================
   2. Tabla de clientes (independiente)
   ============================================================ */
CREATE TABLE SC_AlquilerVehiculos.T_Clientes (
    id_cliente INT IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    cedula VARCHAR(25) NOT NULL UNIQUE,
    telefono VARCHAR(20),
    email VARCHAR(100),
    direccion VARCHAR(150)
)
GO

-- Agregar PK a T_Clientes
ALTER TABLE SC_AlquilerVehiculos.T_Clientes
ADD CONSTRAINT PK_Clientes PRIMARY KEY(id_cliente)
GO

/* ============================================================
   3. Tabla de roles 
   ============================================================ */
   CREATE TABLE SC_AlquilerVehiculos.T_Roles (
    id_rol INT IDENTITY(1,1),
    rol VARCHAR(50) NOT NULL,
)
GO

-- Agregar PK a T_Empleados
ALTER TABLE SC_AlquilerVehiculos.T_Roles
ADD CONSTRAINT PK_Roles PRIMARY KEY(id_rol)
GO

/* ============================================================
   3. Tabla de empleados (depende de sucursales)
   ============================================================ */
CREATE TABLE SC_AlquilerVehiculos.T_Empleados (
    id_empleado INT IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    correo VARCHAR(100) NOT NULL,
    telefono VARCHAR(20),
    contrasena VARCHAR(100) NOT NULL,
    puesto VARCHAR(50) NOT NULL,
    id_sucursal INT NOT NULL,
    id_rol INT NOT NULL
)
GO

-- Agregar PK a T_Empleados
ALTER TABLE SC_AlquilerVehiculos.T_Empleados
ADD CONSTRAINT PK_Empleados PRIMARY KEY(id_empleado)
GO

-- Agregar FK que referencia sucursales en T_Empleados
ALTER TABLE SC_AlquilerVehiculos.T_Empleados
ADD CONSTRAINT FK_Empleados_Sucursales
FOREIGN KEY(id_sucursal) REFERENCES SC_AlquilerVehiculos.T_Sucursales(id_sucursal)
GO

-- Agregar FK que referencia sucursales en T_Empleados
ALTER TABLE SC_AlquilerVehiculos.T_Empleados
ADD CONSTRAINT FK_Empleados_Roles
FOREIGN KEY(id_rol) REFERENCES SC_AlquilerVehiculos.T_Roles(id_rol)
GO

ALTER TABLE SC_AlquilerVehiculos.T_Empleados
ADD CONSTRAINT DF_Empleados_Rol DEFAULT 1 FOR id_rol;

/* ============================================================
   4. Tipos de vehículos (independiente)
   ============================================================ */
CREATE TABLE SC_AlquilerVehiculos.T_VehiculosTipos (
    id_tipo INT IDENTITY(1,1),
    descripcion VARCHAR(50) NOT NULL,
    tarifa_diaria DECIMAL(10,2) NOT NULL CHECK (tarifa_diaria >= 0)
)
GO

-- Agregar PK a T_VehiculosTipos
ALTER TABLE SC_AlquilerVehiculos.T_VehiculosTipos
ADD CONSTRAINT PK_VehiculosTipos PRIMARY KEY(id_tipo)
GO


/* ============================================================
   5. Vehículos (depende de tipos y sucursales)
   ============================================================ */
CREATE TABLE SC_AlquilerVehiculos.T_Vehiculos (
    id_vehiculo INT IDENTITY(1,1),
    placa VARCHAR(20) NOT NULL UNIQUE,
    marca VARCHAR(50) NOT NULL,
    modelo VARCHAR(50) NOT NULL,
    anio INT NOT NULL CHECK (anio >= 1950),
    estado VARCHAR(20) NOT NULL CHECK (estado IN ('Disponible','Alquilado','En reparacion','Fuera de servicio','Reservado')),
    id_tipo INT NOT NULL,
    id_sucursal INT NOT NULL
)
GO

-- Agregar PK a T_Vehiculos
ALTER TABLE SC_AlquilerVehiculos.T_Vehiculos
ADD CONSTRAINT PK_Vehiculos PRIMARY KEY(id_vehiculo)
GO

-- Agregar FK que referencia tipos en T_Vehiculos
ALTER TABLE SC_AlquilerVehiculos.T_Vehiculos
ADD CONSTRAINT FK_Vehiculos_Tipos
FOREIGN KEY(id_tipo) REFERENCES SC_AlquilerVehiculos.T_VehiculosTipos(id_tipo)
GO

-- Agregar FK que referencia sucursales en T_Vehiculos
ALTER TABLE SC_AlquilerVehiculos.T_Vehiculos
ADD CONSTRAINT FK_Vehiculos_Sucursales
FOREIGN KEY(id_sucursal) REFERENCES SC_AlquilerVehiculos.T_Sucursales(id_sucursal)
GO


/* ============================================================
   6. Alquileres (depende de clientes, empleados, sucursales)
   ============================================================ */
CREATE TABLE SC_AlquilerVehiculos.T_Alquileres (
    id_alquiler INT IDENTITY(1,1),
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE,
    iva DECIMAL(10,2) NOT NULL DEFAULT(0),
    id_cliente INT NOT NULL,
    id_empleado INT NOT NULL,
    id_sucursal INT NOT NULL,
    estado VARCHAR(20) NOT NULL CHECK (estado IN ('Activo','Finalizado','Cancelado'))
)
GO

-- Agregar PK a T_Alquileres
ALTER TABLE SC_AlquilerVehiculos.T_Alquileres
ADD CONSTRAINT PK_Alquileres PRIMARY KEY(id_alquiler)
GO

-- Agregar FK que referencia clientes en T_Alquileres
ALTER TABLE SC_AlquilerVehiculos.T_Alquileres
ADD CONSTRAINT FK_Alquileres_Clientes
FOREIGN KEY(id_cliente) REFERENCES SC_AlquilerVehiculos.T_Clientes(id_cliente)
GO

-- Agregar FK que referencia empleados en T_Alquileres
ALTER TABLE SC_AlquilerVehiculos.T_Alquileres
ADD CONSTRAINT FK_Alquileres_Empleados
FOREIGN KEY(id_empleado) REFERENCES SC_AlquilerVehiculos.T_Empleados(id_empleado)
GO

-- Agregar FK que referencia sucursales en T_Alquileres
ALTER TABLE SC_AlquilerVehiculos.T_Alquileres
ADD CONSTRAINT FK_Alquileres_Sucursales
FOREIGN KEY(id_sucursal) REFERENCES SC_AlquilerVehiculos.T_Sucursales(id_sucursal)
GO


/* ============================================================
   7. Detalles de alquileres (depende de alquileres y vehículos)
   ============================================================ */
CREATE TABLE SC_AlquilerVehiculos.T_AlquileresDetalles (
    id_detalle INT IDENTITY(1,1),
    id_alquiler INT NOT NULL,
    id_vehiculo INT NOT NULL,
    tarifa_diaria DECIMAL(10,2) NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    subtotal DECIMAL(10,2) NOT NULL
)
GO

-- Agregar PK a T_AlquileresDetalles
ALTER TABLE SC_AlquilerVehiculos.T_AlquileresDetalles
ADD CONSTRAINT PK_AlquileresDetalles PRIMARY KEY(id_detalle)
GO

-- Agregar FK que referencia alquileres
ALTER TABLE SC_AlquilerVehiculos.T_AlquileresDetalles
ADD CONSTRAINT FK_AlqDet_Alquileres
FOREIGN KEY(id_alquiler) REFERENCES SC_AlquilerVehiculos.T_Alquileres(id_alquiler)
GO

-- Agregar FK que referencia vehículos
ALTER TABLE SC_AlquilerVehiculos.T_AlquileresDetalles
ADD CONSTRAINT FK_AlqDet_Vehiculos
FOREIGN KEY(id_vehiculo) REFERENCES SC_AlquilerVehiculos.T_Vehiculos(id_vehiculo)
GO


/* ============================================================
   8. Recibos (independiente)
   ============================================================ */
CREATE TABLE SC_AlquilerVehiculos.T_Recibos (
    id_recibo INT IDENTITY(1,1),
    fecha_pago DATE NOT NULL,
    monto DECIMAL(10,2) NOT NULL,
    metodo VARCHAR(30) NOT NULL CHECK (metodo IN ('efectivo','tarjeta','transferencia'))
)
GO

-- Agregar PK a T_Recibos
ALTER TABLE SC_AlquilerVehiculos.T_Recibos
ADD CONSTRAINT PK_Recibos PRIMARY KEY(id_recibo)
GO


/* ============================================================
   9. AlquileresRecibos (depende de recibos y alquileres)
   ============================================================ */
CREATE TABLE SC_AlquilerVehiculos.T_AlquileresRecibos (
    id_recibo INT NOT NULL,
    id_alquiler INT NOT NULL
)
GO

-- Agregar PK compuesta a T_AlquileresRecibos
ALTER TABLE SC_AlquilerVehiculos.T_AlquileresRecibos
ADD CONSTRAINT PK_AlquileresRecibos PRIMARY KEY(id_recibo, id_alquiler)
GO

-- Agregar FK que referencia recibos
ALTER TABLE SC_AlquilerVehiculos.T_AlquileresRecibos
ADD CONSTRAINT FK_AlqRec_Recibos
FOREIGN KEY(id_recibo) REFERENCES SC_AlquilerVehiculos.T_Recibos(id_recibo)
GO

-- Agregar FK que referencia alquileres
ALTER TABLE SC_AlquilerVehiculos.T_AlquileresRecibos
ADD CONSTRAINT FK_AlqRec_Alquileres
FOREIGN KEY(id_alquiler) REFERENCES SC_AlquilerVehiculos.T_Alquileres(id_alquiler)
GO


/* ============================================================
   10. Bitácora (independiente)
   ============================================================ */
CREATE TABLE SC_AlquilerVehiculos.T_Bitacora (
    id_bitacora INT IDENTITY(1,1),
    tabla VARCHAR(100) NOT NULL,
    operacion VARCHAR(10) NOT NULL,
    clave_primaria VARCHAR(200) NOT NULL,
    valores_antes NVARCHAR(MAX) NULL,
    valores_despues NVARCHAR(MAX) NULL,
    usuario_sql VARCHAR(200) NOT NULL DEFAULT SUSER_SNAME(),
    fecha DATETIME NOT NULL DEFAULT GETDATE()
)
GO

-- Agregar PK a T_Bitacora
ALTER TABLE SC_AlquilerVehiculos.T_Bitacora
ADD CONSTRAINT PK_Bitacora PRIMARY KEY(id_bitacora)
GO

/* ============================================================
   ÍNDICES RECOMENDADOS
   ============================================================ */

/* Índice para acelerar búsquedas por nombre de sucursal */
CREATE INDEX IX_sucursales_nombre 
    ON SC_AlquilerVehiculos.T_Sucursales(nombre)
GO

/* Índice para consultas rápidas por correo de empleado */
CREATE INDEX IX_empleados_correo 
    ON SC_AlquilerVehiculos.T_Empleados(correo)
GO

/* Índice para filtrar empleados por sucursal */
CREATE INDEX IX_empleados_sucursal 
    ON SC_AlquilerVehiculos.T_Empleados(id_sucursal)
GO

/* Índice para búsquedas por cédula de cliente */
CREATE INDEX IX_clientes_cedula 
    ON SC_AlquilerVehiculos.T_Clientes(cedula)
GO

/* Índice para búsquedas por tipo de vehículo */
CREATE INDEX IX_vehTipos_desc 
    ON SC_AlquilerVehiculos.T_VehiculosTipos(descripcion)
GO

/* Índice para búsquedas rápidas por placa */
CREATE INDEX IX_vehiculos_placa 
    ON SC_AlquilerVehiculos.T_Vehiculos(placa)
GO

/* Índice para obtener vehículos por sucursal */
CREATE INDEX IX_vehiculos_sucursal 
    ON SC_AlquilerVehiculos.T_Vehiculos(id_sucursal)
GO

/* Índice para filtrar vehículos por tipo */
CREATE INDEX IX_vehiculos_tipo 
    ON SC_AlquilerVehiculos.T_Vehiculos(id_tipo)
GO

/* Índice para buscar alquileres por cliente */
CREATE INDEX IX_alquileres_cliente 
    ON SC_AlquilerVehiculos.T_Alquileres(id_cliente)
GO

/* Índice para filtrar alquileres por estado (Activo, Finalizado, Cancelado) */
CREATE INDEX IX_alquileres_estado 
    ON SC_AlquilerVehiculos.T_Alquileres(estado)
GO

/* Índice para obtener detalles de alquileres según su id_alquiler */
CREATE INDEX IX_alqdet_alquiler 
    ON SC_AlquilerVehiculos.T_AlquileresDetalles(id_alquiler)
GO

/* Índice para buscar relaciones entre recibos y alquileres */
CREATE INDEX IX_alqrec_alquiler 
    ON SC_AlquilerVehiculos.T_AlquileresRecibos(id_alquiler)
GO


/* ============================================================
   VISTAS (VIEWS)
   ============================================================ */

/* Vista que muestra vehículos disponibles según su estado y sucursal */
CREATE VIEW SC_AlquilerVehiculos.VW_VehiculosDisponibles AS
SELECT 
    v.id_vehiculo, 
    v.placa, 
    v.marca, 
    v.modelo, 
    v.estado, 
    s.nombre AS sucursal
FROM SC_AlquilerVehiculos.T_Vehiculos v
JOIN SC_AlquilerVehiculos.T_Sucursales s 
    ON v.id_sucursal = s.id_sucursal
WHERE v.estado NOT IN ('Alquilado','En reparacion')
GO


/* Vista que combina alquileres, clientes, vehículos y detalles del alquiler */
CREATE VIEW SC_AlquilerVehiculos.VW_AlquileresDetalleCompleto AS
SELECT 
    a.id_alquiler, 
    c.nombre AS cliente, 
    v.placa, 
    d.fecha_inicio, 
    d.fecha_fin, 
    d.subtotal
FROM SC_AlquilerVehiculos.T_Alquileres a
JOIN SC_AlquilerVehiculos.T_Clientes c 
    ON a.id_cliente = c.id_cliente
JOIN SC_AlquilerVehiculos.T_AlquileresDetalles d 
    ON a.id_alquiler = d.id_alquiler
JOIN SC_AlquilerVehiculos.T_Vehiculos v 
    ON d.id_vehiculo = v.id_vehiculo
GO



/* ============================================================
   STORED PROCEDURES
   ============================================================ */

   /*SUCURSALES***********************************************/

   -- SP para insertar una sucursal y registrar la operación en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_SucursalInsert
    @nombre     VARCHAR(100),
    @telefono   VARCHAR(20),
    @direccion  VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes adicionales

    BEGIN TRY
        DECLARE @id INT;  -- ID generado para la nueva sucursal

        -- Inserta registro en T_Sucursales
        INSERT INTO SC_AlquilerVehiculos.T_Sucursales(nombre, telefono, direccion)
        VALUES(@nombre, @telefono, @direccion);

        SET @id = SCOPE_IDENTITY(); -- Obtiene ID generado

        -- Registra auditoría del INSERT
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
        (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Sucursales',  -- Tabla modificada
            'INSERT',        -- Operación realizada
            @id,             -- PK afectada
            NULL,            -- No hay valores antes en un insert
            (
                SELECT @id AS id_sucursal, @nombre AS nombre, @telefono AS telefono, @direccion AS direccion
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            )                -- Valores después
        );

        SELECT @id AS id_sucursal; -- Retorna el ID insertado
    END TRY
    BEGIN CATCH
        THROW;  -- Reenvía el error
    END CATCH;
END
GO
-- SP para actualizar una sucursal y registrar cambios en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_SucursalUpdate
    @id_sucursal INT,
    @nombre      VARCHAR(100),
    @telefono    VARCHAR(20),
    @direccion   VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes adicionales

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX);   -- Datos antes del cambio
        DECLARE @despues NVARCHAR(MAX); -- Datos después del cambio

        -- Obtiene datos antes del UPDATE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_Sucursales
             WHERE id_sucursal = @id_sucursal
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Ejecuta el UPDATE
        UPDATE SC_AlquilerVehiculos.T_Sucursales
        SET nombre    = @nombre,
            telefono  = @telefono,
            direccion = @direccion
        WHERE id_sucursal = @id_sucursal;

        -- Obtiene datos después del UPDATE
        SELECT @despues =
            (SELECT * FROM SC_AlquilerVehiculos.T_Sucursales
             WHERE id_sucursal = @id_sucursal
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Registra auditoría del UPDATE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
        (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Sucursales',   -- Tabla afectada
            'UPDATE',         -- Tipo de operación
            @id_sucursal,     -- PK modificada
            @antes,           -- Estado anterior
            @despues          -- Estado posterior
        );
    END TRY
    BEGIN CATCH
        THROW;  -- Reenvía error
    END CATCH;
END
GO

-- SP para eliminar una sucursal y registrar el borrado en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_SucursalDelete
    @id_sucursal INT
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes adicionales

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX); -- Datos previos a eliminación

        -- Obtiene valores antes del DELETE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_Sucursales
             WHERE id_sucursal = @id_sucursal
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Elimina la sucursal
        DELETE FROM SC_AlquilerVehiculos.T_Sucursales
        WHERE id_sucursal = @id_sucursal;

        -- Registra auditoría del DELETE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
        (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Sucursales',  -- Tabla afectada
            'DELETE',        -- Tipo de operación
            @id_sucursal,    -- PK eliminada
            @antes,          -- Estado previo
            NULL             -- No hay estado después
        );
    END TRY
    BEGIN CATCH
        THROW;  -- Reenvía error
    END CATCH;
END
GO


/*EMPLEADOS***********************************************/

-- SP para insertar un empleado y registrar la operación en la bitácora
ALTER PROCEDURE SC_AlquilerVehiculos.SP_EmpleadoInsert
    @nombre      VARCHAR(100),
    @correo      VARCHAR(100),
    @telefono    VARCHAR(20),
    @contrasena  VARCHAR(100),
    @puesto      VARCHAR(50),
    @id_sucursal INT,
    @id_rol      INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @id INT;

    INSERT INTO SC_AlquilerVehiculos.T_Empleados
        (nombre, correo, telefono, contrasena, puesto, id_sucursal, id_rol)
    VALUES
        (@nombre, @correo, @telefono, @contrasena, @puesto, @id_sucursal, @id_rol);

    SET @id = SCOPE_IDENTITY();

    INSERT INTO SC_AlquilerVehiculos.T_Bitacora
    (tabla, operacion, clave_primaria, valores_antes, valores_despues)
    VALUES(
        'T_Empleados',
        'INSERT',
        @id,
        NULL,
        (
            SELECT @id AS id_empleado,
                   @nombre AS nombre,
                   @correo AS correo,
                   @telefono AS telefono,
                   @contrasena AS contrasena,
                   @puesto AS puesto,
                   @id_sucursal AS id_sucursal,
                   @id_rol AS id_rol
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        )
    );
END
GO

-- SP para actualizar un empleado y registrar cambios en la bitácora
ALTER PROCEDURE SC_AlquilerVehiculos.SP_EmpleadoUpdate
    @id_empleado INT,
    @nombre      VARCHAR(100),
    @correo      VARCHAR(100),
    @telefono    VARCHAR(20),
    @contrasena  VARCHAR(100),
    @puesto      VARCHAR(50),
    @id_sucursal INT,
    @id_rol      INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @antes NVARCHAR(MAX);
    DECLARE @despues NVARCHAR(MAX);

    SELECT @antes =
        (SELECT * FROM SC_AlquilerVehiculos.T_Empleados
         WHERE id_empleado = @id_empleado
         FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

    UPDATE SC_AlquilerVehiculos.T_Empleados
    SET nombre      = @nombre,
        correo      = @correo,
        telefono    = @telefono,
        contrasena  = @contrasena,
        puesto      = @puesto,
        id_sucursal = @id_sucursal,
        id_rol      = @id_rol
    WHERE id_empleado = @id_empleado;

    SELECT @despues =
        (SELECT * FROM SC_AlquilerVehiculos.T_Empleados
         WHERE id_empleado = @id_empleado
         FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

    INSERT INTO SC_AlquilerVehiculos.T_Bitacora
    (tabla, operacion, clave_primaria, valores_antes, valores_despues)
    VALUES(
        'T_Empleados',
        'UPDATE',
        @id_empleado,
        @antes,
        @despues
    );
END
GO

-- SP para eliminar un empleado y registrar el borrado en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_EmpleadoDelete
    @id_empleado INT
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes adicionales

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX);  -- Datos antes de eliminar

        -- Obtiene datos previos al DELETE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_Empleados
             WHERE id_empleado = @id_empleado
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Ejecuta el borrado
        DELETE FROM SC_AlquilerVehiculos.T_Empleados
        WHERE id_empleado = @id_empleado;

        -- Registra auditoría del DELETE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
        (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Empleados',   -- Tabla
            'DELETE',        -- Operación realizada
            @id_empleado,    -- PK eliminada
            @antes,          -- Estado previo
            NULL             -- No hay estado posterior
        );
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error
    END CATCH;
END
GO


/*CLIENTES***********************************************/

-- SP para insertar un cliente y registrar la operación en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_ClienteInsert
    @nombre     VARCHAR(100),
    @cedula     VARCHAR(25),
    @telefono   VARCHAR(20),
    @email      VARCHAR(100),
    @direccion  VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes extra

    BEGIN TRY
        DECLARE @id INT;  -- ID generado del cliente

        -- Inserta el cliente en la tabla
        INSERT INTO SC_AlquilerVehiculos.T_Clientes(nombre, cedula, telefono, email, direccion)
        VALUES(@nombre, @cedula, @telefono, @email, @direccion);

        SET @id = SCOPE_IDENTITY(); -- Obtiene el ID insertado

        -- Registra auditoría del INSERT
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
        (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Clientes',   -- Tabla afectada
            'INSERT',       -- Tipo de operación
            @id,            -- PK creada
            NULL,           -- No hay valores antes en un INSERT
            (
                SELECT @id AS id_cliente,
                       @nombre AS nombre,
                       @cedula AS cedula,
                       @telefono AS telefono,
                       @email AS email,
                       @direccion AS direccion
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            )               -- Estado después
        );

        SELECT @id AS id_cliente;  -- Retorna ID generado
    END TRY
    BEGIN CATCH
        THROW;  -- Propaga el error
    END CATCH;
END
GO

-- SP para actualizar un cliente y registrar la modificación en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_ClienteUpdate
    @id_cliente INT,
    @nombre     VARCHAR(100),
    @cedula     VARCHAR(25),
    @telefono   VARCHAR(20),
    @email      VARCHAR(100),
    @direccion  VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes extra

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX);   -- Valores antes del cambio
        DECLARE @despues NVARCHAR(MAX); -- Valores después del cambio

        -- Obtiene estado previo
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_Clientes
             WHERE id_cliente = @id_cliente
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Actualiza el registro
        UPDATE SC_AlquilerVehiculos.T_Clientes
        SET nombre     = @nombre,
            cedula     = @cedula,
            telefono   = @telefono,
            email      = @email,
            direccion  = @direccion
        WHERE id_cliente = @id_cliente;

        -- Obtiene estado posterior
        SELECT @despues =
            (SELECT * FROM SC_AlquilerVehiculos.T_Clientes
             WHERE id_cliente = @id_cliente
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Registra auditoría del UPDATE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
        (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Clientes',   -- Tabla afectada
            'UPDATE',       -- Operación
            @id_cliente,    -- PK afectada
            @antes,         -- Estado previo
            @despues        -- Estado posterior
        );
    END TRY
    BEGIN CATCH
        THROW;  -- Reenvía error
    END CATCH;
END
GO

-- SP para eliminar un cliente y registrar el borrado en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_ClienteDelete
    @id_cliente INT
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes extra

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX);  -- Valores antes del borrado

        -- Obtiene datos previos al DELETE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_Clientes
             WHERE id_cliente = @id_cliente
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Elimina el registro
        DELETE FROM SC_AlquilerVehiculos.T_Clientes
        WHERE id_cliente = @id_cliente;

        -- Registra auditoría del DELETE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
        (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Clientes',   -- Tabla
            'DELETE',       -- Operación
            @id_cliente,    -- PK eliminada
            @antes,         -- Valores antes
            NULL            -- No hay valores después
        );
    END TRY
    BEGIN CATCH
        THROW;  -- Reenvía el error
    END CATCH;
END
GO


/*VEHICULOTIPO***********************************************/

-- SP para insertar un tipo de vehículo y registrar la operación en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_VehiculoTipoInsert
    @descripcion   VARCHAR(50),
    @tarifa_diaria DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes extra

    BEGIN TRY
        DECLARE @id INT;  -- Almacena ID generado

        -- Inserta un nuevo tipo de vehículo
        INSERT INTO SC_AlquilerVehiculos.T_VehiculosTipos(descripcion, tarifa_diaria)
        VALUES(@descripcion, @tarifa_diaria);

        SET @id = SCOPE_IDENTITY();  -- Recupera ID generado

        -- Registra auditoría del INSERT
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
        (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_VehiculosTipos',  -- Tabla afectada
            'INSERT',            -- Tipo de operación
            @id,                 -- PK creada
            NULL,                -- No hay "antes" en un INSERT
            (
                SELECT @id AS id_tipo,
                       @descripcion AS descripcion,
                       @tarifa_diaria AS tarifa_diaria
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            )                   -- Estado después
        );

        SELECT @id AS id_tipo;  -- Devuelve ID generado
    END TRY
    BEGIN CATCH
        THROW;  -- Reenvía error
    END CATCH;
END
GO

-- SP para actualizar un tipo de vehículo y registrar cambios en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_VehiculoTipoUpdate
    @id_tipo       INT,
    @descripcion   VARCHAR(50),
    @tarifa_diaria DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;  -- Reduce mensajes innecesarios

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX);   -- JSON del estado previo
        DECLARE @despues NVARCHAR(MAX); -- JSON del estado posterior

        -- Obtiene valores actuales antes del UPDATE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_VehiculosTipos
             WHERE id_tipo = @id_tipo
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Aplica el UPDATE al registro
        UPDATE SC_AlquilerVehiculos.T_VehiculosTipos
        SET descripcion = @descripcion,
            tarifa_diaria = @tarifa_diaria
        WHERE id_tipo = @id_tipo;

        -- Obtiene valores después del UPDATE
        SELECT @despues =
            (SELECT * FROM SC_AlquilerVehiculos.T_VehiculosTipos
             WHERE id_tipo = @id_tipo
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Registra auditoría del UPDATE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
        (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_VehiculosTipos',  -- Tabla modificada
            'UPDATE',            -- Operación
            @id_tipo,            -- PK afectada
            @antes,              -- Estado antes
            @despues             -- Estado después
        );
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error
    END CATCH;
END
GO

-- SP para eliminar un tipo de vehículo y registrar el borrado en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_VehiculoTipoDelete
    @id_tipo INT
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes adicionales

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX); -- Estado previo al borrado

        -- Obtiene datos antes del DELETE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_VehiculosTipos
             WHERE id_tipo = @id_tipo
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Elimina el tipo de vehículo
        DELETE FROM SC_AlquilerVehiculos.T_VehiculosTipos
        WHERE id_tipo = @id_tipo;

        -- Registra auditoría del DELETE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
        (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_VehiculosTipos',  -- Tabla afectada
            'DELETE',            -- Operación
            @id_tipo,            -- PK eliminada
            @antes,              -- Estado antes
            NULL                 -- No hay estado después
        );
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error
    END CATCH;
END
GO


/*VEHICULOS***********************************************/
/* INSERT */

-- SP para insertar un vehículo y registrar la operación en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_VehiculoInsert
    @placa       VARCHAR(20),
    @marca       VARCHAR(50),
    @modelo      VARCHAR(50),
    @anio        INT,
    @estado      VARCHAR(20),
    @id_tipo     INT,
    @id_sucursal INT
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes innecesarios

    BEGIN TRY
        DECLARE @id INT; -- ID generado del vehículo

        -- Inserta el vehículo en la tabla
        INSERT INTO SC_AlquilerVehiculos.T_Vehiculos
            (placa, marca, modelo, anio, estado, id_tipo, id_sucursal)
        VALUES
            (@placa, @marca, @modelo, @anio, @estado, @id_tipo, @id_sucursal);

        SET @id = SCOPE_IDENTITY(); -- Obtiene ID generado

        -- Registra auditoría del INSERT
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Vehiculos', -- Tabla modificada
            'INSERT',      -- Operación realizada
            @id,           -- PK generada
            NULL,          -- No hay estado previo
            (
                SELECT @id AS id_vehiculo,
                       @placa AS placa,
                       @marca AS marca,
                       @modelo AS modelo,
                       @anio AS anio,
                       @estado AS estado,
                       @id_tipo AS id_tipo,
                       @id_sucursal AS id_sucursal
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            )
        );

        SELECT @id AS id_vehiculo; -- Retorna ID insertado
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error
    END CATCH;
END
GO

-- SP para actualizar un vehículo y registrar los cambios en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_VehiculoUpdate
    @id_vehiculo INT,
    @placa       VARCHAR(20),
    @marca       VARCHAR(50),
    @modelo      VARCHAR(50),
    @anio        INT,
    @estado      VARCHAR(20),
    @id_tipo     INT,
    @id_sucursal INT
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes innecesarios

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX);   -- Estado previo
        DECLARE @despues NVARCHAR(MAX); -- Estado posterior

        -- Obtiene estado antes del UPDATE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_Vehiculos
             WHERE id_vehiculo = @id_vehiculo
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Aplica el UPDATE
        UPDATE SC_AlquilerVehiculos.T_Vehiculos
        SET placa       = @placa,
            marca       = @marca,
            modelo      = @modelo,
            anio        = @anio,
            estado      = @estado,
            id_tipo     = @id_tipo,
            id_sucursal = @id_sucursal
        WHERE id_vehiculo = @id_vehiculo;

        -- Obtiene estado después del UPDATE
        SELECT @despues =
            (SELECT * FROM SC_AlquilerVehiculos.T_Vehiculos
             WHERE id_vehiculo = @id_vehiculo
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Registra auditoría
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Vehiculos', -- Tabla afectada
            'UPDATE',      -- Operación
            @id_vehiculo,  -- Llave primaria
            @antes,        -- Estado previo
            @despues       -- Estado después
        );
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía el error
    END CATCH;
END
GO

-- SP para eliminar un vehículo y registrar el borrado en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_VehiculoDelete
    @id_vehiculo INT
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes adicionales

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX); -- Estado previo

        -- Obtiene datos antes del DELETE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_Vehiculos
             WHERE id_vehiculo = @id_vehiculo
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Ejecuta el DELETE
        DELETE FROM SC_AlquilerVehiculos.T_Vehiculos
        WHERE id_vehiculo = @id_vehiculo;

        -- Registra auditoría del DELETE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Vehiculos', -- Tabla
            'DELETE',      -- Operación
            @id_vehiculo,  -- PK eliminada
            @antes,        -- Estado previo
            NULL           -- No existe estado después
        );
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error al cliente
    END CATCH;
END
GO


/*ALQUILERES***********************************************/

-- SP para insertar un alquiler y registrar la operación en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_AlquilerInsert
    @fecha_inicio DATE,
    @fecha_fin    DATE,
    @iva          DECIMAL(10,2),
    @id_cliente   INT,
    @id_empleado  INT,
    @id_sucursal  INT,
    @estado       VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes innecesarios

    BEGIN TRY
        DECLARE @id INT; -- ID generado del alquiler

        -- Inserta un nuevo alquiler
        INSERT INTO SC_AlquilerVehiculos.T_Alquileres
            (fecha_inicio, fecha_fin, iva, id_cliente, id_empleado, id_sucursal, estado)
        VALUES
            (@fecha_inicio, @fecha_fin, @iva, @id_cliente, @id_empleado, @id_sucursal, @estado);

        SET @id = SCOPE_IDENTITY();  -- Obtiene ID insertado

        -- Registra auditoría del INSERT
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Alquileres',  -- Tabla modificada
            'INSERT',        -- Operación realizada
            @id,             -- Llave primaria creada
            NULL,            -- No hay estado previo en un INSERT
            (
                SELECT @id AS id_alquiler,
                       @fecha_inicio AS fecha_inicio,
                       @fecha_fin AS fecha_fin,
                       @iva AS iva,
                       @id_cliente AS id_cliente,
                       @id_empleado AS id_empleado,
                       @id_sucursal AS id_sucursal,
                       @estado AS estado
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            )
        );

        SELECT @id AS id_alquiler;  -- Devuelve ID generado
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error al cliente
    END CATCH;
END
GO

-- SP para actualizar un alquiler y registrar los cambios en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_AlquilerUpdate
    @id_alquiler INT,
    @fecha_inicio DATE,
    @fecha_fin    DATE,
    @iva          DECIMAL(10,2),
    @id_cliente   INT,
    @id_empleado  INT,
    @id_sucursal  INT,
    @estado       VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes extra

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX);   -- Estado previo
        DECLARE @despues NVARCHAR(MAX); -- Estado posterior

        -- Obtiene datos antes del UPDATE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_Alquileres
             WHERE id_alquiler = @id_alquiler
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Actualiza el alquiler
        UPDATE SC_AlquilerVehiculos.T_Alquileres
        SET fecha_inicio = @fecha_inicio,
            fecha_fin    = @fecha_fin,
            iva          = @iva,
            id_cliente   = @id_cliente,
            id_empleado  = @id_empleado,
            id_sucursal  = @id_sucursal,
            estado       = @estado
        WHERE id_alquiler = @id_alquiler;

        -- Obtiene datos después del UPDATE
        SELECT @despues =
            (SELECT * FROM SC_AlquilerVehiculos.T_Alquileres
             WHERE id_alquiler = @id_alquiler
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Registra auditoría del UPDATE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Alquileres',  -- Tabla
            'UPDATE',        -- Tipo de operación
            @id_alquiler,    -- Llave primaria modificada
            @antes,          -- Estado previo
            @despues         -- Estado posterior
        );
    END TRY
    BEGIN CATCH
        THROW;  -- Reenvía error
    END CATCH;
END
GO

-- SP para eliminar un alquiler y registrar el borrado en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_AlquilerDelete
    @id_alquiler INT
AS
BEGIN
    SET NOCOUNT ON; -- Evita mensajes adicionales

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX); -- Estado previo

        -- Obtiene datos antes del DELETE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_Alquileres
             WHERE id_alquiler = @id_alquiler
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Elimina el registro
        DELETE FROM SC_AlquilerVehiculos.T_Alquileres
        WHERE id_alquiler = @id_alquiler;

        -- Registra auditoría del DELETE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Alquileres',   -- Tabla
            'DELETE',         -- Operación
            @id_alquiler,     -- Llave primaria eliminada
            @antes,           -- Estado previo
            NULL              -- No hay valores después
        );
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error
    END CATCH;
END
GO



/*ALQUILERES DETALLE***********************************************/
/* INSERT */

-- SP para insertar un detalle de alquiler y registrar la operación en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_AlquilerDetalleInsert
    @id_alquiler   INT,
    @id_vehiculo   INT,
    @tarifa_diaria DECIMAL(10,2),
    @fecha_inicio  DATE,
    @fecha_fin     DATE,
    @subtotal      DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes adicionales

    BEGIN TRY
        DECLARE @id INT;  -- ID del detalle insertado

        -- Inserta el detalle de alquiler
        INSERT INTO SC_AlquilerVehiculos.T_AlquileresDetalles
            (id_alquiler, id_vehiculo, tarifa_diaria, fecha_inicio, fecha_fin, subtotal)
        VALUES
            (@id_alquiler, @id_vehiculo, @tarifa_diaria, @fecha_inicio, @fecha_fin, @subtotal);

        SET @id = SCOPE_IDENTITY();  -- Obtiene el ID generado

        -- Registra auditoría del INSERT
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_AlquileresDetalles',  -- Tabla modificada
            'INSERT',                -- Operación
            @id,                     -- Llave primaria generada
            NULL,                    -- No hay valores antes
            (
                SELECT @id AS id_detalle,
                       @id_alquiler AS id_alquiler,
                       @id_vehiculo AS id_vehiculo,
                       @tarifa_diaria AS tarifa_diaria,
                       @fecha_inicio AS fecha_inicio,
                       @fecha_fin AS fecha_fin,
                       @subtotal AS subtotal
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            )                        -- Estado después
        );

        SELECT @id AS id_detalle;  -- Devuelve ID insertado
    END TRY
    BEGIN CATCH
        THROW;  -- Propaga el error
    END CATCH;
END
GO

-- SP para actualizar un detalle de alquiler y registrar los cambios en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_AlquilerDetalleUpdate
    @id_detalle   INT,
    @id_alquiler  INT,
    @id_vehiculo  INT,
    @tarifa_diaria DECIMAL(10,2),
    @fecha_inicio DATE,
    @fecha_fin    DATE,
    @subtotal     DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes adicionales

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX);    -- Estado previo
        DECLARE @despues NVARCHAR(MAX);  -- Estado posterior

        -- Obtiene datos antes del UPDATE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_AlquileresDetalles
             WHERE id_detalle = @id_detalle
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Ejecuta el UPDATE
        UPDATE SC_AlquilerVehiculos.T_AlquileresDetalles
        SET id_alquiler   = @id_alquiler,
            id_vehiculo   = @id_vehiculo,
            tarifa_diaria = @tarifa_diaria,
            fecha_inicio  = @fecha_inicio,
            fecha_fin     = @fecha_fin,
            subtotal      = @subtotal
        WHERE id_detalle = @id_detalle;

        -- Obtiene estado después del UPDATE
        SELECT @despues =
            (SELECT * FROM SC_AlquilerVehiculos.T_AlquileresDetalles
             WHERE id_detalle = @id_detalle
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Registra auditoría del UPDATE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_AlquileresDetalles',  -- Tabla
            'UPDATE',                -- Tipo de operación
            @id_detalle,             -- Llave primaria
            @antes,                  -- Estado previo
            @despues                 -- Estado posterior
        );
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error
    END CATCH;
END
GO


-- SP para eliminar un detalle de alquiler y registrar el borrado en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_AlquilerDetalleDelete
    @id_detalle INT
AS
BEGIN
    SET NOCOUNT ON; -- Evita mensajes innecesarios

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX); -- Datos previos al borrado

        -- Obtiene datos antes del DELETE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_AlquileresDetalles
             WHERE id_detalle = @id_detalle
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Ejecuta el DELETE
        DELETE FROM SC_AlquilerVehiculos.T_AlquileresDetalles
        WHERE id_detalle = @id_detalle;

        -- Registra auditoría del DELETE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_AlquileresDetalles',  -- Tabla
            'DELETE',                -- Operación
            @id_detalle,             -- Llave primaria borrada
            @antes,                  -- Estado previo
            NULL                     -- No hay estado posterior
        );
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error
    END CATCH;
END
GO





/*RECIBOS***********************************************/

-- SP para insertar un recibo y registrar la operación en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_ReciboInsert
    @fecha_pago DATE,
    @monto      DECIMAL(10,2),
    @metodo     VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes extra

    BEGIN TRY
        DECLARE @id INT;  -- ID generado del recibo

        -- Inserta el recibo en la tabla
        INSERT INTO SC_AlquilerVehiculos.T_Recibos(fecha_pago, monto, metodo)
        VALUES(@fecha_pago, @monto, @metodo);

        SET @id = SCOPE_IDENTITY();  -- Obtiene el ID generado

        -- Registra auditoría del INSERT
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Recibos',  -- Tabla modificada
            'INSERT',     -- Operación realizada
            @id,          -- PK creada
            NULL,         -- No hay valores antes
            (
                SELECT @id AS id_recibo,
                       @fecha_pago AS fecha_pago,
                       @monto AS monto,
                       @metodo AS metodo
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            )             -- Estado después
        );

        SELECT @id AS id_recibo; -- Devuelve ID insertado
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error
    END CATCH;
END
GO

-- SP para actualizar un recibo y registrar los cambios en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_ReciboUpdate
    @id_recibo  INT,
    @fecha_pago DATE,
    @monto      DECIMAL(10,2),
    @metodo     VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes adicionales

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX);    -- Estado previo
        DECLARE @despues NVARCHAR(MAX);  -- Estado posterior

        -- Obtiene datos antes del UPDATE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_Recibos
             WHERE id_recibo = @id_recibo
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Actualiza el registro
        UPDATE SC_AlquilerVehiculos.T_Recibos
        SET fecha_pago = @fecha_pago,
            monto      = @monto,
            metodo     = @metodo
        WHERE id_recibo = @id_recibo;

        -- Obtiene datos después del cambio
        SELECT @despues =
            (SELECT * FROM SC_AlquilerVehiculos.T_Recibos
             WHERE id_recibo = @id_recibo
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Inserta auditoría del UPDATE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Recibos',   -- Tabla
            'UPDATE',      -- Operación
            @id_recibo,    -- Llave primaria afectada
            @antes,        -- Estado previo
            @despues       -- Estado posterior
        );
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error
    END CATCH;
END
GO


-- SP para eliminar un recibo y registrar el borrado en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_ReciboDelete
    @id_recibo INT
AS
BEGIN
    SET NOCOUNT ON;  -- Evita mensajes extra

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX); -- Estado previo

        -- Obtiene valores antes del DELETE
        SELECT @antes =
            (SELECT * FROM SC_AlquilerVehiculos.T_Recibos
             WHERE id_recibo = @id_recibo
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Elimina el registro
        DELETE FROM SC_AlquilerVehiculos.T_Recibos
        WHERE id_recibo = @id_recibo;

        -- Registra auditoría del DELETE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_Recibos',  -- Tabla
            'DELETE',     -- Operación
            @id_recibo,   -- PK eliminada
            @antes,       -- Estado previo
            NULL          -- No hay estado posterior
        );
    END TRY
    BEGIN CATCH
        THROW; -- Propaga el error
    END CATCH;
END
GO




/*ALQUILERES RECIBOS***********************************************/


-- SP para vincular un recibo con un alquiler y registrar la operación en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_AlquilerReciboInsert
    @id_recibo   INT,
    @id_alquiler INT
AS
BEGIN
    SET NOCOUNT ON; -- Evita mensajes adicionales

    BEGIN TRY
        -- Inserta relación entre recibo y alquiler
        INSERT INTO SC_AlquilerVehiculos.T_AlquileresRecibos(id_recibo, id_alquiler)
        VALUES(@id_recibo, @id_alquiler);

        -- Registra auditoría del INSERT
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_AlquileresRecibos',  -- Tabla
            'INSERT',               -- Operación
            CONCAT(@id_recibo, '-', @id_alquiler), -- PK compuesta
            NULL,                   -- No existe estado antes
            (
                SELECT @id_recibo AS id_recibo,
                       @id_alquiler AS id_alquiler
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            )                       -- Estado después
        );
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía el error
    END CATCH;
END
GO

-- SP para eliminar la relación entre un recibo y un alquiler, registrando el borrado en la bitácora
CREATE PROCEDURE SC_AlquilerVehiculos.SP_AlquilerReciboDelete
    @id_recibo   INT,
    @id_alquiler INT
AS
BEGIN
    SET NOCOUNT ON; -- Evita mensajes extra

    BEGIN TRY
        DECLARE @antes NVARCHAR(MAX);  -- Estado previo al borrado

        -- Obtiene valores antes del DELETE
        SELECT @antes =
            (SELECT * 
             FROM SC_AlquilerVehiculos.T_AlquileresRecibos
             WHERE id_recibo = @id_recibo
               AND id_alquiler = @id_alquiler
             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        -- Elimina la relación
        DELETE FROM SC_AlquilerVehiculos.T_AlquileresRecibos
        WHERE id_recibo = @id_recibo
          AND id_alquiler = @id_alquiler;

        -- Registra auditoría del DELETE
        INSERT INTO SC_AlquilerVehiculos.T_Bitacora
            (tabla, operacion, clave_primaria, valores_antes, valores_despues)
        VALUES(
            'T_AlquileresRecibos', -- Tabla
            'DELETE',              -- Operación
            CONCAT(@id_recibo, '-', @id_alquiler), -- PK compuesta
            @antes,                -- Estado previo
            NULL                   -- No existe estado después
        );
    END TRY
    BEGIN CATCH
        THROW; -- Reenvía error
    END CATCH;
END
GO

-- -----------------------------------------------------------------------------------

/* ============================================================
   PERMISOS PARA CADA ROL
   ============================================================ */

------------------------------
-- ROL EMPLEADO
------------------------------


-- Inserciones permitidas
GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_Clientes TO rol_empleado;
GRANT SELECT, INSERT, UPDATE, DELETE ON  SC_AlquilerVehiculos.T_Empleados TO rol_empleado;
GRANT SELECT, INSERT, UPDATE, DELETE ON  SC_AlquilerVehiculos.T_Vehiculos TO rol_empleado;
GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_Sucursales TO rol_empleado;
GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_VehiculosTipos TO rol_empleado;

GRANT SELECT, INSERT ON SC_AlquilerVehiculos.T_Alquileres          TO rol_empleado;
GRANT SELECT, INSERT ON SC_AlquilerVehiculos.T_AlquileresDetalles  TO rol_empleado;
GRANT SELECT, INSERT ON SC_AlquilerVehiculos.T_Recibos             TO rol_empleado;
GRANT SELECT, INSERT ON SC_AlquilerVehiculos.T_AlquileresRecibos   TO rol_empleado;

------------------------------
-- ROL JEFE (ACCEDER A TODO)
------------------------------

GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_Sucursales          TO rol_jefe;
GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_Clientes            TO rol_jefe;
GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_Empleados           TO rol_jefe;
GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_VehiculosTipos      TO rol_jefe;
GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_Vehiculos           TO rol_jefe;
GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_Alquileres          TO rol_jefe;
GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_AlquileresDetalles  TO rol_jefe;
GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_Recibos             TO rol_jefe;
GRANT SELECT, INSERT, UPDATE, DELETE ON SC_AlquilerVehiculos.T_AlquileresRecibos   TO rol_jefe;
GRANT SELECT ON SC_AlquilerVehiculos.T_Bitacora TO rol_jefe;


------------------------------
-- 4) ROL ADMINISTRADOR APP (super usuario)
------------------------------

GRANT CONTROL ON DATABASE::DB_AlquilerVehiculos TO rol_adminApp;
GRANT EXECUTE TO rol_adminApp;  -- ejecutar SP
GRANT ALTER ANY SCHEMA TO rol_adminApp;
GRANT VIEW DEFINITION TO rol_adminApp;


/* ============================================================
   ASIGNAR ROLES A LOS USUARIOS
   ============================================================ */

EXEC sp_addrolemember 'rol_empleado', 'User_Empleado';
EXEC sp_addrolemember 'rol_jefe', 'User_Jefe';
EXEC sp_addrolemember 'rol_adminApp', 'User_AdminApp';
GO

GRANT EXECUTE TO rol_empleado;
GRANT EXECUTE TO rol_jefe;

