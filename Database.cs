using System;
using System.Data;
using Microsoft.Data.SqlClient;

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
    }
}
