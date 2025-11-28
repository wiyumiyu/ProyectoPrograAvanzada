USE master;
GO

/* ============================================================
   1. Cerrar conexiones activas a la BD
   ============================================================ */
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'DB_AlquilerVehiculos')
BEGIN
    ALTER DATABASE DB_AlquilerVehiculos SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
END
GO

/* ============================================================
   2. Eliminar la base de datos
   ============================================================ */
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'DB_AlquilerVehiculos')
BEGIN
    DROP DATABASE DB_AlquilerVehiculos;
END
GO


/* ============================================================
   3. Eliminar USUARIOS del servidor (logins deben ser dropeados DESPUÉS)
   ============================================================ */

-- Eliminar usuario del login Login_Empleado
IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'Login_Empleado')
BEGIN
    DROP LOGIN Login_Empleado;
END
GO

-- Eliminar usuario del login Login_Jefe
IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'Login_Jefe')
BEGIN
    DROP LOGIN Login_Jefe;
END
GO

-- Eliminar usuario del login Login_AdminApp
IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'Login_AdminApp')
BEGIN
    DROP LOGIN Login_AdminApp;
END
GO


/* ============================================================
   4. Eliminar roles de nivel servidor (si se llegaron a crear)
   ============================================================ */

-- Nota: los roles que creaste son ROLES DE BD, no de servidor,
-- y se eliminan automáticamente cuando borrás la base de datos.
-- Por eso NO se requieren DROP ROLE independientes acá.


/* ============================================================
   5. Limpieza opcional (solo si quieres eliminar restos)
   ============================================================ */

-- Eliminación manual de usuarios que hayan quedado huérfanos
-- (normalmente no hace falta si se eliminó la BD)
DECLARE @usuario NVARCHAR(100);

DECLARE orphaned_users CURSOR FOR
SELECT dp.name
FROM sys.database_principals dp
LEFT JOIN sys.server_principals sp ON dp.sid = sp.sid
WHERE sp.sid IS NULL
  AND dp.type_desc = 'SQL_USER';

OPEN orphaned_users;
FETCH NEXT FROM orphaned_users INTO @usuario;

WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC ('DROP USER [' + @usuario + ']');
    FETCH NEXT FROM orphaned_users INTO @usuario;
END

CLOSE orphaned_users;
DEALLOCATE orphaned_users;
GO
