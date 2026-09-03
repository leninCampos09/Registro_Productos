using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Registro_Productos
{
    public class ProveedoresForm : Form
    {
        private DataGridView dgv;
        private Button btnNuevo;
        private Button btnRefresh;
        private Button btnEliminar;
        private Label lblSearch;
        private TextBox txtSearch;

        public ProveedoresForm()
        {
            this.Text = "Proveedores";
            this.Size = new Size(700, 460);
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponents();
            LoadProveedores();
            this.Shown += (s, e) => { try { dgv.ClearSelection(); } catch { } };
        }

        private void InitializeComponents()
        {
            dgv = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            btnNuevo = new Button { Text = "Nuevo", Location = new Point(10, 10), Size = new Size(100, 30), FlatStyle = FlatStyle.System };
            btnRefresh = new Button { Text = "Actualizar", Location = new Point(120, 10), Size = new Size(100, 30), FlatStyle = FlatStyle.System };
            btnEliminar = new Button { Text = "Eliminar", Location = new Point(230, 10), Size = new Size(100, 30), FlatStyle = FlatStyle.System };
            lblSearch = new Label { Text = "Buscar:", AutoSize = true, Location = new Point(350, 15) };
            txtSearch = new TextBox { Location = new Point(400, 12), Size = new Size(220, 23) };

            btnNuevo.Click += BtnNuevo_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnEliminar.Click += BtnEliminar_Click;
            dgv.CellDoubleClick += Dgv_CellDoubleClick;

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            pnlTop.Controls.Add(btnNuevo);
            pnlTop.Controls.Add(btnRefresh);
            pnlTop.Controls.Add(btnEliminar);
            pnlTop.Controls.Add(lblSearch);
            pnlTop.Controls.Add(txtSearch);

            this.Controls.Add(dgv);
            this.Controls.Add(pnlTop);

            txtSearch.TextChanged += (s, e) => LoadProveedores();
        }

        public void LoadProveedores()
        {
            DataTable dt;
            try
            {
                // Intentar aplicar migraciones SQL embebidas antes de consultar
                try { Database.EnsureMigrationsApplied(); } catch { /* si falla, GetAllProveedores lanzará después */ }
                dt = Database.GetAllProveedores();
                if (dt == null) dt = new DataTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dt = new DataTable();
            }

            try
            {
                var q = this.txtSearch.Text?.Trim();
                if (!string.IsNullOrEmpty(q))
                {
                    q = q.Replace("'", "''");
                    var dv = dt.DefaultView;
                    dv.RowFilter = $"nombre LIKE '%{q}%' OR email LIKE '%{q}%' OR telefono LIKE '%{q}%'";
                    dt = dv.ToTable();
                }
            }
            catch { }

            dgv.DataSource = dt;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowTemplate.Height = 28;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 144, 255);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            using var f = new ProveedorEditForm();
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                LoadProveedores();
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadProveedores();
        }

        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgv.CurrentRow == null) return;
                var id = Convert.ToInt32(dgv.CurrentRow.Cells["idProveedor"].Value);
                var nombre = dgv.CurrentRow.Cells["nombre"].Value?.ToString() ?? "";
                if (MessageBox.Show($"Eliminar proveedor '{nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Database.DeleteProveedor(id);
                    LoadProveedores();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgv_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            var prevBack = row.DefaultCellStyle.BackColor;
            var prevFore = row.DefaultCellStyle.ForeColor;
            var prevSelBack = row.DefaultCellStyle.SelectionBackColor;
            try
            {
                row.DefaultCellStyle.BackColor = Color.Red;
                row.DefaultCellStyle.ForeColor = Color.White;
                row.DefaultCellStyle.SelectionBackColor = Color.DarkRed;
                dgv.Refresh();

                var id = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["idProveedor"].Value);
                var nombre = dgv.Rows[e.RowIndex].Cells["nombre"].Value?.ToString() ?? string.Empty;
                if (MessageBox.Show($"Eliminar proveedor '{nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Database.DeleteProveedor(id);
                    LoadProveedores();
                }
                else
                {
                    row.DefaultCellStyle.BackColor = prevBack;
                    row.DefaultCellStyle.ForeColor = prevFore;
                    row.DefaultCellStyle.SelectionBackColor = prevSelBack;
                    dgv.Refresh();
                }
            }
            catch
            {
                try
                {
                    row.DefaultCellStyle.BackColor = prevBack;
                    row.DefaultCellStyle.ForeColor = prevFore;
                    row.DefaultCellStyle.SelectionBackColor = prevSelBack;
                }
                catch { }
            }
        }
    }
}
