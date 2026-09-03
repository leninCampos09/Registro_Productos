using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Registro_Productos
{
    public class CompraDetallesForm : Form
    {
        private int compraId;
        private DataGridView dgvDetalles;

        public CompraDetallesForm(int compraId)
        {
            this.compraId = compraId;
            InitializeComponent();
            LoadDetalles();
        }

        private void InitializeComponent()
        {
            this.Text = $"Detalles Compra #{compraId}";
            this.Size = new Size(700, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            dgvDetalles = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false };
            dgvDetalles.BackgroundColor = Color.White;
            dgvDetalles.EnableHeadersVisualStyles = false;
            dgvDetalles.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvDetalles.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDetalles.ColumnHeadersDefaultCellStyle.Font = new Font(dgvDetalles.Font, FontStyle.Bold);
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdDetalle", HeaderText = "Id", DataPropertyName = "IdDetalle", Width = 60 });
            // Mostrar nombre del producto cuando exista; mantener productoId oculto para referencia
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "Producto", HeaderText = "Producto", DataPropertyName = "producto", Width = 180 });
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProductoId", HeaderText = "ProductoId", DataPropertyName = "productoId", Width = 80, Visible = false });
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descripcion", HeaderText = "Descripcion", DataPropertyName = "descripcion", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cantidad", HeaderText = "Cantidad", DataPropertyName = "cantidad", Width = 80 });
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "Precio", HeaderText = "Precio unitario", DataPropertyName = "precio", Width = 100 });
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal", HeaderText = "Subtotal", DataPropertyName = "subtotal", Width = 100 });

            this.BackColor = Color.FromArgb(250, 250, 250);
            this.Controls.Add(dgvDetalles);
        }

        private void LoadDetalles()
        {
            try
            {
                var dt = Database.GetCompraDetalles(compraId);
                dgvDetalles.DataSource = dt;

                // Ocultar la columna Descripcion si en todos los registros coincide con Producto o está vacía
                try
                {
                    bool hideDesc = true;
                    foreach (DataRow r in dt.Rows)
                    {
                        var prod = (r.Table.Columns.Contains("producto") ? (r["producto"]?.ToString() ?? string.Empty) : string.Empty).Trim();
                        var desc = (r.Table.Columns.Contains("descripcion") ? (r["descripcion"]?.ToString() ?? string.Empty) : string.Empty).Trim();
                        if (!(string.IsNullOrEmpty(desc) || string.Equals(prod, desc, StringComparison.OrdinalIgnoreCase)))
                        {
                            hideDesc = false; break;
                        }
                    }
                    if (dgvDetalles.Columns.Contains("Descripcion")) dgvDetalles.Columns["Descripcion"].Visible = !hideDesc;
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando detalles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
