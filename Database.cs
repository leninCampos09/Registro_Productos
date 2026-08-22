using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Registro_Productos
{
    public static class Database
    {
        // Cadena de conexión proporcionada por el usuario
        private static readonly string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Registro_Productos;Integrated Security=True;Connect Timeout=60;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

        public static int InsertProduct(string nombre, decimal precio, int cantidad, bool disponible, string imgPath, string descripcion = null, int? categoriaId = null)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(@"INSERT INTO Productos (producto, precio, cantidad, disponible, img, descripcion, categoriaId)
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

        public static DataTable GetAllProducts()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT p.idProducto, p.producto, p.precio, p.cantidad, p.disponible, p.img, p.descripcion, p.categoriaId, c.nombre AS categoria, p.fechaRegistro FROM Productos p LEFT JOIN Categorias c ON p.categoriaId = c.idCategoria ORDER BY p.producto", conn);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public static DataRow GetProductById(int id)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT p.idProducto, p.producto, p.precio, p.cantidad, p.disponible, p.img, p.descripcion, p.categoriaId, c.nombre AS categoria, p.fechaRegistro FROM Productos p LEFT JOIN Categorias c ON p.categoriaId = c.idCategoria WHERE p.idProducto = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public static int UpdateProduct(int id, string nombre, decimal precio, int cantidad, bool disponible, string imgPath, string descripcion = null, int? categoriaId = null)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(@"UPDATE Productos SET producto=@producto, precio=@precio, cantidad=@cantidad, disponible=@disponible, img=@img, descripcion=@descripcion, categoriaId=@categoriaId WHERE idProducto=@id; SELECT @@ROWCOUNT;", conn);
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
            using var cmd = new SqlCommand("SELECT idCategoria, nombre FROM Categorias ORDER BY nombre", conn);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public static int InsertCategory(string nombre)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("INSERT INTO Categorias (nombre) VALUES (@nombre); SELECT SCOPE_IDENTITY();", conn);
            cmd.Parameters.AddWithValue("@nombre", nombre ?? string.Empty);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static int UpdateCategory(int id, string nombre)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("UPDATE Categorias SET nombre=@nombre WHERE idCategoria=@id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nombre", nombre ?? string.Empty);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static int DeleteCategory(int id)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("DELETE FROM Categorias WHERE idCategoria=@id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static int DeleteProduct(int id)
        {
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("DELETE FROM Productos WHERE idProducto = @id; SELECT @@ROWCOUNT;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }
    }
}
