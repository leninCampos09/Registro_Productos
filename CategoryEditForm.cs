using System;
using System.Drawing;
using System.Windows.Forms;

namespace Registro_Productos
{
    public class CategoryEditForm : Form
    {
        private TextBox txtName;
        private Button btnSave;
        private Label lblNombre;
        private Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel pnlButtons;
        private int editingId = 0;

        public CategoryEditForm(int id = 0, string nombre = null)
        {
            editingId = id;
            this.Text = id == 0 ? "Nueva categoría" : "Editar categoría";
            this.Size = new Size(380, 180);
            // No permitir reducir la ventana a un tamaño en el que el botón quede oculto
            this.MinimumSize = new Size(360, 180);
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponents();
            if (!string.IsNullOrEmpty(nombre)) txtName.Text = nombre.ToUpperInvariant();
        }

        private void InitializeComponents()
        {
            lblTitle = new Label { Text = this.Text, AutoSize = true, Location = new Point(12, 8), Font = new Font("Segoe UI", 11F, FontStyle.Bold) };
            lblNombre = new Label { Text = "Nombre", AutoSize = true, Location = new Point(12, 36), Name = "lblNombre" };
            txtName = new TextBox { Location = new Point(12, 60), Width = 340, Name = "txtName" };

            // panel inferior para botones (alineado a la derecha)
            pnlButtons = new System.Windows.Forms.FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                Padding = new Padding(12, 8, 12, 8),
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoSize = false
            };
            btnSave = new Button { Text = "Guardar", Size = new Size(100, 30), Name = "btnSave" };
            btnSave.Margin = new Padding(6, 8, 6, 8);
            btnSave.Click += BtnSave_Click;
            pnlButtons.Controls.Add(btnSave);

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblNombre);
            this.Controls.Add(txtName);
            this.Controls.Add(pnlButtons);

            // pulsar Enter activa Guardar
            this.AcceptButton = btnSave;
            // ajustar posición del panel de botones cuando cambie el tamaño o el estado de la ventana
            this.Shown += (s, e) => AdjustButtonsPosition();
            this.SizeChanged += (s, e) => AdjustButtonsPosition();
            this.Resize += (s, e) => AdjustButtonsPosition();
        }

        private void AdjustButtonsPosition()
        {
            try
            {
                if (pnlButtons == null || txtName == null) return;

                if (this.WindowState == FormWindowState.Maximized)
                {
                    // colocar justo debajo del campo
                    pnlButtons.Dock = DockStyle.None;
                    pnlButtons.Width = txtName.Width;
                    pnlButtons.Location = new Point(txtName.Left, txtName.Bottom + 8);
                    pnlButtons.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                }
                else
                {
                    // volver a pie de página
                    pnlButtons.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                    pnlButtons.Dock = DockStyle.Bottom;
                }
            }
            catch { }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            var nombre = txtName.Text?.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese el nombre de la categoría.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            try
            {
                // Guardar siempre en mayúsculas
                nombre = nombre.ToUpperInvariant();
                if (editingId == 0)
                {
                    Database.InsertCategory(nombre);
                }
                else
                {
                    Database.UpdateCategory(editingId, nombre);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar categoría: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
