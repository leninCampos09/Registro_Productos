using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Registro_Productos
{
    public class VentaDetallesForm : Form
    {
        private DataGridView dgv;
        private Label lblTotal;
        private Label lblClienteInfo;
        private Label lblFechaInfo;

        public VentaDetallesForm(DataTable detalles, DataRow venta = null)
        {
            this.Text = "Detalles de la venta";
            this.Size = new Size(720, 460);
            this.StartPosition = FormStartPosition.CenterParent;

            // Panel superior con información de la venta / cliente
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(8) };
            lblFechaInfo = new Label { Text = "Fecha: -", AutoSize = false, Width = 340, Height = 20, TextAlign = ContentAlignment.MiddleLeft };
            lblClienteInfo = new Label { Text = "Cliente: -", AutoSize = false, Width = 340, Height = 20, TextAlign = ContentAlignment.MiddleLeft };
            var lblTelefono = new Label { Text = "Tel: -", AutoSize = false, Width = 340, Height = 20, TextAlign = ContentAlignment.MiddleLeft };
            var lblEmail = new Label { Text = "Email: -", AutoSize = false, Width = 340, Height = 20, TextAlign = ContentAlignment.MiddleLeft };

            // Colocar labels en dos filas
            lblFechaInfo.Left = 8; lblFechaInfo.Top = 8;
            lblClienteInfo.Left = 8; lblClienteInfo.Top = 30;
            lblTelefono.Left = 360; lblTelefono.Top = 8;
            lblEmail.Left = 360; lblEmail.Top = 30;

            topPanel.Controls.Add(lblFechaInfo);
            topPanel.Controls.Add(lblClienteInfo);
            topPanel.Controls.Add(lblTelefono);
            topPanel.Controls.Add(lblEmail);

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 300,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            lblTotal = new Label { Text = "Total: 0.00", Dock = DockStyle.Bottom, Height = 36, TextAlign = ContentAlignment.MiddleRight, Padding = new Padding(8) };

            this.Controls.Add(dgv);
            this.Controls.Add(lblTotal);
            this.Controls.Add(topPanel);

            if (venta != null)
            {
                try
                {
                    if (venta.Table.Columns.Contains("fecha") && venta["fecha"] != DBNull.Value)
                    {
                        var dt = Convert.ToDateTime(venta["fecha"]);
                        dt = Database.ConvertUtcToElSalvador(dt);
                        lblFechaInfo.Text = $"Fecha: {dt:dd/MM/yyyy}    Hora: {dt:hh:mm tt}";
                    }
                    if (venta.Table.Columns.Contains("clienteNombre") && venta["clienteNombre"] != DBNull.Value)
                        lblClienteInfo.Text = $"Cliente: {venta["clienteNombre"]}";
                    if (venta.Table.Columns.Contains("clienteTelefono") && venta["clienteTelefono"] != DBNull.Value)
                        lblTelefono.Text = $"Tel: {venta["clienteTelefono"]}";
                    if (venta.Table.Columns.Contains("clienteEmail") && venta["clienteEmail"] != DBNull.Value)
                        lblEmail.Text = $"Email: {venta["clienteEmail"]}";
                    if (venta.Table.Columns.Contains("total") && venta["total"] != DBNull.Value)
                        lblTotal.Text = $"Total: {Convert.ToDecimal(venta["total"]):F2}";
                }
                catch { }
            }

            if (detalles != null)
            {
                dgv.DataSource = detalles.Copy();
                // calcular total si no vino en venta
                try
                {
                    if (lblTotal.Text == "Total: 0.00")
                    {
                        var total = detalles.AsEnumerable().Sum(r => r.Table.Columns.Contains("total") && r["total"] != DBNull.Value ? Convert.ToDecimal(r["total"]) : 0m);
                        lblTotal.Text = $"Total: {total:F2}";
                    }
                }
                catch { }
            }
        }
    }
}
