IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'app')
BEGIN
	EXEC('CREATE SCHEMA app');
END

IF NOT EXISTS (SELECT 1 FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'app' AND t.name = 'VentasMaster')
BEGIN
	CREATE TABLE app.VentasMaster (
		idVenta INT IDENTITY(1,1) PRIMARY KEY,
		fecha DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
		total DECIMAL(18,2) NOT NULL,
		clienteNombre NVARCHAR(200) NULL,
		clienteTelefono NVARCHAR(50) NULL,
		clienteEmail NVARCHAR(200) NULL
	);
END

IF NOT EXISTS (SELECT 1 FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'app' AND t.name = 'VentasDetalles')
BEGIN
	CREATE TABLE app.VentasDetalles (
		id INT IDENTITY(1,1) PRIMARY KEY,
		ventaId INT NOT NULL,
		productoId INT NULL,
		cantidad INT NOT NULL,
		precioUnit DECIMAL(18,2) NOT NULL,
		total DECIMAL(18,2) NOT NULL,
		CONSTRAINT FK_VentasDetalles_VentasMaster FOREIGN KEY(ventaId) REFERENCES app.VentasMaster(idVenta) ON DELETE CASCADE
	);
END
