using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Registro_Productos
{
    public class InformesForm : Form
    {
        private TabControl tabs;
        private DataGridView dgvVentas;
        private DataGridView dgvCompras;

        private DateTimePicker dtpInicioVentas;
        private DateTimePicker dtpFinVentas;
        private Button btnNuevoVenta, btnEditarVenta, btnEliminarVenta, btnGenerarVenta, btnRefrescarVentas;

        private DateTimePicker dtpInicioCompras;
        private DateTimePicker dtpFinCompras;
        private Button btnNuevoCompra, btnEditarCompra, btnEliminarCompra, btnGenerarCompra, btnRefrescarCompras;

        public InformesForm()
        {
            InitializeComponent();
            LoadVentas();
            LoadCompras();
        }

        private bool ValidatePeriodDates(DateTime? inicio, DateTime? fin)
        {
            var now = DateTime.Now;
            if (inicio.HasValue && inicio.Value.Date > now.Date)
            {
                SweetAlert.ShowError(this, "Fecha inválida", $"La fecha de inicio no puede ser en el futuro. Fecha actual: {now:d}.");
                return false;
            }
            if (fin.HasValue && fin.Value > now)
            {
                SweetAlert.ShowError(this, "Fecha inválida", $"La fecha final no puede ser en el futuro. Fecha actual: {now:d} {now:T}.");
                return false;
            }
            if (inicio.HasValue && fin.HasValue && inicio.Value > fin.Value)
            {
                SweetAlert.ShowError(this, "Fechas inválidas", $"La fecha de inicio debe ser anterior o igual a la fecha final. Fecha actual: {now:d}.");
                return false;
            }
            return true;
        }

        private bool AnyDateInFuture(DateTime? inicio, DateTime? fin)
        {
            var now = DateTime.Now;
            if (inicio.HasValue && inicio.Value.Date > now.Date) return true;
            if (fin.HasValue && fin.Value > now) return true;
            return false;
        }

        private void UpdateGenerateButtons()
        {
            try
            {
                var invVentas = AnyDateInFuture(dtpInicioVentas?.Value.Date, dtpFinVentas?.Value.Date.AddDays(1).AddTicks(-1));
                btnGenerarVenta.Enabled = !invVentas;
                var invCompras = AnyDateInFuture(dtpInicioCompras?.Value.Date, dtpFinCompras?.Value.Date.AddDays(1).AddTicks(-1));
                btnGenerarCompra.Enabled = !invCompras;
            }
            catch { }
        }

        private void ExportTableToCsv(DataTable table, string baseName)
        {
            try
            {
                using var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = baseName + "_report.csv" };
                if (sfd.ShowDialog() != DialogResult.OK) return;
                using var sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8);
                // headers
                var headers = string.Join(";", table.Columns.Cast<DataColumn>().Select(c => EscapeCsv(c.ColumnName)));
                sw.WriteLine(headers);
                foreach (DataRow r in table.Rows)
                {
                    var vals = table.Columns.Cast<DataColumn>().Select(c => EscapeCsv(r[c] == DBNull.Value ? string.Empty : r[c].ToString()));
                    sw.WriteLine(string.Join(";", vals));
                }
                MessageBox.Show("Exportado a CSV.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exportando: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportTableToExcelLikeCsv(DataTable table, string baseName)
        {
            try
            {
                using var sfd = new SaveFileDialog { Filter = "Excel 97-2003 (*.xls)|*.xls|Excel CSV (*.csv)|*.csv", FileName = baseName + "_report.xls" };
                if (sfd.ShowDialog() != DialogResult.OK) return;
                using var sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8);
                var headers = string.Join("\t", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                sw.WriteLine(headers);
                foreach (DataRow r in table.Rows)
                {
                    var vals = table.Columns.Cast<DataColumn>().Select(c => (r[c] == DBNull.Value ? string.Empty : r[c].ToString()));
                    sw.WriteLine(string.Join("\t", vals));
                }
                MessageBox.Show("Exportado como archivo compatible con Excel.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exportando: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDataTable(DataTable table, string title)
        {
            try
            {
                var pd = new System.Drawing.Printing.PrintDocument();
                pd.DefaultPageSettings.Landscape = true;
                var font = new Font("Consolas", 9);
                var headerFont = new Font("Consolas", 9, FontStyle.Bold);
                int rowIndex = 0;
                float cellPadding = 4f;

                pd.PrintPage += (s, e) =>
                {
                    var g = e.Graphics;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    var rect = e.MarginBounds;

                    int y = rect.Top;
                    // title
                    g.DrawString(title, new Font("Segoe UI", 12, FontStyle.Bold), Brushes.Black, rect.Left, y);
                    y += 28;

                    int colCount = table.Columns.Count;
                    if (colCount == 0)
                    {
                        e.HasMorePages = false;
                        return;
                    }

                    // compute column width to fit page
                    float availableWidth = rect.Width;
                    float colWidth = availableWidth / colCount;

                    // draw header background
                    float x = rect.Left;
                    float headerHeight = headerFont.Height + cellPadding * 2;
                    var headerBackBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
                    g.FillRectangle(headerBackBrush, rect.Left, y, rect.Width, headerHeight);

                    // headers and vertical separators
                    for (int ci = 0; ci < colCount; ci++)
                    {
                        var header = table.Columns[ci].ColumnName;
                        var headerRect = new RectangleF(x + cellPadding, y + cellPadding, colWidth - cellPadding * 2, headerFont.Height + 2);
                        g.DrawString(header, headerFont, Brushes.Black, headerRect);
                        // vertical line
                        g.DrawLine(Pens.Gray, x + colWidth, y, x + colWidth, rect.Bottom);
                        x += colWidth;
                    }

                    // horizontal line under header
                    g.DrawLine(Pens.Black, rect.Left, y + headerHeight, rect.Right, y + headerHeight);
                    y += (int)headerHeight;

                    // rows with grid lines
                    while (rowIndex < table.Rows.Count && y < rect.Bottom - 20)
                    {
                        x = rect.Left;
                        var r = table.Rows[rowIndex];
                        float rowHeight = font.Height + cellPadding * 2;
                        for (int ci = 0; ci < colCount; ci++)
                        {
                            var vobj = r[ci];
                            var v = vobj == DBNull.Value ? string.Empty : vobj.ToString();
                            var cellRect = new RectangleF(x + cellPadding, y + cellPadding, colWidth - cellPadding * 2, font.Height + 2);
                            g.DrawString(v, font, Brushes.Black, cellRect);
                            // vertical separator already drawn for column end
                            x += colWidth;
                        }
                        // horizontal separator
                        g.DrawLine(Pens.LightGray, rect.Left, y + rowHeight, rect.Right, y + rowHeight);
                        y += (int)rowHeight;
                        rowIndex++;
                    }

                    // footer/page number
                    var footer = $"Página { (e.PageSettings.PrinterSettings.ToPage > 0 ? e.PageSettings.PrinterSettings.ToPage.ToString() : "") }";
                    g.DrawString(footer, font, Brushes.Gray, rect.Right - 100, rect.Bottom + 10);

                    e.HasMorePages = rowIndex < table.Rows.Count;
                };

                using var dlg = new PrintPreviewDialog { Document = pd, Width = 900, Height = 600 };
                dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error imprimiendo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Informes";
            this.Width = 900;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterParent;

            // Tema de colores
            var primaryColor = Color.FromArgb(52, 152, 219); // azul principal
            var primaryDark = Color.FromArgb(41, 128, 185);
            var headerBack = Color.FromArgb(245, 247, 250);
            this.BackColor = Color.WhiteSmoke;

            tabs = new TabControl { Dock = DockStyle.Fill, BackColor = Color.White };

            var tabVentas = new TabPage("Ventas");
            var tabCompras = new TabPage("Compras");

            // Ventas layout (mejorado)
            var panelVentasTop = new TableLayoutPanel { Dock = DockStyle.Top, Height = 48, ColumnCount = 2, RowCount = 1, Padding = new Padding(6) };
            panelVentasTop.BackColor = Color.Transparent;
            panelVentasTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelVentasTop.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            dtpInicioVentas = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110 };
            dtpFinVentas = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110 };
            dtpInicioVentas.ValueChanged += (s, e) => UpdateGenerateButtons();
            dtpFinVentas.ValueChanged += (s, e) => UpdateGenerateButtons();
            btnGenerarVenta = new Button { Text = "Generar", AutoSize = true, Margin = new Padding(8, 6, 8, 6), BackColor = primaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefrescarVentas = new Button { Text = "Refrescar", AutoSize = true, Margin = new Padding(0, 6, 8, 6), BackColor = primaryDark, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnNuevoVenta = new Button { Text = "Nuevo", AutoSize = true, Margin = new Padding(6), BackColor = primaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEditarVenta = new Button { Text = "Editar", AutoSize = true, Margin = new Padding(6), BackColor = primaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEliminarVenta = new Button { Text = "Eliminar", AutoSize = true, Margin = new Padding(6), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            // quitar borde duro
            btnGenerarVenta.FlatAppearance.BorderSize = 0;
            btnRefrescarVentas.FlatAppearance.BorderSize = 0;
            btnNuevoVenta.FlatAppearance.BorderSize = 0;
            btnEditarVenta.FlatAppearance.BorderSize = 0;
            btnEliminarVenta.FlatAppearance.BorderSize = 0;

            btnGenerarVenta.Click += (s, e) => GenerarInformeVenta();
            btnRefrescarVentas.Click += (s, e) => LoadVentas();

            var flowFiltrosVentas = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, BackColor = Color.Transparent };
            flowFiltrosVentas.Controls.Add(new Label { Text = "Desde:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(6, 10, 3, 3) });
            flowFiltrosVentas.Controls.Add(dtpInicioVentas);
            flowFiltrosVentas.Controls.Add(new Label { Text = "Hasta:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(12, 10, 3, 3) });
            flowFiltrosVentas.Controls.Add(dtpFinVentas);
            flowFiltrosVentas.Controls.Add(btnGenerarVenta);
            flowFiltrosVentas.Controls.Add(btnRefrescarVentas);

            var flowAccionesVentas = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, BackColor = Color.Transparent };
            flowAccionesVentas.Controls.Add(btnNuevoVenta);
            flowAccionesVentas.Controls.Add(btnEditarVenta);
            flowAccionesVentas.Controls.Add(btnEliminarVenta);

            panelVentasTop.Controls.Add(flowFiltrosVentas, 0, 0);
            panelVentasTop.Controls.Add(flowAccionesVentas, 1, 0);

            dgvVentas = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false, BackgroundColor = Color.White };
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "Id", DataPropertyName = "Id", Width = 60 });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Fecha", HeaderText = "Generado", DataPropertyName = "FechaGeneracion", Width = 160 });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Periodo", HeaderText = "Periodo", DataPropertyName = "Periodo", Width = 220 });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total Ventas", DataPropertyName = "TotalVentas", Width = 140 });
            // acciones: Ver, CSV, Excel, PDF
            dgvVentas.Columns.Add(new DataGridViewButtonColumn { Name = "Ver", HeaderText = "Ver", Text = "Ver", UseColumnTextForButtonValue = true, Width = 60 });
            dgvVentas.Columns.Add(new DataGridViewButtonColumn { Name = "CSV", HeaderText = "CSV", Text = "CSV", UseColumnTextForButtonValue = true, Width = 60 });
            dgvVentas.Columns.Add(new DataGridViewButtonColumn { Name = "XLS", HeaderText = "Excel", Text = "Excel", UseColumnTextForButtonValue = true, Width = 60 });
            dgvVentas.Columns.Add(new DataGridViewButtonColumn { Name = "PDF", HeaderText = "PDF", Text = "PDF", UseColumnTextForButtonValue = true, Width = 60 });

            btnNuevoVenta.Click += (s, e) => NuevoInformeVenta();
            btnEditarVenta.Click += (s, e) => EditarInformeVenta();
            btnEliminarVenta.Click += (s, e) => EliminarInformeVenta();

            // estilo DataGridView ventas
            dgvVentas.EnableHeadersVisualStyles = false;
            dgvVentas.ColumnHeadersDefaultCellStyle.BackColor = primaryColor;
            dgvVentas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvVentas.ColumnHeadersDefaultCellStyle.Font = new Font(dgvVentas.Font, FontStyle.Bold);
            dgvVentas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 245, 255);
            dgvVentas.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvVentas.RowHeadersVisible = false;

            tabVentas.Controls.Add(dgvVentas);
            tabVentas.Controls.Add(panelVentasTop);
            dgvVentas.CellClick += DgvVentas_CellClick;
            // doble clic: marcar en rojo y confirmar eliminación similar a Productos
            dgvVentas.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                try { dgvVentas.ClearSelection(); dgvVentas.Rows[e.RowIndex].Selected = true; } catch { }
                EliminarInformeVenta();
            };

            // Compras layout (mejorado)
            var panelComprasTop = new TableLayoutPanel { Dock = DockStyle.Top, Height = 48, ColumnCount = 2, RowCount = 1, Padding = new Padding(6) };
            panelComprasTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelComprasTop.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            dtpInicioCompras = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110 };
            dtpFinCompras = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110 };
            dtpInicioCompras.ValueChanged += (s, e) => UpdateGenerateButtons();
            dtpFinCompras.ValueChanged += (s, e) => UpdateGenerateButtons();
            btnGenerarCompra = new Button { Text = "Generar", AutoSize = true, Margin = new Padding(8, 6, 8, 6), BackColor = primaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefrescarCompras = new Button { Text = "Refrescar", AutoSize = true, Margin = new Padding(0, 6, 8, 6), BackColor = primaryDark, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnNuevoCompra = new Button { Text = "Nuevo", AutoSize = true, Margin = new Padding(6), BackColor = primaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEditarCompra = new Button { Text = "Editar", AutoSize = true, Margin = new Padding(6), BackColor = primaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEliminarCompra = new Button { Text = "Eliminar", AutoSize = true, Margin = new Padding(6), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnGenerarCompra.FlatAppearance.BorderSize = 0;
            btnRefrescarCompras.FlatAppearance.BorderSize = 0;
            btnNuevoCompra.FlatAppearance.BorderSize = 0;
            btnEditarCompra.FlatAppearance.BorderSize = 0;
            btnEliminarCompra.FlatAppearance.BorderSize = 0;

            btnGenerarCompra.Click += (s, e) => GenerarInformeCompra();
            btnRefrescarCompras.Click += (s, e) => LoadCompras();

            var flowFiltrosCompras = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, BackColor = Color.Transparent };
            flowFiltrosCompras.Controls.Add(new Label { Text = "Desde:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(6, 10, 3, 3) });
            flowFiltrosCompras.Controls.Add(dtpInicioCompras);
            flowFiltrosCompras.Controls.Add(new Label { Text = "Hasta:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(12, 10, 3, 3) });
            flowFiltrosCompras.Controls.Add(dtpFinCompras);
            flowFiltrosCompras.Controls.Add(btnGenerarCompra);
            flowFiltrosCompras.Controls.Add(btnRefrescarCompras);

            var flowAccionesCompras = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, BackColor = Color.Transparent };
            flowAccionesCompras.Controls.Add(btnNuevoCompra);
            flowAccionesCompras.Controls.Add(btnEditarCompra);
            flowAccionesCompras.Controls.Add(btnEliminarCompra);

            panelComprasTop.Controls.Add(flowFiltrosCompras, 0, 0);
            panelComprasTop.Controls.Add(flowAccionesCompras, 1, 0);

            dgvCompras = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false, BackgroundColor = Color.White };
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "Id", DataPropertyName = "Id", Width = 60 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Fecha", HeaderText = "Generado", DataPropertyName = "FechaGeneracion", Width = 160 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Periodo", HeaderText = "Periodo", DataPropertyName = "Periodo", Width = 220 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total Compras", DataPropertyName = "TotalCompras", Width = 140 });
            dgvCompras.Columns.Add(new DataGridViewButtonColumn { Name = "Ver", HeaderText = "Ver", Text = "Ver", UseColumnTextForButtonValue = true, Width = 60 });
            dgvCompras.Columns.Add(new DataGridViewButtonColumn { Name = "CSV", HeaderText = "CSV", Text = "CSV", UseColumnTextForButtonValue = true, Width = 60 });
            dgvCompras.Columns.Add(new DataGridViewButtonColumn { Name = "XLS", HeaderText = "Excel", Text = "Excel", UseColumnTextForButtonValue = true, Width = 60 });
            dgvCompras.Columns.Add(new DataGridViewButtonColumn { Name = "PDF", HeaderText = "PDF", Text = "PDF", UseColumnTextForButtonValue = true, Width = 60 });

            btnNuevoCompra.Click += (s, e) => NuevoInformeCompra();
            btnEditarCompra.Click += (s, e) => EditarInformeCompra();
            btnEliminarCompra.Click += (s, e) => EliminarInformeCompra();

            // estilo DataGridView compras
            dgvCompras.EnableHeadersVisualStyles = false;
            dgvCompras.ColumnHeadersDefaultCellStyle.BackColor = primaryColor;
            dgvCompras.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCompras.ColumnHeadersDefaultCellStyle.Font = new Font(dgvCompras.Font, FontStyle.Bold);
            dgvCompras.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 245, 255);
            dgvCompras.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvCompras.RowHeadersVisible = false;

            tabCompras.Controls.Add(dgvCompras);
            tabCompras.Controls.Add(panelComprasTop);
            dgvCompras.CellClick += DgvCompras_CellClick;
            // doble clic: marcar en rojo y confirmar eliminación similar a Productos
            dgvCompras.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                try { dgvCompras.ClearSelection(); dgvCompras.Rows[e.RowIndex].Selected = true; } catch { }
                EliminarInformeCompra();
            };

            tabs.TabPages.Add(tabVentas);
            tabs.TabPages.Add(tabCompras);

            this.Controls.Add(tabs);
        }

        private void LoadVentas()
        {
            var dt = Database.GetAllReportVentas();
            // añadir columna virtual Periodo
            if (!dt.Columns.Contains("Periodo")) dt.Columns.Add("Periodo");
            foreach (DataRow r in dt.Rows)
            {
                var inicio = r["PeriodoInicio"] as DateTime?;
                var fin = r["PeriodoFin"] as DateTime?;
                r["Periodo"] = inicio.HasValue && fin.HasValue ? $"{inicio.Value:d} - {fin.Value:d}" : "-";
            }
            // asignar resultados directamente; si no hay filas, la cuadrícula quedará vacía
            dgvVentas.DataSource = dt;
        }

        private void LoadCompras()
        {
            var dt = Database.GetAllReportCompras();
            if (!dt.Columns.Contains("Periodo")) dt.Columns.Add("Periodo");
            foreach (DataRow r in dt.Rows)
            {
                var inicio = r["PeriodoInicio"] as DateTime?;
                var fin = r["PeriodoFin"] as DateTime?;
                r["Periodo"] = inicio.HasValue && fin.HasValue ? $"{inicio.Value:d} - {fin.Value:d}" : "-";
            }
            dgvCompras.DataSource = dt;
        }

        private void DgvVentas_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var col = dgvVentas.Columns[e.ColumnIndex].Name;
            var id = Convert.ToInt32(dgvVentas.Rows[e.RowIndex].Cells["Id"].Value);
            if (col == "Ver")
            {
                var row = Database.GetReportVentaById(id);
                string json;
                if (row != null)
                {
                    json = row["ReportData"] == DBNull.Value ? "{}" : row["ReportData"].ToString();
                }
                else
                {
                    if (dgvVentas.Rows[e.RowIndex].DataBoundItem is DataRowView drv && drv.Row.Table.Columns.Contains("ReportData"))
                        json = drv.Row["ReportData"] == DBNull.Value ? "{}" : drv.Row["ReportData"].ToString();
                    else
                        json = "{}";
                }
                ShowReportDialog("Informe de ventas", json);
            }
            else if (col == "CSV")
            {
                // exportar tabla tabular de ventas (master + detalles)
                DateTime? inicio = null, fin = null;
                var row = Database.GetReportVentaById(id);
                if (row != null)
                {
                    inicio = row["PeriodoInicio"] == DBNull.Value ? null : (DateTime?)row["PeriodoInicio"];
                    fin = row["PeriodoFin"] == DBNull.Value ? null : (DateTime?)row["PeriodoFin"];
                }
                else
                {
                    inicio = dtpInicioVentas.Value.Date;
                    fin = dtpFinVentas.Value.Date.AddDays(1).AddTicks(-1);
                }
                var table = Database.GetVentasReport(inicio, fin);
                ExportTableToCsv(table, "ventas_report");
            }
            else if (col == "XLS")
            {
                DateTime? inicio = null, fin = null;
                var row = Database.GetReportVentaById(id);
                if (row != null)
                {
                    inicio = row["PeriodoInicio"] == DBNull.Value ? null : (DateTime?)row["PeriodoInicio"];
                    fin = row["PeriodoFin"] == DBNull.Value ? null : (DateTime?)row["PeriodoFin"];
                }
                else
                {
                    inicio = dtpInicioVentas.Value.Date;
                    fin = dtpFinVentas.Value.Date.AddDays(1).AddTicks(-1);
                }
                var table = Database.GetVentasReport(inicio, fin);
                ExportTableToExcelLikeCsv(table, "ventas_report");
            }
            else if (col == "PDF")
            {
                DateTime? inicio = null, fin = null;
                var row = Database.GetReportVentaById(id);
                if (row != null)
                {
                    inicio = row["PeriodoInicio"] == DBNull.Value ? null : (DateTime?)row["PeriodoInicio"];
                    fin = row["PeriodoFin"] == DBNull.Value ? null : (DateTime?)row["PeriodoFin"];
                }
                else
                {
                    inicio = dtpInicioVentas.Value.Date;
                    fin = dtpFinVentas.Value.Date.AddDays(1).AddTicks(-1);
                }
                var table = Database.GetVentasReport(inicio, fin);
                PrintDataTable(table, "Informe de ventas");
            }
        }

        private void DgvCompras_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var col = dgvCompras.Columns[e.ColumnIndex].Name;
            var id = Convert.ToInt32(dgvCompras.Rows[e.RowIndex].Cells["Id"].Value);
            if (col == "Ver")
            {
                var row = Database.GetReportCompraById(id);
                string json;
                if (row != null)
                {
                    json = row["ReportData"] == DBNull.Value ? "{}" : row["ReportData"].ToString();
                }
                else
                {
                    if (dgvCompras.Rows[e.RowIndex].DataBoundItem is DataRowView drv && drv.Row.Table.Columns.Contains("ReportData"))
                        json = drv.Row["ReportData"] == DBNull.Value ? "{}" : drv.Row["ReportData"].ToString();
                    else
                        json = "{}";
                }
                ShowReportDialog("Informe de compras", json);
            }
            else if (col == "CSV")
            {
                DateTime? inicio = null, fin = null;
                var row = Database.GetReportCompraById(id);
                if (row != null)
                {
                    inicio = row["PeriodoInicio"] == DBNull.Value ? null : (DateTime?)row["PeriodoInicio"];
                    fin = row["PeriodoFin"] == DBNull.Value ? null : (DateTime?)row["PeriodoFin"];
                }
                else
                {
                    inicio = dtpInicioCompras.Value.Date;
                    fin = dtpFinCompras.Value.Date.AddDays(1).AddTicks(-1);
                }
                var table = Database.GetComprasReport(inicio, fin);
                ExportTableToCsv(table, "compras_report");
            }
            else if (col == "XLS")
            {
                DateTime? inicio = null, fin = null;
                var row = Database.GetReportCompraById(id);
                if (row != null)
                {
                    inicio = row["PeriodoInicio"] == DBNull.Value ? null : (DateTime?)row["PeriodoInicio"];
                    fin = row["PeriodoFin"] == DBNull.Value ? null : (DateTime?)row["PeriodoFin"];
                }
                else
                {
                    inicio = dtpInicioCompras.Value.Date;
                    fin = dtpFinCompras.Value.Date.AddDays(1).AddTicks(-1);
                }
                var table = Database.GetComprasReport(inicio, fin);
                ExportTableToExcelLikeCsv(table, "compras_report");
            }
            else if (col == "PDF")
            {
                DateTime? inicio = null, fin = null;
                var row = Database.GetReportCompraById(id);
                if (row != null)
                {
                    inicio = row["PeriodoInicio"] == DBNull.Value ? null : (DateTime?)row["PeriodoInicio"];
                    fin = row["PeriodoFin"] == DBNull.Value ? null : (DateTime?)row["PeriodoFin"];
                }
                else
                {
                    inicio = dtpInicioCompras.Value.Date;
                    fin = dtpFinCompras.Value.Date.AddDays(1).AddTicks(-1);
                }
                var table = Database.GetComprasReport(inicio, fin);
                PrintDataTable(table, "Informe de compras");
            }
        }

        private void NuevoInformeVenta()
        {
            using var dlg = new ReportEditForm("Venta");
            if (dlg.ShowDialog() != DialogResult.OK) return;
            // validar periodo (ReportEditForm ya valida, pero por seguridad)
            if (!ValidatePeriodDates(dlg.PeriodoInicio, dlg.PeriodoFin)) return;
            // generar agregado
            var agg = Database.GetSalesAggregate(dlg.PeriodoInicio, dlg.PeriodoFin);
            var id = Database.InsertReportVenta(DateTime.Now, dlg.PeriodoInicio, dlg.PeriodoFin, agg.total, agg.items, dlg.ReportData);
            LoadVentas();
            MessageBox.Show($"Informe de ventas creado (Id {id}).", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditarInformeVenta()
        {
            if (dgvVentas.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(dgvVentas.SelectedRows[0].Cells["Id"].Value);
            var row = Database.GetReportVentaById(id);
            if (row == null) return;
            DateTime? inicio = row["PeriodoInicio"] == DBNull.Value ? null : (DateTime?)row["PeriodoInicio"];
            DateTime? fin = row["PeriodoFin"] == DBNull.Value ? null : (DateTime?)row["PeriodoFin"];
            string data = row["ReportData"] == DBNull.Value ? string.Empty : row["ReportData"].ToString();
            using var dlg = new ReportEditForm("Venta") { PeriodoInicio = inicio, PeriodoFin = fin, ReportData = data };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            if (!ValidatePeriodDates(dlg.PeriodoInicio, dlg.PeriodoFin)) return;
            var agg = Database.GetSalesAggregate(dlg.PeriodoInicio, dlg.PeriodoFin);
            Database.UpdateReportVenta(id, DateTime.Now, dlg.PeriodoInicio, dlg.PeriodoFin, agg.total, agg.items, dlg.ReportData);
            LoadVentas();
        }

        private void EliminarInformeVenta()
        {
            if (dgvVentas.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(dgvVentas.SelectedRows[0].Cells["Id"].Value);
            if (MessageBox.Show($"Eliminar informe de ventas #{id}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Database.DeleteReportVenta(id);
                LoadVentas();
            }
        }

        private void NuevoInformeCompra()
        {
            using var dlg = new ReportEditForm("Compra");
            if (dlg.ShowDialog() != DialogResult.OK) return;
            if (!ValidatePeriodDates(dlg.PeriodoInicio, dlg.PeriodoFin)) return;
            var agg = Database.GetPurchasesAggregate(dlg.PeriodoInicio, dlg.PeriodoFin);
            var id = Database.InsertReportCompra(DateTime.Now, dlg.PeriodoInicio, dlg.PeriodoFin, agg.total, agg.items, dlg.ReportData);
            LoadCompras();
            MessageBox.Show($"Informe de compras creado (Id {id}).", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditarInformeCompra()
        {
            if (dgvCompras.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(dgvCompras.SelectedRows[0].Cells["Id"].Value);
            if (id <= 0)
            {
                MessageBox.Show("No se puede editar: el informe no está guardado en la base de datos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var row = Database.GetReportCompraById(id);
            if (row == null) return;
            DateTime? inicio = row["PeriodoInicio"] == DBNull.Value ? null : (DateTime?)row["PeriodoInicio"];
            DateTime? fin = row["PeriodoFin"] == DBNull.Value ? null : (DateTime?)row["PeriodoFin"];
            string data = row["ReportData"] == DBNull.Value ? string.Empty : row["ReportData"].ToString();
            using var dlg = new ReportEditForm("Compra") { PeriodoInicio = inicio, PeriodoFin = fin, ReportData = data };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            if (!ValidatePeriodDates(dlg.PeriodoInicio, dlg.PeriodoFin)) return;
            var agg = Database.GetPurchasesAggregate(dlg.PeriodoInicio, dlg.PeriodoFin);
            Database.UpdateReportCompra(id, DateTime.Now, dlg.PeriodoInicio, dlg.PeriodoFin, agg.total, agg.items, dlg.ReportData);
            LoadCompras();
        }

        private void EliminarInformeCompra()
        {
            if (dgvCompras.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(dgvCompras.SelectedRows[0].Cells["Id"].Value);
            if (id <= 0)
            {
                MessageBox.Show("No se puede eliminar: el informe no está guardado en la base de datos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show($"Eliminar informe de compras #{id}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var rows = Database.DeleteReportCompra(id);
                if (rows > 0)
                {
                    MessageBox.Show("Informe eliminado.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se encontró el informe en la base de datos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                LoadCompras();
            }
        }

        private void GenerarInformeVenta()
        {
            var inicio = dtpInicioVentas.Value.Date;
            var fin = dtpFinVentas.Value.Date.AddDays(1).AddTicks(-1);
            if (!ValidatePeriodDates(inicio, fin)) return;
            var agg = Database.GetSalesAggregate(inicio, fin);
            var json = $"{{ \"Total\": {agg.total}, \"Items\": {agg.items}, \"Inicio\": \"{inicio:o}\", \"Fin\": \"{fin:o}\" }}";
            var id = Database.InsertReportVenta(DateTime.Now, inicio, fin, agg.total, agg.items, json);
            LoadVentas();
            MessageBox.Show($"Informe de ventas generado y guardado (Id {id}).", "Generado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GenerarInformeCompra()
        {
            var inicio = dtpInicioCompras.Value.Date;
            var fin = dtpFinCompras.Value.Date.AddDays(1).AddTicks(-1);
            if (!ValidatePeriodDates(inicio, fin)) return;
            var agg = Database.GetPurchasesAggregate(inicio, fin);
            var json = $"{{ \"Total\": {agg.total}, \"Items\": {agg.items}, \"Inicio\": \"{inicio:o}\", \"Fin\": \"{fin:o}\" }}";
            var id = Database.InsertReportCompra(DateTime.Now, inicio, fin, agg.total, agg.items, json);
            LoadCompras();
            MessageBox.Show($"Informe de compras generado y guardado (Id {id}).", "Generado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowReportDialog(string title, string json)
        {
            using var dlg = new Form { Text = title, Width = 700, Height = 500, StartPosition = FormStartPosition.CenterParent };
            var txt = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, Dock = DockStyle.Fill, Font = new Font("Consolas", 10) };
            try
            {
                var doc = System.Text.Json.JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
                var opts = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                txt.Text = System.Text.Json.JsonSerializer.Serialize(doc.RootElement, opts);
            }
            catch
            {
                txt.Text = json ?? string.Empty;
            }

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40, FlowDirection = FlowDirection.RightToLeft };
            var btnClose = new Button { Text = "Cerrar", DialogResult = DialogResult.Cancel, AutoSize = true };
            var btnPdf = new Button { Text = "Imprimir/PDF", AutoSize = true };
            var btnXls = new Button { Text = "Exportar Excel", AutoSize = true };
            var btnCsv = new Button { Text = "Exportar CSV", AutoSize = true };

            btnCsv.Click += (s, e) => ExportJsonToCsv(txt.Text, title.ToLower().Contains("venta") ? "ventas" : "compras");
            btnXls.Click += (s, e) => ExportJsonToExcelLikeCsv(txt.Text, title.ToLower().Contains("venta") ? "ventas" : "compras");
            btnPdf.Click += (s, e) => PrintJson(txt.Text);

            btnPanel.Controls.Add(btnClose);
            btnPanel.Controls.Add(btnPdf);
            btnPanel.Controls.Add(btnXls);
            btnPanel.Controls.Add(btnCsv);

            dlg.Controls.Add(txt);
            dlg.Controls.Add(btnPanel);
            dlg.AcceptButton = btnClose;
            dlg.ShowDialog();
        }

        private void ExportJsonToCsv(string json, string baseName)
        {
            try
            {
                using var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = baseName + "_report.csv" };
                if (sfd.ShowDialog() != DialogResult.OK) return;
                using var sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8);
                try
                {
                    var doc = System.Text.Json.JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
                    if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        sw.WriteLine("Key;Value");
                        foreach (var p in doc.RootElement.EnumerateObject())
                        {
                            var val = p.Value.ValueKind == System.Text.Json.JsonValueKind.Object || p.Value.ValueKind == System.Text.Json.JsonValueKind.Array ? p.Value.ToString().Replace("\r\n", " ") : p.Value.ToString();
                            sw.WriteLine($"{EscapeCsv(p.Name)};{EscapeCsv(val)}");
                        }
                    }
                    else if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        // write rows from array flattening first-level properties
                        var first = true;
                        foreach (var el in doc.RootElement.EnumerateArray())
                        {
                            if (el.ValueKind != System.Text.Json.JsonValueKind.Object) continue;
                            if (first)
                            {
                                var headers = string.Join(";", el.EnumerateObject().Select(p => EscapeCsv(p.Name)));
                                sw.WriteLine(headers);
                                first = false;
                            }
                            var row = string.Join(";", el.EnumerateObject().Select(p => EscapeCsv(p.Value.ToString())));
                            sw.WriteLine(row);
                        }
                    }
                    else
                    {
                        sw.WriteLine(EscapeCsv(json));
                    }
                }
                catch
                {
                    // fallback: write raw JSON
                    sw.WriteLine(json ?? string.Empty);
                }
                MessageBox.Show("Exportado a CSV.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exportando: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportJsonToExcelLikeCsv(string json, string baseName)
        {
            try
            {
                using var sfd = new SaveFileDialog { Filter = "Excel 97-2003 (*.xls)|*.xls|Excel CSV (*.csv)|*.csv", FileName = baseName + "_report.xls" };
                if (sfd.ShowDialog() != DialogResult.OK) return;
                // reuse CSV exporter but save with chosen filename
                using var sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8);
                try
                {
                    var doc = System.Text.Json.JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
                    if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        sw.WriteLine("Key\tValue");
                        foreach (var p in doc.RootElement.EnumerateObject())
                        {
                            var val = p.Value.ValueKind == System.Text.Json.JsonValueKind.Object || p.Value.ValueKind == System.Text.Json.JsonValueKind.Array ? p.Value.ToString().Replace("\r\n", " ") : p.Value.ToString();
                            sw.WriteLine($"{p.Name}\t{val}");
                        }
                    }
                    else if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        var first = true;
                        foreach (var el in doc.RootElement.EnumerateArray())
                        {
                            if (el.ValueKind != System.Text.Json.JsonValueKind.Object) continue;
                            if (first)
                            {
                                var headers = string.Join("\t", el.EnumerateObject().Select(p => p.Name));
                                sw.WriteLine(headers);
                                first = false;
                            }
                            var row = string.Join("\t", el.EnumerateObject().Select(p => p.Value.ToString()));
                            sw.WriteLine(row);
                        }
                    }
                    else
                    {
                        sw.WriteLine(json ?? string.Empty);
                    }
                }
                catch
                {
                    sw.WriteLine(json ?? string.Empty);
                }
                MessageBox.Show("Exportado como archivo compatible con Excel.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exportando: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string EscapeCsv(string v)
        {
            if (v == null) return string.Empty;
            if (v.Contains(";") || v.Contains("\n") || v.Contains("\r") || v.Contains('"'))
            {
                return '"' + v.Replace("\"", "\"\"") + '"';
            }
            return v;
        }

        private void PrintJson(string json)
        {
            try
            {
                var pd = new System.Drawing.Printing.PrintDocument();
                var text = json ?? string.Empty;
                var font = new Font("Consolas", 10);
                pd.PrintPage += (s, e) =>
                {
                    var rect = e.MarginBounds;
                    e.Graphics.DrawString(text, font, Brushes.Black, rect);
                };
                using var dlg = new PrintPreviewDialog { Document = pd, Width = 800, Height = 600 };
                dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error imprimiendo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
