using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Registro_Productos
{
    public static class Database
    {
        // Cadena de conexión proporcionada por el usuario
        private static readonly string ConnectionString = @"Data Source=LENIN-CAMPOS09;Initial Catalog=RegistroProductos;Integrated Security=True;Trust Server Certificate=True";

        public static int InsertProduct(string nombre, decimal precio, int cantidad, bool disponible, string imgPath, string descripcion = null, int? categoriaId = null)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(@"INSERT INTO app.Productos (Nombre, Precio, Cantidad, Disponible, img, descripcion, categoriaId)
                                            VALUES (@producto, @precio, @cantidad, @disponible, @img, @descripcion, @categoriaId);
                                            SELECT SCOPE_IDENTITY();", conn);
            cmd.Parameters.AddWithValue("@producto", nombre);
            cmd.Parameters.AddWithValue("@precio", precio);
            cmd.Parameters.AddWithValue("@cantidad", cantidad);
            cmd.Parameters.AddWithValue("@disponible", disponible);
            cmd.Parameters.AddWithValue("@img", (object)imgPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@descripcion", (object)descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@categoriaId", (object)categoriaId ?? DBNull.Value);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static int DeleteCompra(int id)
        {
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();
            try
            {
                using var tran = conn.BeginTransaction();
                try
                {
                    using (var delDet = new SqlCommand("DELETE FROM app.CompraDetalles WHERE compraId = @id;", conn, tran)) { delDet.Parameters.AddWithValue("@id", id); delDet.ExecuteNonQuery(); }
                    using (var delMaster = new SqlCommand("DELETE FROM app.Compras WHERE IdCompra = @id; SELECT @@ROWCOUNT;", conn, tran)) { delMaster.Parameters.AddWithValue("@id", id); var res = delMaster.ExecuteScalar(); tran.Commit(); return Convert.ToInt32(res); }
                }
                catch
                {
                    try { tran.Rollback(); } catch { }
                }
            }
            catch { }

            using var cmd = new SqlCommand("DELETE FROM app.Compras WHERE IdCompra = @id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static DataTable GetAllCompras()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            // Incluir el nombre del proveedor para mostrar en la UI
            using var cmd = new SqlCommand(@"SELECT c.IdCompra, c.proveedorId, ISNULL(p.nombre, '') AS proveedorNombre, c.fecha, c.tipo, c.factura, c.total, c.FechaCreacion FROM app.Compras c LEFT JOIN app.Proveedores p ON c.proveedorId = p.idProveedor ORDER BY c.IdCompra DESC", conn);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        // Informes CRUD (InformesVentas)
        public static DataTable GetAllReportVentas()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT Id, FechaGeneracion, PeriodoInicio, PeriodoFin, TotalVentas, TotalItems, ReportData FROM app.InformesVentas ORDER BY FechaGeneracion DESC", conn);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public static DataRow GetReportVentaById(int id)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT Id, FechaGeneracion, PeriodoInicio, PeriodoFin, TotalVentas, TotalItems, ReportData FROM app.InformesVentas WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public static int InsertReportVenta(DateTime fechaGeneracion, DateTime? periodoInicio, DateTime? periodoFin, decimal totalVentas, int totalItems, string reportData)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("INSERT INTO app.InformesVentas (FechaGeneracion, PeriodoInicio, PeriodoFin, TotalVentas, TotalItems, ReportData) VALUES (@fg, @pi, @pf, @tv, @ti, @rd); SELECT SCOPE_IDENTITY();", conn);
            cmd.Parameters.AddWithValue("@fg", fechaGeneracion);
            cmd.Parameters.AddWithValue("@pi", (object)periodoInicio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@pf", (object)periodoFin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tv", totalVentas);
            cmd.Parameters.AddWithValue("@ti", totalItems);
            cmd.Parameters.AddWithValue("@rd", (object)reportData ?? DBNull.Value);
            conn.Open();
            var res = cmd.ExecuteScalar();
            return Convert.ToInt32(res);
        }

        public static int UpdateReportVenta(int id, DateTime fechaGeneracion, DateTime? periodoInicio, DateTime? periodoFin, decimal totalVentas, int totalItems, string reportData)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("UPDATE app.InformesVentas SET FechaGeneracion=@fg, PeriodoInicio=@pi, PeriodoFin=@pf, TotalVentas=@tv, TotalItems=@ti, ReportData=@rd WHERE Id=@id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fg", fechaGeneracion);
            cmd.Parameters.AddWithValue("@pi", (object)periodoInicio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@pf", (object)periodoFin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tv", totalVentas);
            cmd.Parameters.AddWithValue("@ti", totalItems);
            cmd.Parameters.AddWithValue("@rd", (object)reportData ?? DBNull.Value);
            conn.Open();
            var res = cmd.ExecuteScalar();
            return Convert.ToInt32(res);
        }

        public static int DeleteReportVenta(int id)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("DELETE FROM app.InformesVentas WHERE Id=@id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var res = cmd.ExecuteScalar();
            return Convert.ToInt32(res);
        }

        // Informes CRUD (InformesCompras)
        public static DataTable GetAllReportCompras()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT Id, FechaGeneracion, PeriodoInicio, PeriodoFin, TotalCompras, TotalItems, ReportData FROM app.InformesCompras ORDER BY FechaGeneracion DESC", conn);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public static DataRow GetReportCompraById(int id)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT Id, FechaGeneracion, PeriodoInicio, PeriodoFin, TotalCompras, TotalItems, ReportData FROM app.InformesCompras WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public static int InsertReportCompra(DateTime fechaGeneracion, DateTime? periodoInicio, DateTime? periodoFin, decimal totalCompras, int totalItems, string reportData)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("INSERT INTO app.InformesCompras (FechaGeneracion, PeriodoInicio, PeriodoFin, TotalCompras, TotalItems, ReportData) VALUES (@fg, @pi, @pf, @tc, @ti, @rd); SELECT SCOPE_IDENTITY();", conn);
            cmd.Parameters.AddWithValue("@fg", fechaGeneracion);
            cmd.Parameters.AddWithValue("@pi", (object)periodoInicio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@pf", (object)periodoFin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tc", totalCompras);
            cmd.Parameters.AddWithValue("@ti", totalItems);
            cmd.Parameters.AddWithValue("@rd", (object)reportData ?? DBNull.Value);
            conn.Open();
            var res = cmd.ExecuteScalar();
            return Convert.ToInt32(res);
        }

        public static int UpdateReportCompra(int id, DateTime fechaGeneracion, DateTime? periodoInicio, DateTime? periodoFin, decimal totalCompras, int totalItems, string reportData)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("UPDATE app.InformesCompras SET FechaGeneracion=@fg, PeriodoInicio=@pi, PeriodoFin=@pf, TotalCompras=@tc, TotalItems=@ti, ReportData=@rd WHERE Id=@id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fg", fechaGeneracion);
            cmd.Parameters.AddWithValue("@pi", (object)periodoInicio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@pf", (object)periodoFin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tc", totalCompras);
            cmd.Parameters.AddWithValue("@ti", totalItems);
            cmd.Parameters.AddWithValue("@rd", (object)reportData ?? DBNull.Value);
            conn.Open();
            var res = cmd.ExecuteScalar();
            return Convert.ToInt32(res);
        }

        public static int DeleteReportCompra(int id)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("DELETE FROM app.InformesCompras WHERE Id=@id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var res = cmd.ExecuteScalar();
            return Convert.ToInt32(res);
        }

        // Agregados simples para generar informes
        public static (decimal total, int items) GetSalesAggregate(DateTime? inicio, DateTime? fin)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT ISNULL(SUM(total),0) AS total, COUNT(*) AS items FROM app.VentasMaster WHERE (@inicio IS NULL OR fecha >= @inicio) AND (@fin IS NULL OR fecha <= @fin)", conn);
            cmd.Parameters.AddWithValue("@inicio", (object)inicio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fin", (object)fin ?? DBNull.Value);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return (reader.GetDecimal(0), reader.GetInt32(1));
            return (0m, 0);
        }

        public static (decimal total, int items) GetPurchasesAggregate(DateTime? inicio, DateTime? fin)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT ISNULL(SUM(total),0) AS total, COUNT(*) AS items FROM app.Compras WHERE (@inicio IS NULL OR fecha >= @inicio) AND (@fin IS NULL OR fecha <= @fin)", conn);
            cmd.Parameters.AddWithValue("@inicio", (object)inicio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fin", (object)fin ?? DBNull.Value);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return (reader.GetDecimal(0), reader.GetInt32(1));
            return (0m, 0);
        }

        // Report data tables (detallados) - Ventas
        public static DataTable GetVentasReport(DateTime? inicio, DateTime? fin)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            var sql = @"SELECT vm.idVenta AS VentaId, vm.fecha AS FechaVenta, vm.total AS TotalVenta, vm.clienteNombre, vm.clienteTelefono, vm.clienteEmail,
                                 d.id AS DetalleId, d.productoId, ISNULL(p.Nombre,'') AS Producto, d.cantidad, d.precioUnit, d.total AS Subtotal
                          FROM app.VentasMaster vm
                          LEFT JOIN app.VentasDetalles d ON d.ventaId = vm.idVenta
                          LEFT JOIN app.Productos p ON p.Id = d.productoId
                          WHERE (@inicio IS NULL OR vm.fecha >= @inicio) AND (@fin IS NULL OR vm.fecha <= @fin)
                          ORDER BY vm.fecha, vm.idVenta";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inicio", (object)inicio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fin", (object)fin ?? DBNull.Value);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        // Report data tables (detallados) - Compras
        public static DataTable GetComprasReport(DateTime? inicio, DateTime? fin)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            var sql = @"SELECT c.IdCompra AS CompraId, c.fecha AS FechaCompra, c.factura, c.tipo, c.total AS TotalCompra, ISNULL(pr.nombre,'') AS Proveedor,
                                 d.IdDetalle AS DetalleId, d.productoId, ISNULL(p.Nombre,'') AS Producto, d.descripcion, d.cantidad, d.precio, d.subtotal
                          FROM app.Compras c
                          LEFT JOIN app.CompraDetalles d ON d.compraId = c.IdCompra
                          LEFT JOIN app.Proveedores pr ON pr.idProveedor = c.proveedorId
                          LEFT JOIN app.Productos p ON p.Id = d.productoId
                          WHERE (@inicio IS NULL OR c.fecha >= @inicio) AND (@fin IS NULL OR c.fecha <= @fin)
                          ORDER BY c.fecha, c.IdCompra";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inicio", (object)inicio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fin", (object)fin ?? DBNull.Value);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public static int? GetProductoIdByName(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            try
            {
                using var conn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("SELECT TOP 1 Id FROM app.Productos WHERE LOWER(LTRIM(RTRIM(Nombre))) = LOWER(LTRIM(RTRIM(@n)))", conn);
                cmd.Parameters.AddWithValue("@n", nombre);
                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (obj == null || obj == DBNull.Value) return null;
                return Convert.ToInt32(obj);
            }
            catch
            {
                return null;
            }
        }

        public static DataTable GetCompraDetalles(int compraId)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            // Incluir el nombre del producto si existe; si no existe, usar la descripción como 'producto' para mostrar en la UI
            using var cmd = new SqlCommand("SELECT d.IdDetalle, d.compraId, d.productoId, COALESCE(NULLIF(p.Nombre, ''), d.descripcion) AS producto, d.descripcion, d.cantidad, d.precio, d.subtotal FROM app.CompraDetalles d LEFT JOIN app.Productos p ON d.productoId = p.Id WHERE d.compraId=@id ORDER BY d.IdDetalle", conn);
            cmd.Parameters.AddWithValue("@id", compraId);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        // Compra y detalle
        public class CompraLinea
        {
            public int? productoId { get; set; }
            public string descripcion { get; set; }
            public int cantidad { get; set; }
            public decimal precio { get; set; }
            public decimal subtotal { get; set; }
        }

        public static int InsertCompraWithDetails(int? proveedorId, DateTime fecha, string tipo, string factura, decimal total, List<CompraLinea> lineas)
        {
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                using var cmd = new SqlCommand(@"INSERT INTO app.Compras (proveedorId, fecha, tipo, factura, total) VALUES (@prov, @fecha, @tipo, @factura, @total); SELECT SCOPE_IDENTITY();", conn, tran);
                cmd.Parameters.AddWithValue("@prov", (object)proveedorId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@fecha", fecha);
                cmd.Parameters.AddWithValue("@tipo", (object)tipo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@factura", (object)factura ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@total", total);
                var idObj = cmd.ExecuteScalar();
                var compraId = Convert.ToInt32(idObj);

                foreach (var l in lineas)
                {
                    using var cmd2 = new SqlCommand(@"INSERT INTO app.CompraDetalles (compraId, productoId, descripcion, cantidad, precio, subtotal) VALUES (@compraId, @productoId, @descripcion, @cantidad, @precio, @subtotal);", conn, tran);
                    cmd2.Parameters.AddWithValue("@compraId", compraId);
                    cmd2.Parameters.AddWithValue("@productoId", (object)l.productoId ?? DBNull.Value);
                    cmd2.Parameters.AddWithValue("@descripcion", (object)l.descripcion ?? DBNull.Value);
                    cmd2.Parameters.AddWithValue("@cantidad", l.cantidad);
                    cmd2.Parameters.AddWithValue("@precio", l.precio);
                    cmd2.Parameters.AddWithValue("@subtotal", l.subtotal);
                    cmd2.ExecuteNonQuery();
                }

                tran.Commit();
                return compraId;
            }
            catch
            {
                try { tran.Rollback(); } catch { }
                throw;
            }
        }

        // Convertir DateTime que viene de la base de datos (almacenado en UTC) a la zona de El Salvador
        public static DateTime ConvertUtcToElSalvador(DateTime dbDateTime)
        {
            try
            {
                // Si el valor ya es Local, devolver tal cual
                if (dbDateTime.Kind == DateTimeKind.Local) return dbDateTime;
                // Asegurar que tratamos el valor como UTC cuando es Unspecified
                var utc = dbDateTime.Kind == DateTimeKind.Utc ? dbDateTime : DateTime.SpecifyKind(dbDateTime, DateTimeKind.Utc);
                // Windows timezone id para El Salvador
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
            }
            catch
            {
                // Fallback: convertir a la hora local del sistema
                try { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dbDateTime, DateTimeKind.Utc), TimeZoneInfo.Local); } catch { return dbDateTime; }
            }
        }

        // Ensure migrations SQL scripts under output\Migrations are applied once
        public static void EnsureMigrationsApplied()
        {
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var migrationsDir = Path.Combine(baseDir, "Migrations");
                if (!Directory.Exists(migrationsDir)) return;

                var files = Directory.GetFiles(migrationsDir, "*.sql").OrderBy(n => n).ToList();
                if (files.Count == 0) return;

                using var conn = new SqlConnection(ConnectionString);
                conn.Open();
                foreach (var file in files)
                {
                    var sql = File.ReadAllText(file);
                    // Split by GO statements (on a line by itself)
                    var batches = Regex.Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    using var tran = conn.BeginTransaction();
                    try
                    {
                        foreach (var batch in batches)
                        {
                            var text = batch.Trim();
                            if (string.IsNullOrEmpty(text)) continue;
                            using var cmd = new SqlCommand(text, conn, tran) { CommandTimeout = 60 };
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        try { tran.Rollback(); } catch { }
                        throw;
                    }
                }
            }
            catch
            {
                // rethrow to let caller handle error and show message
                throw;
            }
        }

        public static DataTable GetAllProducts()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT p.Id AS idProducto, p.Nombre AS producto, p.Precio AS precio, p.Cantidad AS cantidad, p.Disponible AS disponible, p.img AS img, p.descripcion AS descripcion, p.categoriaId AS categoriaId, c.nombre AS categoria, p.FechaCreacion AS fechaRegistro FROM app.Productos p LEFT JOIN app.Categorias c ON p.categoriaId = c.idCategoria ORDER BY p.Nombre", conn);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public static DataRow GetProductById(int id)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT p.Id AS idProducto, p.Nombre AS producto, p.Precio AS precio, p.Cantidad AS cantidad, p.Disponible AS disponible, p.img AS img, p.descripcion AS descripcion, p.categoriaId AS categoriaId, c.nombre AS categoria, p.FechaCreacion AS fechaRegistro FROM app.Productos p LEFT JOIN app.Categorias c ON p.categoriaId = c.idCategoria WHERE p.Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public static int UpdateProduct(int id, string nombre, decimal precio, int cantidad, bool disponible, string imgPath, string descripcion = null, int? categoriaId = null)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(@"UPDATE app.Productos SET Nombre=@producto, Precio=@precio, Cantidad=@cantidad, Disponible=@disponible, img=@img, descripcion=@descripcion, categoriaId=@categoriaId WHERE Id=@id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@producto", nombre);
            cmd.Parameters.AddWithValue("@precio", precio);
            cmd.Parameters.AddWithValue("@cantidad", cantidad);
            cmd.Parameters.AddWithValue("@disponible", disponible);
            cmd.Parameters.AddWithValue("@img", (object)imgPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@descripcion", (object)descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@categoriaId", (object)categoriaId ?? DBNull.Value);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        // Categorias CRUD
        public static DataTable GetAllCategories()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT idCategoria, nombre FROM app.Categorias ORDER BY nombre", conn);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public static int InsertCategory(string nombre)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("INSERT INTO app.Categorias (nombre) VALUES (@nombre); SELECT SCOPE_IDENTITY();", conn);
            cmd.Parameters.AddWithValue("@nombre", nombre ?? string.Empty);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static int UpdateCategory(int id, string nombre)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("UPDATE app.Categorias SET nombre=@nombre WHERE idCategoria=@id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nombre", nombre ?? string.Empty);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static int DeleteCategory(int id)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("DELETE FROM app.Categorias WHERE idCategoria=@id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static int DeleteProduct(int id)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("DELETE FROM app.Productos WHERE Id = @id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        // Proveedores CRUD
        public static DataTable GetAllProveedores()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT idProveedor, nombre, telefono, email, direccion, FechaCreacion FROM app.Proveedores ORDER BY nombre", conn);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public static DataRow GetProveedorById(int id)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT idProveedor, nombre, telefono, email, direccion FROM app.Proveedores WHERE idProveedor = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public static int InsertProveedor(string nombre, string telefono = null, string email = null, string direccion = null)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("INSERT INTO app.Proveedores (nombre, telefono, email, direccion) VALUES (@nombre, @telefono, @email, @direccion); SELECT SCOPE_IDENTITY();", conn);
            cmd.Parameters.AddWithValue("@nombre", nombre ?? string.Empty);
            cmd.Parameters.AddWithValue("@telefono", (object)telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object)email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object)direccion ?? DBNull.Value);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static int UpdateProveedor(int id, string nombre, string telefono = null, string email = null, string direccion = null)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("UPDATE app.Proveedores SET nombre=@nombre, telefono=@telefono, email=@email, direccion=@direccion WHERE idProveedor=@id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nombre", nombre ?? string.Empty);
            cmd.Parameters.AddWithValue("@telefono", (object)telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object)email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object)direccion ?? DBNull.Value);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static int DeleteProveedor(int id)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("DELETE FROM app.Proveedores WHERE idProveedor=@id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        // Ventas CRUD
        public static DataTable GetAllVentas()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            try
            {
                using var cmd = new SqlCommand("SELECT vm.idVenta, vm.fecha, vm.total, vm.clienteNombre, vm.clienteTelefono, vm.clienteEmail, (SELECT COUNT(*) FROM app.VentasDetalles vd WHERE vd.ventaId = vm.idVenta) AS itemsCount FROM app.VentasMaster vm ORDER BY vm.fecha DESC", conn);
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch
            {
                // Fallback a la tabla antigua si no existe la estructura nueva
                using var cmd = new SqlCommand("SELECT v.idVenta, v.fecha, v.productoId, p.Nombre AS producto, v.cantidad, v.total, v.clienteNombre, v.clienteTelefono, v.clienteEmail FROM app.Ventas v LEFT JOIN app.Productos p ON v.productoId = p.Id ORDER BY v.fecha DESC", conn);
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public static int InsertVenta(int productoId, int cantidad, decimal total, string? clienteNombre = null, string? clienteTelefono = null, string? clienteEmail = null)
        {
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                // Leer stock actual con bloqueo para evitar race conditions
                using (var chk = new SqlCommand("SELECT Cantidad FROM app.Productos WITH (UPDLOCK, ROWLOCK) WHERE Id = @id", conn, tran))
                {
                    chk.Parameters.AddWithValue("@id", productoId);
                    var obj = chk.ExecuteScalar();
                    if (obj == null || obj == DBNull.Value)
                    {
                        throw new InvalidOperationException("Producto no encontrado.");
                    }
                    var available = Convert.ToInt32(obj);
                    if (available < cantidad)
                    {
                        throw new InvalidOperationException($"Stock insuficiente. Disponible: {available}, requerido: {cantidad}.");
                    }
                }

                using var cmd = new SqlCommand("INSERT INTO app.Ventas (fecha, productoId, cantidad, total, clienteNombre, clienteTelefono, clienteEmail) VALUES (SYSUTCDATETIME(), @productoId, @cantidad, @total, @clienteNombre, @clienteTelefono, @clienteEmail); SELECT SCOPE_IDENTITY();", conn, tran);
                cmd.Parameters.AddWithValue("@productoId", productoId);
                cmd.Parameters.AddWithValue("@cantidad", cantidad);
                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@clienteNombre", (object)clienteNombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteTelefono", (object)clienteTelefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteEmail", (object)clienteEmail ?? DBNull.Value);
                var newId = Convert.ToInt32(cmd.ExecuteScalar());

                // Ajustar stock del producto (restar cantidad)
                using var upd = new SqlCommand("UPDATE app.Productos SET Cantidad = Cantidad - @qty WHERE Id = @id; SELECT @@ROWCOUNT;", conn, tran);
                upd.Parameters.AddWithValue("@qty", cantidad);
                upd.Parameters.AddWithValue("@id", productoId);
                var rowsAffectedObj = upd.ExecuteScalar();
                var rowsAffected = rowsAffectedObj == null || rowsAffectedObj == DBNull.Value ? 0 : Convert.ToInt32(rowsAffectedObj);
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException("No se pudo actualizar el stock del producto.");
                }

                tran.Commit();
                return newId; // no-op patch to ensure placement
            }
            catch
            {
                try { tran.Rollback(); } catch { }
                throw;
            }
        }

        // Insertar venta maestra con varias lineas (carrito)
        public static int InsertVentaWithDetails(DataTable cartTable, string? clienteNombre = null, string? clienteTelefono = null, string? clienteEmail = null)
        {
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                // validar stock para todos los items con bloqueo
                foreach (DataRow r in cartTable.Rows)
                {
                    var pid = Convert.ToInt32(r["productoId"]);
                    var qty = Convert.ToInt32(r["Cantidad"]);
                    using var chk = new SqlCommand("SELECT Cantidad FROM app.Productos WITH (UPDLOCK, ROWLOCK) WHERE Id = @id", conn, tran);
                    chk.Parameters.AddWithValue("@id", pid);
                    var obj = chk.ExecuteScalar();
                    if (obj == null || obj == DBNull.Value) throw new InvalidOperationException("Producto no encontrado.");
                    var available = Convert.ToInt32(obj);
                    if (available < qty) throw new InvalidOperationException($"Stock insuficiente para producto {pid}. Disponible: {available}, requerido: {qty}.");
                }

                // insertar venta master
                decimal total = cartTable.AsEnumerable().Sum(r => r.Field<decimal>("Total"));
                using var cmd = new SqlCommand("INSERT INTO app.VentasMaster (fecha, total, clienteNombre, clienteTelefono, clienteEmail) VALUES (SYSUTCDATETIME(), @total, @clienteNombre, @clienteTelefono, @clienteEmail); SELECT SCOPE_IDENTITY();", conn, tran);
                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@clienteNombre", (object)clienteNombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteTelefono", (object)clienteTelefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteEmail", (object)clienteEmail ?? DBNull.Value);
                var newVentaId = Convert.ToInt32(cmd.ExecuteScalar());

                // insertar detalles y actualizar stock
                foreach (DataRow r in cartTable.Rows)
                {
                    var pid = Convert.ToInt32(r["productoId"]);
                    var qty = Convert.ToInt32(r["Cantidad"]);
                    var precio = Convert.ToDecimal(r["PrecioUnit"]);
                    var totalLinea = Convert.ToDecimal(r["Total"]);
                    using var ins = new SqlCommand("INSERT INTO app.VentasDetalles (ventaId, productoId, cantidad, precioUnit, total) VALUES (@ventaId, @productoId, @cantidad, @precioUnit, @total);", conn, tran);
                    ins.Parameters.AddWithValue("@ventaId", newVentaId);
                    ins.Parameters.AddWithValue("@productoId", pid);
                    ins.Parameters.AddWithValue("@cantidad", qty);
                    ins.Parameters.AddWithValue("@precioUnit", precio);
                    ins.Parameters.AddWithValue("@total", totalLinea);
                    ins.ExecuteNonQuery();

                    using var upd = new SqlCommand("UPDATE app.Productos SET Cantidad = Cantidad - @qty WHERE Id = @id; SELECT @@ROWCOUNT;", conn, tran);
                    upd.Parameters.AddWithValue("@qty", qty);
                    upd.Parameters.AddWithValue("@id", pid);
                    var rowsAffectedObj = upd.ExecuteScalar();
                    var rowsAffected = rowsAffectedObj == null || rowsAffectedObj == DBNull.Value ? 0 : Convert.ToInt32(rowsAffectedObj);
                    if (rowsAffected == 0) throw new InvalidOperationException("No se pudo actualizar el stock del producto.");
                }

                tran.Commit();
                return newVentaId;
            }
            catch
            {
                try { tran.Rollback(); } catch { }
                throw;
            }
        }

        public static int DeleteVenta(int id)
        {
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();
            try
            {
                using var tran = conn.BeginTransaction();
                try
                {
                    // intentar eliminar master + detalles
                    using (var delDet = new SqlCommand("DELETE FROM app.VentasDetalles WHERE ventaId = @id;", conn, tran)) { delDet.Parameters.AddWithValue("@id", id); delDet.ExecuteNonQuery(); }
                    using (var delMaster = new SqlCommand("DELETE FROM app.VentasMaster WHERE idVenta = @id; SELECT @@ROWCOUNT;", conn, tran)) { delMaster.Parameters.AddWithValue("@id", id); var res = delMaster.ExecuteScalar(); tran.Commit(); return Convert.ToInt32(res); }
                }
                catch
                {
                    try { tran.Rollback(); } catch { }
                    // fallback a la tabla antigua
                }
            }
            catch { }

            // fallback simple
            using var cmd = new SqlCommand("DELETE FROM app.Ventas WHERE idVenta = @id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static DataRow GetVentaById(int id)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            try
            {
                using var cmd = new SqlCommand("SELECT vm.idVenta, vm.fecha, vm.total, vm.clienteNombre, vm.clienteTelefono, vm.clienteEmail FROM app.VentasMaster vm WHERE vm.idVenta = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0) return dt.Rows[0];
            }
            catch
            {
                // fallback
            }

            using var cmd2 = new SqlCommand("SELECT v.idVenta, v.fecha, v.productoId, p.Nombre AS producto, v.cantidad, v.total, v.clienteNombre, v.clienteTelefono, v.clienteEmail FROM app.Ventas v LEFT JOIN app.Productos p ON v.productoId = p.Id WHERE v.idVenta = @id", conn);
            cmd2.Parameters.AddWithValue("@id", id);
            using var da2 = new SqlDataAdapter(cmd2);
            da2.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public static DataTable GetVentaDetalles(int ventaId)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            try
            {
                using var cmd = new SqlCommand("SELECT d.id, d.ventaId, d.productoId, p.Nombre AS producto, d.cantidad, d.precioUnit, d.total FROM app.VentasDetalles d LEFT JOIN app.Productos p ON d.productoId = p.Id WHERE d.ventaId = @id", conn);
                cmd.Parameters.AddWithValue("@id", ventaId);
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch
            {
                // ninguna acción
            }
            return dt;
        }
    }
}
