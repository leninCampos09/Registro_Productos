using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Registro_Productos
{
    public partial class ProductoViewForm : Form
    {
        public ProductoViewForm(DataRow row)
        {
            InitializeComponent();
            if (row == null)
            {
                MessageBox.Show("No se recibió información del producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            lblNombre.Text = row["producto"].ToString();
            lblDescripcion.Text = row.Table.Columns.Contains("descripcion") && row["descripcion"] != DBNull.Value ? row["descripcion"].ToString() : string.Empty;
            // categoria
            lblCategoria.Text = row.Table.Columns.Contains("categoria") && row["categoria"] != DBNull.Value ? row["categoria"].ToString() : "(sin categoría)";
            lblCategoria.ForeColor = System.Drawing.Color.Green;
            // prefijos para campos
            lblPrecio.Text = "Precio: " + Convert.ToDecimal(row["precio"]).ToString("C2");
            lblCantidad.Text = "Cantidad: " + row["cantidad"].ToString();
            lblDisponible.Text = "Disponible: " + (Convert.ToBoolean(row["disponible"]) ? "Sí" : "No");
            var img = row["img"] == DBNull.Value ? null : row["img"].ToString();
            if (!string.IsNullOrEmpty(img))
            {
                string path = img;
                // si la ruta no es absoluta, combinar con Application.StartupPath
                if (!Path.IsPathRooted(img))
                    path = Path.Combine(Application.StartupPath, img);

                if (File.Exists(path))
                {
                    try
                    {
                        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                        using var loaded = Image.FromStream(fs);
                        pbImage.Image = new Bitmap(loaded); // copia independiente
                    }
                    catch
                    {
                        lblImgStatus.Text = "Error al cargar la imagen.";
                        lblImgStatus.Visible = true;
                        pbImage.Visible = false;
                    }
                }
                else
                {
                    lblImgStatus.Text = "Imagen no encontrada.";
                    lblImgStatus.Visible = true;
                    pbImage.Visible = false;
                }
            }
            else
            {
                lblImgStatus.Text = "Sin imagen.";
                lblImgStatus.Visible = true;
                pbImage.Visible = false;
            }

            // conectar evento del botón cerrar (si existe)
            try
            {
                if (this.Controls.ContainsKey("btnClose"))
                {
                    var btn = this.Controls["btnClose"] as Button;
                    if (btn != null) btn.Click += (s, e) => this.Close();
                }
            }
            catch { }
        }
    }
}
