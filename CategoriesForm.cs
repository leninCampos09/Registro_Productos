using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Registro_Productos
{
    public class CategoriesForm : Form
    {
        private DataGridView dgv;
        private Button btnNuevo;
        private Button btnRefresh;
        private Label lblSearch;
        private TextBox txtSearch;

        public CategoriesForm()
        {
            this.Text = "Categorías";
            this.Size = new Size(600, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponents();
            LoadCategories();
            // quitar selección inicial cuando el formulario se muestre
            this.Shown += (s, e) => { try { dgv.ClearSelection(); if (dgv.CurrentCell != null) dgv.CurrentCell = null; } catch { } };
        }

        private void InitializeComponents()
        {
            dgv = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            btnNuevo = new Button { Text = "Nuevo", Location = new Point(10, 10), Size = new Size(100, 30), FlatStyle = FlatStyle.System };
            btnRefresh = new Button { Text = "Actualizar", Location = new Point(120, 10), Size = new Size(100, 30), FlatStyle = FlatStyle.System };
            lblSearch = new Label { Text = "Buscar:", AutoSize = true, Location = new Point(240, 15) };
            txtSearch = new TextBox { Location = new Point(290, 12), Size = new Size(180, 23) };

            btnNuevo.Click += BtnNuevo_Click;
            btnRefresh.Click += BtnRefresh_Click;
            dgv.CellContentClick += Dgv_CellContentClick;

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            pnlTop.Controls.Add(btnNuevo);
            pnlTop.Controls.Add(btnRefresh);
            pnlTop.Controls.Add(lblSearch);
            pnlTop.Controls.Add(txtSearch);

            this.Controls.Add(dgv);
            this.Controls.Add(pnlTop);
            // suscribir búsqueda para recargar
            txtSearch.TextChanged += (s, e) => LoadCategories();
        }

        private void LoadCategories()
        {
            var dt = Database.GetAllCategories();
            if (dt == null) dt = new DataTable();

            // aplicar filtro de búsqueda si existe
            try
            {
                if (this.txtSearch != null)
                {
                    var q = this.txtSearch.Text?.Trim();
                    if (!string.IsNullOrEmpty(q))
                    {
                        q = q.Replace("'", "''");
                        var dv = dt.DefaultView;
                        dv.RowFilter = $"nombre LIKE '%{q}%'";
                        dt = dv.ToTable();
                    }
                }
            }
            catch { }

            dgv.DataSource = dt;

            // Mostrar nombres en mayúsculas para consistencia
            try
            {
                if (dt.Columns.Contains("nombre"))
                {
                    foreach (System.Data.DataRow r in dt.Rows)
                    {
                        if (r["nombre"] != DBNull.Value && r["nombre"] != null)
                        {
                            var s = r["nombre"].ToString();
                            if (!string.IsNullOrEmpty(s)) r["nombre"] = s.ToUpperInvariant();
                        }
                    }
                }
            }
            catch { }

            // Ajustes visuales similares a ProductosForm
            dgv.RowTemplate.Height = 60;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToResizeColumns = true;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 144, 255);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);

            // remove action cols if present
            foreach (var name in new[] { "Edit", "Delete" })
                if (dgv.Columns.Contains(name)) dgv.Columns.Remove(name);

            // helper para iconos pequeños con fondo redondeado
            System.Drawing.Bitmap CreateActionIconStyled(string action)
            {
                int size = 18;
                var bmp = new System.Drawing.Bitmap(size, size);
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(System.Drawing.Color.Transparent);

                    System.Drawing.Drawing2D.GraphicsPath RoundRect(System.Drawing.RectangleF r, float radius)
                    {
                        var p = new System.Drawing.Drawing2D.GraphicsPath();
                        float d = radius * 2f;
                        p.AddArc(r.X, r.Y, d, d, 180, 90);
                        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
                        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
                        p.CloseFigure();
                        return p;
                    }

                    if (action == "edit")
                    {
                        using var bg = new SolidBrush(Color.FromArgb(37, 211, 102));
                        var rect = new RectangleF(1.5f, 1.5f, size - 3, size - 3);
                        using var path = RoundRect(rect, 4f);
                        g.FillPath(bg, path);

                        using var bodyBrush = new SolidBrush(Color.White);
                        var body = new PointF[] { new PointF(5f, 12f), new PointF(11f, 6f), new PointF(13f, 8f), new PointF(7f, 14f) };
                        g.FillPolygon(bodyBrush, body);

                        using var tipBrush = new SolidBrush(Color.FromArgb(80, 80, 80));
                        var tip = new PointF[] { new PointF(11.5f, 5f), new PointF(13.5f, 7f), new PointF(11f, 8.5f) };
                        g.FillPolygon(tipBrush, tip);

                        using var penDetail = new Pen(Color.FromArgb(200, 200, 200), 1f);
                        g.DrawLine(penDetail, 6f, 13f, 12f, 7f);
                    }
                    else if (action == "delete")
                    {
                        using var bg = new SolidBrush(Color.FromArgb(220, 53, 69));
                        var rect = new RectangleF(1.5f, 1.5f, size - 3, size - 3);
                        using var path = RoundRect(rect, 4f);
                        g.FillPath(bg, path);

                        using var pen = new Pen(Color.White, 1.4f);
                        g.DrawLine(pen, 5, 5, 13, 5);
                        g.DrawRectangle(pen, 6, 6, 6, 8);
                        g.DrawLine(pen, 8, 8, 8, 12);
                        g.DrawLine(pen, 10, 8, 10, 12);
                    }
                    else
                    {
                        using var bg = new SolidBrush(Color.Gray);
                        var rect = new RectangleF(1.5f, 1.5f, size - 3, size - 3);
                        using var path = RoundRect(rect, 4f);
                        g.FillPath(bg, path);
                    }
                }
                return bmp;
            }

            var imgEdit = CreateActionIconStyled("edit");
            var imgDelete = CreateActionIconStyled("delete");

            var editColImg = new DataGridViewImageColumn() { Name = "Edit", HeaderText = "", Image = imgEdit, ImageLayout = DataGridViewImageCellLayout.Normal };
            var delColImg = new DataGridViewImageColumn() { Name = "Delete", HeaderText = "", Image = imgDelete, ImageLayout = DataGridViewImageCellLayout.Normal };

            editColImg.Width = 28; editColImg.ReadOnly = true; editColImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; editColImg.DefaultCellStyle.Padding = new Padding(2);
            delColImg.Width = 28; delColImg.ReadOnly = true; delColImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; delColImg.DefaultCellStyle.Padding = new Padding(2);

            dgv.Columns.Add(editColImg);
            dgv.Columns.Add(delColImg);

            // ocultar encabezados de fila (columna gris vacía a la izquierda)
            dgv.RowHeadersVisible = false;

            // eliminar columnas con header vacío o completamente vacías
            try
            {
                var cols = new System.Collections.Generic.List<DataGridViewColumn>();
                foreach (DataGridViewColumn c in dgv.Columns) cols.Add(c);
                foreach (var c in cols)
                {
                    if (c.Name == "Edit" || c.Name == "Delete") continue;
                    if (string.IsNullOrWhiteSpace(c.HeaderText)) { try { dgv.Columns.Remove(c); continue; } catch { } }
                    bool allEmpty = true;
                    foreach (DataGridViewRow r in dgv.Rows)
                    {
                        if (r.IsNewRow) continue;
                        var v = r.Cells[c.Index].Value;
                        if (v != null && v != DBNull.Value)
                        {
                            if (v is string s)
                            {
                                if (!string.IsNullOrWhiteSpace(s)) { allEmpty = false; break; }
                            }
                            else { allEmpty = false; break; }
                        }
                    }
                    if (allEmpty) { try { dgv.Columns.Remove(c); } catch { } }
                }
            }
            catch { }

            // evitar celda seleccionada
            try { dgv.ClearSelection(); if (dgv.CurrentCell != null) dgv.CurrentCell = null; } catch { }

            // Doble click en fila: preguntar si desea eliminar y marcar en rojo mientras confirma
            dgv.CellDoubleClick -= Dgv_CellDoubleClick;
            dgv.CellDoubleClick += Dgv_CellDoubleClick;
        }

        private void BtnRefresh_Click(object? sender, EventArgs e) => LoadCategories();

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            using var f = new CategoryEditForm();
            if (f.ShowDialog() == DialogResult.OK) LoadCategories();
        }

        private void Dgv_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = dgv.Columns[e.ColumnIndex].Name;
            var id = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["idCategoria"].Value);
            if (colName == "Edit")
            {
                var nombre = dgv.Rows[e.RowIndex].Cells["nombre"].Value.ToString();
                using var f = new CategoryEditForm(id, nombre);
                if (f.ShowDialog() == DialogResult.OK) LoadCategories();
            }
            else if (colName == "Delete")
            {
                var r = MessageBox.Show("¿Eliminar categoría?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    Database.DeleteCategory(id);
                    LoadCategories();
                }
            }
        }

        private void Dgv_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            // colorear fila en rojo como indicación (también cambiar SelectionBackColor para que sea visible
            // aunque la fila quede seleccionada)
            var prevColor = row.DefaultCellStyle.BackColor;
            var prevSelColor = row.DefaultCellStyle.SelectionBackColor;
            var prevForeColor = row.DefaultCellStyle.ForeColor;
            var prevSelForeColor = row.DefaultCellStyle.SelectionForeColor;
            try
            {
                // usar rojo puro y texto blanco para contraste
                row.DefaultCellStyle.BackColor = Color.Red;
                row.DefaultCellStyle.SelectionBackColor = Color.Red;
                row.DefaultCellStyle.ForeColor = Color.White;
                row.DefaultCellStyle.SelectionForeColor = Color.White;
                try { dgv.InvalidateRow(e.RowIndex); } catch { dgv.Refresh(); }
            }
            catch { }

            try
            {
                var id = Convert.ToInt32(row.Cells["idCategoria"].Value);
                var nombre = row.Cells["nombre"].Value?.ToString();
                var q = MessageBox.Show($"Eliminar categoría '{nombre}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (q == DialogResult.Yes)
                {
                    Database.DeleteCategory(id);
                    LoadCategories();
                }
                else
                {
                    // restaurar color original
                    try { row.DefaultCellStyle.BackColor = prevColor; row.DefaultCellStyle.SelectionBackColor = prevSelColor; row.DefaultCellStyle.ForeColor = prevForeColor; row.DefaultCellStyle.SelectionForeColor = prevSelForeColor; dgv.InvalidateRow(e.RowIndex); } catch { }
                }
            }
            catch (Exception ex)
            {
                try { row.DefaultCellStyle.BackColor = prevColor; row.DefaultCellStyle.SelectionBackColor = prevSelColor; row.DefaultCellStyle.ForeColor = prevForeColor; row.DefaultCellStyle.SelectionForeColor = prevSelForeColor; } catch { }
                MessageBox.Show("Error al procesar la acción: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
