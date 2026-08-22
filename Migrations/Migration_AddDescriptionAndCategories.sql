-- Migration: Añadir columna descripcion y categoriaId a Productos; crear tabla Categorias
BEGIN TRANSACTION;

-- Crear tabla Categorias si no existe
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categorias' AND type = 'U')
BEGIN
	CREATE TABLE Categorias (
		idCategoria INT IDENTITY(1,1) PRIMARY KEY,
		nombre NVARCHAR(200) NOT NULL
	);
END

-- Añadir columna descripcion si no existe
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = 'descripcion' AND Object_ID = Object_ID('Productos'))
BEGIN
	ALTER TABLE Productos ADD descripcion NVARCHAR(MAX) NULL;
END

-- Añadir columna categoriaId si no existe
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = 'categoriaId' AND Object_ID = Object_ID('Productos'))
BEGIN
	ALTER TABLE Productos ADD categoriaId INT NULL;
END

-- Añadir FK si no existe y ambos existen
IF EXISTS(SELECT * FROM sys.tables WHERE name = 'Categorias' AND type = 'U')
AND EXISTS(SELECT * FROM sys.columns WHERE Name = 'categoriaId' AND Object_ID = Object_ID('Productos'))
AND NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE name = 'FK_Productos_Categorias')
BEGIN
	ALTER TABLE Productos ADD CONSTRAINT FK_Productos_Categorias FOREIGN KEY (categoriaId) REFERENCES Categorias(idCategoria);
END

COMMIT;
