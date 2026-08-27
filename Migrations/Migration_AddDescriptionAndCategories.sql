-- Migration: Añadir columna descripcion y categoriaId a Productos; crear tabla Categorias
BEGIN TRANSACTION;
USE RegistroProductos;

-- Crear tabla Categorias en el esquema app si no existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'app.Categorias') AND type = N'U')
BEGIN
	CREATE TABLE app.Categorias (
		idCategoria INT IDENTITY(1,1) PRIMARY KEY,
		nombre NVARCHAR(200) NOT NULL
	);
END

-- Añadir columna descripcion si no existe
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = 'descripcion' AND Object_ID = Object_ID(N'app.Productos'))
BEGIN
	ALTER TABLE app.Productos ADD descripcion NVARCHAR(MAX) NULL;
END

-- Añadir columna categoriaId si no existe
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = 'categoriaId' AND Object_ID = Object_ID(N'app.Productos'))
BEGIN
	ALTER TABLE app.Productos ADD categoriaId INT NULL;
END

-- Añadir FK si no existe y ambos existen
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'app.Categorias') AND type = N'U')
AND EXISTS(SELECT * FROM sys.columns WHERE Name = 'categoriaId' AND Object_ID = Object_ID(N'app.Productos'))
AND NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE name = 'FK_Productos_Categorias' AND parent_object_id = OBJECT_ID(N'app.Productos'))
BEGIN
	ALTER TABLE app.Productos ADD CONSTRAINT FK_Productos_Categorias FOREIGN KEY (categoriaId) REFERENCES app.Categorias(idCategoria);
END

COMMIT;
