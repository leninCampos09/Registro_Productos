-- Migration: Crear tablas Compras y CompraDetalles
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'app')
BEGIN
	EXEC('CREATE SCHEMA app');
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'app' AND TABLE_NAME = 'Compras')
BEGIN
	CREATE TABLE app.Compras (
		IdCompra INT IDENTITY(1,1) PRIMARY KEY,
		proveedorId INT NULL,
		fecha DATETIMEOFFSET NOT NULL,
		tipo NVARCHAR(50) NULL,
		factura NVARCHAR(100) NULL,
		total DECIMAL(18,2) NOT NULL,
		FechaCreacion DATETIMEOFFSET NOT NULL DEFAULT SYSUTCDATETIME()
	);
	ALTER TABLE app.Compras ADD CONSTRAINT FK_Compras_Proveedores FOREIGN KEY (proveedorId) REFERENCES app.Proveedores(idProveedor);
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'app' AND TABLE_NAME = 'CompraDetalles')
BEGIN
	CREATE TABLE app.CompraDetalles (
		IdDetalle INT IDENTITY(1,1) PRIMARY KEY,
		compraId INT NOT NULL,
		productoId INT NULL,
		descripcion NVARCHAR(250) NULL,
		cantidad INT NOT NULL,
		precio DECIMAL(18,2) NOT NULL,
		subtotal DECIMAL(18,2) NOT NULL
	);
	ALTER TABLE app.CompraDetalles ADD CONSTRAINT FK_CompraDetalles_Compras FOREIGN KEY (compraId) REFERENCES app.Compras(IdCompra);
	ALTER TABLE app.CompraDetalles ADD CONSTRAINT FK_CompraDetalles_Productos FOREIGN KEY (productoId) REFERENCES app.Productos(Id);
END
