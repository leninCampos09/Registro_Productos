using System;
using System.Windows.Forms;

namespace Registro_Productos
{
    public class ProveedorEditForm : Form
    {
        private TextBox txtNombre, txtTelefono, txtEmail, txtDireccion;
        private Button btnSave, btnCancel;
        private int editingId = 0;

        public ProveedorEditForm(int id = 0)
        {
            this.Text = id == 0 ? "Nuevo proveedor" : "Editar proveedor";
            this.Size = new System.Drawing.Size(420, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            editingId = id;
            InitializeComponents();
            if (id != 0) LoadProveedor(id);
        }

        private void InitializeComponents()
        {
            var lblNombre = new Label { Text = "Nombre:", Left = 10, Top = 15, AutoSize = true };
            txtNombre = new TextBox { Left = 110, Top = 12, Width = 280 };

            var lblTel = new Label { Text = "Teléfono:", Left = 10, Top = 50, AutoSize = true };
            txtTelefono = new TextBox { Left = 110, Top = 47, Width = 280 };

            var lblEmail = new Label { Text = "Email:", Left = 10, Top = 85, AutoSize = true };
            txtEmail = new TextBox { Left = 110, Top = 82, Width = 280 };

            var lblDir = new Label { Text = "Dirección:", Left = 10, Top = 120, AutoSize = true };
            txtDireccion = new TextBox { Left = 110, Top = 117, Width = 280, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };

            btnSave = new Button { Text = "Guardar", Left = 220, Width = 80, Top = 200, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancelar", Left = 310, Width = 80, Top = 200, DialogResult = DialogResult.Cancel };

            btnSave.Click += BtnSave_Click;

            this.Controls.AddRange(new Control[] { lblNombre, txtNombre, lblTel, txtTelefono, lblEmail, txtEmail, lblDir, txtDireccion, btnSave, btnCancel });
        }

        private void LoadProveedor(int id)
        {
            var row = Database.GetProveedorById(id);
            if (row == null) return;
            txtNombre.Text = row["nombre"]?.ToString() ?? string.Empty;
            txtTelefono.Text = row.Table.Columns.Contains("telefono") && row["telefono"] != DBNull.Value ? row["telefono"].ToString() : string.Empty;
            txtEmail.Text = row.Table.Columns.Contains("email") && row["email"] != DBNull.Value ? row["email"].ToString() : string.Empty;
            txtDireccion.Text = row.Table.Columns.Contains("direccion") && row["direccion"] != DBNull.Value ? row["direccion"].ToString() : string.Empty;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            var nombre = txtNombre.Text?.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Nombre es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            try
            {
                if (editingId == 0)
                {
                    Database.InsertProveedor(nombre, txtTelefono.Text?.Trim(), txtEmail.Text?.Trim(), txtDireccion.Text?.Trim());
                }
                else
                {
                    Database.UpdateProveedor(editingId, nombre, txtTelefono.Text?.Trim(), txtEmail.Text?.Trim(), txtDireccion.Text?.Trim());
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }
}
