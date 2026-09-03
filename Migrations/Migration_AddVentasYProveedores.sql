-- Migration: Añadir tablas Proveedores y Ventas
BEGIN TRANSACTION;
USE RegistroProductos;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'app.Proveedores') AND type = N'U')
BEGIN
	CREATE TABLE app.Proveedores (
		idProveedor INT IDENTITY(1,1) PRIMARY KEY,
		nombre NVARCHAR(250) NOT NULL,
		telefono NVARCHAR(50) NULL,
		email NVARCHAR(200) NULL,
		direccion NVARCHAR(500) NULL,
		FechaCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
	);
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'app.Ventas') AND type = N'U')
BEGIN
	CREATE TABLE app.Ventas (
		idVenta INT IDENTITY(1,1) PRIMARY KEY,
		fecha DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
		productoId INT NOT NULL,
		cantidad INT NOT NULL,
		total DECIMAL(18,2) NOT NULL,
		CONSTRAINT FK_Ventas_Productos FOREIGN KEY (productoId) REFERENCES app.Productos(Id)
	);
END

COMMIT;
