-- Script: CreateRegistroProductos.sql
-- Uso: Abrir en SQL Server Management Studio y ejecutar por partes.

-- 1) Crear base de datos
IF DB_ID('RegistroProductos') IS NULL
BEGIN
	CREATE DATABASE RegistroProductos;
	PRINT 'Base de datos RegistroProductos creada.';
END
ELSE
BEGIN
	PRINT 'Base de datos RegistroProductos ya existe.';
END
GO

-- 2) Seleccionar la base de datos
USE RegistroProductos;
GO

-- 3) Crear esquema (opcional)
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'app')
	EXEC('CREATE SCHEMA app');
GO

-- 4) Crear tabla Productos
IF OBJECT_ID('app.Productos', 'U') IS NOT NULL
	DROP TABLE app.Productos;
GO

CREATE TABLE app.Productos (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Nombre NVARCHAR(200) NOT NULL,
	Precio DECIMAL(18,2) NOT NULL CONSTRAINT CK_Precio_NonNegative CHECK (Precio >= 0),
	Cantidad INT NOT NULL CONSTRAINT CK_Cantidad_NonNegative CHECK (Cantidad >= 0),
	Disponible BIT NOT NULL DEFAULT 1,
	FechaCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 5) Índices (si se desea buscar por nombre)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Productos_Nombre' AND object_id = OBJECT_ID('app.Productos'))
	CREATE INDEX IX_Productos_Nombre ON app.Productos(Nombre);
GO

-- 6) Datos de ejemplo
INSERT INTO app.Productos (Nombre, Precio, Cantidad, Disponible)
VALUES
('Camiseta deportiva', 19.99, 50, 1),
('Zapatos running', 79.50, 20, 1),
('Mochila urbana', 45.00, 15, 1);
GO

-- 7) Procedimientos almacenados básicos
-- Insertar producto
IF OBJECT_ID('app.sp_InsertProducto', 'P') IS NOT NULL
	DROP PROCEDURE app.sp_InsertProducto;
GO
CREATE PROCEDURE app.sp_InsertProducto
	@Nombre NVARCHAR(200),
	@Precio DECIMAL(18,2),
	@Cantidad INT,
	@Disponible BIT = 1
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO app.Productos (Nombre, Precio, Cantidad, Disponible)
	VALUES (@Nombre, @Precio, @Cantidad, @Disponible);
	SELECT SCOPE_IDENTITY() AS NewId;
END;
GO

-- Obtener todos los productos
IF OBJECT_ID('app.sp_GetProductos', 'P') IS NOT NULL
	DROP PROCEDURE app.sp_GetProductos;
GO
CREATE PROCEDURE app.sp_GetProductos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Id, Nombre, Precio, Cantidad, Disponible, FechaCreacion
	FROM app.Productos
	ORDER BY Nombre;
END;
GO

-- Actualizar producto
IF OBJECT_ID('app.sp_UpdateProducto', 'P') IS NOT NULL
	DROP PROCEDURE app.sp_UpdateProducto;
GO
CREATE PROCEDURE app.sp_UpdateProducto
	@Id INT,
	@Nombre NVARCHAR(200),
	@Precio DECIMAL(18,2),
	@Cantidad INT,
	@Disponible BIT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE app.Productos
	SET Nombre = @Nombre,
		Precio = @Precio,
		Cantidad = @Cantidad,
		Disponible = @Disponible
	WHERE Id = @Id;
	SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

-- Eliminar producto
IF OBJECT_ID('app.sp_DeleteProducto', 'P') IS NOT NULL
	DROP PROCEDURE app.sp_DeleteProducto;
GO
CREATE PROCEDURE app.sp_DeleteProducto
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM app.Productos WHERE Id = @Id;
	SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

-- 8) Permisos recomendados (ejecutar como administrador si se desea crear usuario)
-- Ejemplo de creación de usuario SQL (opcional, ejecutar sólo si se necesita)
-- CREATE LOGIN registro_user WITH PASSWORD = 'ContraseñaSegura123!';
-- CREATE USER registro_user FOR LOGIN registro_user;
-- ALTER ROLE db_datareader ADD MEMBER registro_user;
-- ALTER ROLE db_datawriter ADD MEMBER registro_user;

-- Fin del script
