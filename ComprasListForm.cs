using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Registro_Productos
{
    public class ComprasListForm : Form
    {
        private DataGridView dgvCompras;
        private Button btnRefresh;
        private Button btnVerDetalles;
        private Button btnNuevo;
        private Button btnEliminar;
        private ComboBox cbProveedor;
        private TextBox txtSearch;
        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;
        private Button btnAplicar;

        public ComprasListForm()
        {
            InitializeComponent();
            LoadCompras();
        }
        private void DgvCompras_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvCompras.Rows[e.RowIndex];
            var prevBack = row.DefaultCellStyle.BackColor;
            var prevFore = row.DefaultCellStyle.ForeColor;
            var prevSelBack = row.DefaultCellStyle.SelectionBackColor;
            try
            {
                row.DefaultCellStyle.BackColor = Color.Red;
                row.DefaultCellStyle.ForeColor = Color.White;
                row.DefaultCellStyle.SelectionBackColor = Color.DarkRed;
                dgvCompras.Refresh();

                var id = Convert.ToInt32(row.Cells["IdCompra"].Value);
                var r = MessageBox.Show("¿Eliminar esta compra?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    Database.DeleteCompra(id);
                    LoadCompras();
                }
                else
                {
                    row.DefaultCellStyle.BackColor = prevBack;
                    row.DefaultCellStyle.ForeColor = prevFore;
                    row.DefaultCellStyle.SelectionBackColor = prevSelBack;
                    dgvCompras.Refresh();
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


        private void InitializeComponent()
        {
            this.Text = "Compras - Listado";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            dgvCompras = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false };
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdCompra", HeaderText = "Id", DataPropertyName = "IdCompra", Width = 60 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Proveedor", HeaderText = "Proveedor", DataPropertyName = "proveedorNombre", Width = 180 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Fecha", HeaderText = "Fecha", DataPropertyName = "fecha", Width = 150 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tipo", HeaderText = "Tipo", DataPropertyName = "tipo", Width = 120 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Factura", HeaderText = "Factura", DataPropertyName = "factura", Width = 120 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total", DataPropertyName = "total", Width = 120 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "FechaCreacion", HeaderText = "Creado", DataPropertyName = "FechaCreacion", Width = 160 });

            btnRefresh = new Button { Text = "Refrescar", AutoSize = true };
            btnVerDetalles = new Button { Text = "Ver detalles", AutoSize = true };
            btnNuevo = new Button { Text = "Nueva compra", AutoSize = true };
            btnEliminar = new Button { Text = "Eliminar", AutoSize = true };

            cbProveedor = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };
            txtSearch = new TextBox { Width = 180 };
            dtpDesde = new DateTimePicker { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Value = DateTime.Now.Date.AddDays(-30), Width = 110 };
            dtpHasta = new DateTimePicker { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Value = DateTime.Now.Date, Width = 110 };
            btnRefresh.Click += BtnRefresh_Click;
            btnVerDetalles.Click += BtnVerDetalles_Click;
            btnNuevo.Click += BtnNuevo_Click;
            btnEliminar.Click += BtnEliminar_Click;

            txtSearch.TextChanged += (s, e) => LoadCompras();
            cbProveedor.SelectedIndexChanged += (s, e) => LoadCompras();
            // mostrar botón Aplicar cuando el usuario interactúe con los datepickers
            dtpDesde.ValueChanged += (s, e) => { btnAplicar.Visible = true; };
            dtpDesde.MouseUp += (s, e) => { btnAplicar.Visible = true; };
            dtpHasta.ValueChanged += (s, e) => { btnAplicar.Visible = true; };
            dtpHasta.MouseUp += (s, e) => { btnAplicar.Visible = true; };

            var pTop = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 56, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(8) };
            // primera fila de acciones
            btnRefresh.Margin = new Padding(6, 6, 6, 6);
            btnVerDetalles.Margin = new Padding(6, 6, 6, 6);
            btnNuevo.Margin = new Padding(6, 6, 6, 6);
            btnEliminar.Margin = new Padding(6, 6, 6, 6);
            pTop.Controls.Add(btnRefresh);
            pTop.Controls.Add(btnVerDetalles);
            pTop.Controls.Add(btnNuevo);
            pTop.Controls.Add(btnEliminar);
            // separador visual
            var sep = new Label { AutoSize = false, Width = 14 };
            pTop.Controls.Add(sep);
            // filtros
            var lblProv = new Label { Text = "Proveedor:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
            lblProv.Margin = new Padding(8, 10, 4, 4);
            cbProveedor.Margin = new Padding(0, 6, 12, 6);
            pTop.Controls.Add(lblProv);
            pTop.Controls.Add(cbProveedor);
            var lblSearch = new Label { Text = "Buscar:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
            lblSearch.Margin = new Padding(0, 10, 4, 4);
            txtSearch.Margin = new Padding(0, 6, 12, 6);
            pTop.Controls.Add(lblSearch);
            pTop.Controls.Add(txtSearch);
            var lblDesde = new Label { Text = "Desde:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
            var lblHasta = new Label { Text = "Hasta:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
            lblDesde.Margin = new Padding(0, 10, 4, 4);
            dtpDesde.Margin = new Padding(0, 6, 12, 6);
            lblHasta.Margin = new Padding(0, 10, 4, 4);
            dtpHasta.Margin = new Padding(0, 6, 6, 6);
            pTop.Controls.Add(lblDesde);
            pTop.Controls.Add(dtpDesde);
            pTop.Controls.Add(lblHasta);
            pTop.Controls.Add(dtpHasta);

            // botón Aplicar para activar filtro por fecha cuando el usuario lo desee
            btnAplicar = new Button { Text = "Aplicar", AutoSize = true, Visible = false };
            btnAplicar.Margin = new Padding(8, 8, 8, 8);
            btnAplicar.Click += (s, e) => { btnAplicar.Visible = false; LoadCompras(); };
            pTop.Controls.Add(btnAplicar);

            // Doble clic: marcar en rojo y confirmar eliminación (comportamiento similar a Productos)
            dgvCompras.CellDoubleClick += DgvCompras_CellDoubleClick;

            // Estilos
            this.BackColor = Color.FromArgb(250, 250, 250);
            dgvCompras.BackgroundColor = Color.White;
            dgvCompras.EnableHeadersVisualStyles = false;
            dgvCompras.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvCompras.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCompras.ColumnHeadersDefaultCellStyle.Font = new Font(dgvCompras.Font, FontStyle.Bold);

            btnRefresh.BackColor = Color.FromArgb(52, 152, 219);
            btnRefresh.ForeColor = Color.White;
            btnVerDetalles.BackColor = Color.FromArgb(46, 204, 113);
            btnVerDetalles.ForeColor = Color.White;
            btnNuevo.BackColor = Color.FromArgb(41, 128, 185);
            btnNuevo.ForeColor = Color.White;

            this.Controls.Add(dgvCompras);
            this.Controls.Add(pTop);

            // llenar proveedores y mostrar los date pickers como selector de periodo
            LoadProveedoresIntoCombo();

            // Formateo de fechas para mostrar hora local (sin offset +00:00)
            dgvCompras.CellFormatting += DgvCompras_CellFormatting;
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            try
            {
                // Restaurar filtros a su estado por defecto
                try { txtSearch.Text = string.Empty; } catch { }
                try { if (cbProveedor != null && cbProveedor.Items.Count > 0) cbProveedor.SelectedIndex = 0; } catch { }
                try { if (dtpDesde != null) { dtpDesde.Checked = false; dtpDesde.Value = DateTime.Now.Date.AddDays(-30); } } catch { }
                try { if (dtpHasta != null) { dtpHasta.Checked = false; dtpHasta.Value = DateTime.Now.Date; } } catch { }

                // Recargar datos
                LoadCompras();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al refrescar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvCompras.SelectedRows.Count == 0) return;
                var idObj = dgvCompras.SelectedRows[0].Cells["IdCompra"].Value;
                if (idObj == null) return;
                var id = Convert.ToInt32(idObj);
                if (MessageBox.Show($"Eliminar compra #{id}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Database.DeleteCompra(id);
                    LoadCompras();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar compra: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            try
            {
                var t = typeof(ComprasForm);
                var frm = (Form)Activator.CreateInstance(t)!;
                frm.StartPosition = FormStartPosition.CenterParent;
                // Show as dialog and refresh after close
                frm.ShowDialog(this);
                LoadCompras();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el formulario de nueva compra: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCompras()
        {
            try
            {
                var dt = Database.GetAllCompras() ?? new DataTable();

                // Aplicar filtros UI (proveedor, búsqueda, periodo/fechas)
                var q = txtSearch?.Text?.Trim();
                var provId = 0;
                try { provId = cbProveedor?.SelectedValue == null ? 0 : Convert.ToInt32(cbProveedor.SelectedValue); } catch { provId = 0; }

                DateTime? desde = null, hasta = null;
                // usar datepickers si están marcados
                if (dtpDesde != null && dtpDesde.ShowCheckBox && dtpDesde.Checked)
                {
                    desde = dtpDesde.Value.Date;
                }
                if (dtpHasta != null && dtpHasta.ShowCheckBox && dtpHasta.Checked)
                {
                    hasta = dtpHasta.Value.Date.AddDays(1).AddTicks(-1);
                }

                // Validar que las fechas no sean posteriores a septiembre de 2026
                try
                {
                    var cutoff = new DateTime(2026, 9, 30);
                    var desdeInvalid = (dtpDesde != null && dtpDesde.ShowCheckBox && dtpDesde.Checked && dtpDesde.Value.Date > cutoff);
                    var hastaInvalid = (dtpHasta != null && dtpHasta.ShowCheckBox && dtpHasta.Checked && dtpHasta.Value.Date > cutoff);
                    if (desdeInvalid || hastaInvalid)
                    {
                        MessageBox.Show("No se permiten fechas posteriores a septiembre de 2026.", "Fecha inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        // Ignorar filtros de fecha inválidos
                        desde = null;
                        hasta = null;
                    }
                }
                catch { }

                var result = dt.Clone();
                foreach (DataRow r in dt.Rows)
                {
                    var ok = true;
                    if (provId > 0)
                    {
                        try
                        {
                            var pv = r["proveedorId"];
                            if (pv == DBNull.Value) { ok = false; }
                            else if (Convert.ToInt32(pv) != provId) ok = false;
                        }
                        catch { ok = false; }
                    }

                    if (!string.IsNullOrEmpty(q) && ok)
                    {
                        var tipo = (r.Table.Columns.Contains("tipo") ? (r["tipo"]?.ToString() ?? "") : "").ToLowerInvariant();
                        var factura = (r.Table.Columns.Contains("factura") ? (r["factura"]?.ToString() ?? "") : "").ToLowerInvariant();
                        var sq = q.ToLowerInvariant();
                        if (!tipo.Contains(sq) && !factura.Contains(sq)) ok = false;
                    }

                    if (ok && (desde.HasValue || hasta.HasValue))
                    {
                        try
                        {
                            var fv = r["fecha"];
                            DateTime rowDate;
                            if (fv is DateTimeOffset dto) rowDate = dto.UtcDateTime;
                            else if (fv is DateTime dtv) rowDate = dtv;
                            else if (!DateTime.TryParse(fv?.ToString(), out rowDate)) { ok = false; rowDate = DateTime.MinValue; }

                            if (ok)
                            {
                                if (desde.HasValue && rowDate < desde.Value) ok = false;
                                if (hasta.HasValue && rowDate > hasta.Value) ok = false;
                            }
                        }
                        catch { ok = false; }
                    }

                    if (ok) result.ImportRow(r);
                }

                dgvCompras.DataSource = result;

                // Si se aplicó filtro por fecha y no hay resultados, informar al usuario
                try
                {
                    var dateFilterActive = (dtpDesde != null && dtpDesde.ShowCheckBox && dtpDesde.Checked) || (dtpHasta != null && dtpHasta.ShowCheckBox && dtpHasta.Checked);
                    if (dateFilterActive && (result.Rows == null || result.Rows.Count == 0))
                    {
                        MessageBox.Show("No se encontraron compras para el periodo seleccionado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando compras: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProveedoresIntoCombo()
        {
            try
            {
                var dt = Database.GetAllProveedores() ?? new DataTable();
                var dt2 = dt.Copy();
                var newRow = dt2.NewRow();
                if (dt2.Columns.Contains("idProveedor")) newRow["idProveedor"] = 0;
                if (dt2.Columns.Contains("nombre")) newRow["nombre"] = "Todos";
                dt2.Rows.InsertAt(newRow, 0);
                cbProveedor.DisplayMember = dt2.Columns.Contains("nombre") ? "nombre" : dt2.Columns[0].ColumnName;
                cbProveedor.ValueMember = dt2.Columns.Contains("idProveedor") ? "idProveedor" : dt2.Columns[0].ColumnName;
                cbProveedor.DataSource = dt2;
            }
            catch { }
        }

        private void DgvCompras_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                var colName = dgvCompras.Columns[e.ColumnIndex].Name;
                if ((colName == "Fecha" || colName == "FechaCreacion") && e.Value != null)
                {
                    // Manejar DateTimeOffset o DateTime
                    if (e.Value is DateTimeOffset dto)
                    {
                        // Convertir UTC/offset a hora local de El Salvador (Central America Standard Time)
                        try
                        {
                            var utc = dto.UtcDateTime;
                            var tz = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
                            var local = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
                            e.Value = local.ToString("g");
                            e.FormattingApplied = true;
                        }
                        catch
                        {
                            e.Value = dto.LocalDateTime.ToString("g");
                            e.FormattingApplied = true;
                        }
                    }
                    else if (e.Value is DateTime dt)
                    {
                        // Asegurar que mostramos en la zona deseada
                        try
                        {
                            var tz = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
                            var utc = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                            var local = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
                            e.Value = local.ToString("g");
                            e.FormattingApplied = true;
                        }
                        catch
                        {
                            e.Value = dt.ToLocalTime().ToString("g");
                            e.FormattingApplied = true;
                        }
                    }
                }
            }
            catch { }
        }

        private void BtnVerDetalles_Click(object? sender, EventArgs e)
        {
            if (dgvCompras.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una compra.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idObj = dgvCompras.SelectedRows[0].Cells["IdCompra"].Value;
            if (idObj == null) return;
            var id = Convert.ToInt32(idObj);
            var detailsForm = new CompraDetallesForm(id);
            detailsForm.ShowDialog(this);
        }
    }
}
