using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Registro_Productos
{
    public class ComprasForm : Form
    {
        private ComboBox cboProveedor;
        private DateTimePicker dtpFecha;
        private RadioButton rbMayorista;
        private RadioButton rbMinorista;
        private TextBox txtFactura;
        private DataGridView dgvLineas;
        private TextBox txtProducto;
        private NumericUpDown numCantidad;
        private TextBox txtPrecio;
        private Button btnAddLinea;
        private Label lblTotal;
        private Button btnGuardar;
        private Button btnCancelar;

        public ComprasForm()
        {
            InitializeComponent();
            LoadProveedores();
        }

        private void InitializeComponent()
        {
            this.Text = "Compras";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            var tl = new TableLayoutPanel();
            tl.Dock = DockStyle.Fill;
            tl.ColumnCount = 3;
            tl.RowCount = 6;
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // header
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // proveedor/fact
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 36)); // tipo
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 36)); // add linea
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // grid
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // acciones

            // Row 0: Fecha y factura
            dtpFecha = new DateTimePicker() { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
            txtFactura = new TextBox() { Dock = DockStyle.Fill, PlaceholderText = "Nº Factura/Referencia" };
            tl.Controls.Add(new Label() { Text = "Fecha", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
            tl.Controls.Add(dtpFecha, 1, 0);
            tl.Controls.Add(txtFactura, 2, 0);

            // Row 1: Proveedor
            cboProveedor = new ComboBox() { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            tl.Controls.Add(new Label() { Text = "Proveedor", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 1);
            tl.SetColumnSpan(cboProveedor, 2);
            tl.Controls.Add(cboProveedor, 1, 1);

            // Row 2: Tipo compra
            rbMayorista = new RadioButton() { Text = "Al por mayor", AutoSize = true };
            rbMinorista = new RadioButton() { Text = "Minorista", AutoSize = true };
            rbMayorista.Checked = true;
            // Usar FlowLayoutPanel para evitar que un radio quede oculto
            var pTipo = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            pTipo.Controls.Add(rbMayorista);
            pTipo.Controls.Add(rbMinorista);
            tl.Controls.Add(new Label() { Text = "Tipo", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 2);
            tl.SetColumnSpan(pTipo, 2);
            tl.Controls.Add(pTipo, 1, 2);

            // Row 3: Agregar línea (producto, cantidad, precio, boton)
            txtProducto = new TextBox() { Dock = DockStyle.Fill, PlaceholderText = "Producto (nombre o código)" };
            numCantidad = new NumericUpDown() { Dock = DockStyle.Fill, Minimum = 1, Maximum = 100000, Value = 1 };
            txtPrecio = new TextBox() { Dock = DockStyle.Fill, PlaceholderText = "Precio unitario" };
            btnAddLinea = new Button() { Text = "Agregar", AutoSize = true };
            btnAddLinea.Click += BtnAddLinea_Click;

            // Crear un panel horizontal para que el botón sea visible junto a los campos
            var pLine = new TableLayoutPanel() { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1 };
            pLine.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); // producto
            pLine.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15)); // cantidad
            pLine.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15)); // precio
            pLine.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10)); // boton
            pLine.Controls.Add(txtProducto, 0, 0);
            pLine.Controls.Add(numCantidad, 1, 0);
            pLine.Controls.Add(txtPrecio, 2, 0);
            pLine.Controls.Add(btnAddLinea, 3, 0);

            tl.SetColumnSpan(pLine, 3);
            tl.Controls.Add(pLine, 0, 3);

            // Permitir añadir con Enter en el campo precio
            txtPrecio.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    BtnAddLinea_Click(btnAddLinea, EventArgs.Empty);
                }
            };

            // Row 4: DataGridView
            dgvLineas = new DataGridView();
            dgvLineas.Dock = DockStyle.Fill;
            dgvLineas.AllowUserToAddRows = false;
            dgvLineas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // estilo visual
            dgvLineas.BackgroundColor = Color.WhiteSmoke;
            dgvLineas.EnableHeadersVisualStyles = false;
            dgvLineas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvLineas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLineas.ColumnHeadersDefaultCellStyle.Font = new Font(dgvLineas.Font, FontStyle.Bold);
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Producto", HeaderText = "Producto", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Cantidad", HeaderText = "Cantidad", Width = 80 });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Precio", HeaderText = "Precio unitario", Width = 120 });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Subtotal", HeaderText = "Subtotal", Width = 120 });
            tl.SetColumnSpan(dgvLineas, 3);
            tl.Controls.Add(dgvLineas, 0, 4);

            // Row 5: acciones y total
            lblTotal = new Label() { Text = "Total: 0.00", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font(FontFamily.GenericSansSerif, 10F, FontStyle.Bold) };
            btnGuardar = new Button() { Text = "Guardar compra", Dock = DockStyle.Right, AutoSize = true };
            btnCancelar = new Button() { Text = "Cancelar", Dock = DockStyle.Right, AutoSize = true };
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += (s, e) => this.Close();

            var pActions = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            pActions.Controls.Add(btnGuardar);
            pActions.Controls.Add(btnCancelar);
            pActions.Controls.Add(lblTotal);
            tl.SetColumnSpan(pActions, 3);
            tl.Controls.Add(pActions, 0, 5);

            // Colores y estilos del formulario
            this.BackColor = Color.FromArgb(249, 250, 252);
            btnAddLinea.BackColor = Color.FromArgb(52, 152, 219);
            btnAddLinea.ForeColor = Color.White;
            btnGuardar.BackColor = Color.FromArgb(46, 204, 113);
            btnGuardar.ForeColor = Color.White;
            btnCancelar.BackColor = Color.LightGray;
            btnCancelar.ForeColor = Color.Black;

            this.Controls.Add(tl);
        }

        private void LoadProveedores()
        {
            try
            {
                var dt = Database.GetAllProveedores();
                if (dt != null)
                {
                    var dtCopy = dt.Copy();
                    // Agregar fila vacía para selección nula
                    var empty = dtCopy.NewRow();
                    empty["idProveedor"] = DBNull.Value;
                    empty["nombre"] = "-- Seleccione proveedor --";
                    dtCopy.Rows.InsertAt(empty, 0);

                    cboProveedor.DataSource = dtCopy;
                    cboProveedor.DisplayMember = "nombre";
                    cboProveedor.ValueMember = "idProveedor";
                }
            }
            catch
            {
                // ignorar errores de carga para no bloquear la UI
            }
        }

        private void BtnAddLinea_Click(object? sender, EventArgs e)
        {
            var producto = txtProducto.Text?.Trim();
            if (string.IsNullOrEmpty(producto))
            {
                MessageBox.Show("Ingrese el nombre o código del producto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtProducto.Focus();
                return;
            }

            var cantidad = (int)numCantidad.Value;
            // Aceptar tanto coma como punto como separador decimal
            decimal precio;
            var priceText = txtPrecio.Text?.Trim() ?? string.Empty;
            var parsed = decimal.TryParse(priceText, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out precio);
            if (!parsed)
            {
                // Intentar con invariant (punto)
                parsed = decimal.TryParse(priceText.Replace(',', '.'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out precio);
            }
            if (!parsed || precio < 0)
            {
                MessageBox.Show("Ingrese un precio válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPrecio.Focus();
                return;
            }

            var subtotal = Math.Round(cantidad * precio, 2);
            dgvLineas.Rows.Add(producto, cantidad.ToString(), precio.ToString("F2"), subtotal.ToString("F2"));
            txtProducto.Text = "";
            numCantidad.Value = 1;
            txtPrecio.Text = "";
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            try
            {
                decimal total = 0M;
                foreach (DataGridViewRow r in dgvLineas.Rows)
                {
                    if (r.Cells[3].Value != null && decimal.TryParse(r.Cells[3].Value.ToString(), out var s))
                        total += s;
                }
                lblTotal.Text = "Total: " + total.ToString("F2");
            }
            catch { }
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            // Validaciones mínimas
            if (dgvLineas.Rows.Count == 0)
            {
                MessageBox.Show("No hay líneas en la compra.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // Validar que la fecha de la compra no sea futura
            if (dtpFecha != null && dtpFecha.Value.Date > DateTime.Now.Date)
            {
                SweetAlert.ShowError(this, "Fecha inválida", $"La fecha de la compra no puede ser en el futuro. Fecha actual: {DateTime.Now:d}.");
                return;
            }

            var proveedorId = cboProveedor.SelectedValue == DBNull.Value ? (int?)null : Convert.ToInt32(cboProveedor.SelectedValue);
            var tipo = rbMayorista.Checked ? "Mayorista" : "Minorista";
            var fecha = dtpFecha.Value;
            var factura = txtFactura.Text?.Trim();

            // Construir líneas
            var lineas = new System.Collections.Generic.List<Database.CompraLinea>();
            decimal total = 0M;
            foreach (DataGridViewRow r in dgvLineas.Rows)
            {
                var prod = r.Cells[0].Value?.ToString() ?? string.Empty;
                if (!int.TryParse(r.Cells[1].Value?.ToString(), out var cantidad)) cantidad = 0;
                if (!decimal.TryParse(r.Cells[2].Value?.ToString(), out var precio)) precio = 0M;
                if (!decimal.TryParse(r.Cells[3].Value?.ToString(), out var subtotal)) subtotal = Math.Round(cantidad * precio, 2);
                total += subtotal;

                // Intentar resolver producto por nombre; si existe, guardar productoId y dejar descripcion vacía
                int? pid = null;
                try { pid = Database.GetProductoIdByName(prod); } catch { pid = null; }

                var linea = new Database.CompraLinea
                {
                    productoId = pid,
                    descripcion = pid == null ? prod : null,
                    cantidad = cantidad,
                    precio = precio,
                    subtotal = subtotal
                };
                lineas.Add(linea);
            }

            try
            {
                var id = Database.InsertCompraWithDetails(proveedorId, fecha, tipo, factura, total, lineas);
                MessageBox.Show($"Compra guardada. Id: {id}. Total: {total:F2}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la compra: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
