using System;
using System.Data;
using System.Windows.Forms;

namespace Registro_Productos
{
    public partial class ProductosForm : Form
    {
        public ProductosForm()
        {
            InitializeComponent();
            this.Load += ProductosForm_Load;
            this.btnNuevo.Click += BtnNuevo_Click;
            this.btnRefresh.Click += BtnRefresh_Click;
            this.dgvProducts.CellContentClick += DgvProducts_CellContentClick;
            // eventos adicionales
            this.dgvProducts.CellDoubleClick += DgvProducts_CellDoubleClick;
            this.dgvProducts.CellMouseDown += DgvProducts_CellMouseDown;
            if (this.cboCategoriaFilter != null)
                this.cboCategoriaFilter.SelectedIndexChanged += (s, e) => LoadProducts();
            if (this.txtSearch != null)
                this.txtSearch.TextChanged += (s, e) => LoadProducts();
            // Quitar selección inicial una vez que el formulario ya se ha mostrado
            this.Shown += (s, e) =>
            {
                try { dgvProducts.ClearSelection(); if (dgvProducts.CurrentCell != null) dgvProducts.CurrentCell = null; } catch { }
            };
        }

        private void ProductosForm_Load(object? sender, EventArgs e)
        {
            LoadCategories();
            LoadProducts();
        }

        public void LoadCategories()
        {
            try
            {
                var dt = Database.GetAllCategories();
                if (dt == null) dt = new DataTable();
                if (!dt.Columns.Contains("idCategoria")) dt.Columns.Add("idCategoria", typeof(int));
                if (!dt.Columns.Contains("nombre")) dt.Columns.Add("nombre", typeof(string));
                var empty = dt.NewRow();
                empty["idCategoria"] = DBNull.Value;
                empty["nombre"] = "-- Todas las categorías --";
                dt.Rows.InsertAt(empty, 0);
                cboCategoriaFilter.DisplayMember = "nombre";
                cboCategoriaFilter.ValueMember = "idCategoria";
                cboCategoriaFilter.DataSource = dt;
                cboCategoriaFilter.SelectedIndex = 0;
            }
            catch { }
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            using var f = new frmPrincipal();
            f.ShowDialog();
            LoadProducts();
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            var dt = Database.GetAllProducts();

            // aplicar filtro por categoria y por búsqueda si corresponde
            try
            {
                var dv = dt.DefaultView;
                var filters = new System.Collections.Generic.List<string>();

                if (this.cboCategoriaFilter != null && this.cboCategoriaFilter.SelectedValue != null && !(this.cboCategoriaFilter.SelectedValue is DBNull))
                {
                    int catId = Convert.ToInt32(this.cboCategoriaFilter.SelectedValue);
                    filters.Add($"categoriaId = {catId}");
                }

                if (this.txtSearch != null)
                {
                    var q = this.txtSearch.Text?.Trim();
                    if (!string.IsNullOrEmpty(q))
                    {
                        // escapar comillas simples
                        q = q.Replace("'", "''");
                        filters.Add($"(producto LIKE '%{q}%' OR descripcion LIKE '%{q}%')");
                    }
                }

                if (filters.Count > 0)
                    dv.RowFilter = string.Join(" AND ", filters);

                dt = dv.ToTable();
            }
            catch { }

            // Crear una tabla intermedia para reemplazar la columna img por una columna de imagen
            var dt2 = new System.Data.DataTable();
            foreach (System.Data.DataColumn col in dt.Columns)
            {
                // omitimos columnas internas/relacionales que no deben mostrarse en la lista
                if (col.ColumnName == "img" || col.ColumnName == "categoriaId" || col.ColumnName == "categoria")
                    continue;
                // mostrar "Sí"/"No" en vez de checkbox para la columna disponible
                if (col.ColumnName == "disponible")
                {
                    dt2.Columns.Add("disponible", typeof(string));
                }
                else
                {
                    dt2.Columns.Add(col.ColumnName, col.DataType);
                }
            }
            // columna para la imagen en memoria
            dt2.Columns.Add("imgPreview", typeof(System.Drawing.Image));

            foreach (System.Data.DataRow r in dt.Rows)
            {
                var nr = dt2.NewRow();
                foreach (System.Data.DataColumn col in dt.Columns)
                {
                    // omitimos columnas que no agregamos a la vista
                    if (col.ColumnName == "img" || col.ColumnName == "categoriaId" || col.ColumnName == "categoria") continue;
                    if (col.ColumnName == "disponible")
                    {
                        // convertir a Sí/No
                        nr["disponible"] = Convert.ToBoolean(r["disponible"]) ? "Sí" : "No";
                    }
                    else
                    {
                        nr[col.ColumnName] = r[col.ColumnName];
                    }
                }

                // cargar imagen desde path guardado en la columna img
                var imgPathObj = r["img"];
                System.Drawing.Image? img = null;
                if (imgPathObj != DBNull.Value && imgPathObj != null)
                {
                    var imgPath = imgPathObj.ToString();
                    string fullPath = imgPath ?? string.Empty;
                    if (!string.IsNullOrEmpty(imgPath) && !System.IO.Path.IsPathRooted(imgPath))
                        fullPath = System.IO.Path.Combine(Application.StartupPath, imgPath);

                    try
                    {
                        if (System.IO.File.Exists(fullPath))
                        {
                            using var fs = new System.IO.FileStream(fullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                            var original = System.Drawing.Image.FromStream(fs);
                            // crear copia redimensionada para previsualización (max 64x64)
                            var thumb = new System.Drawing.Bitmap(64, 64);
                            using (var g = System.Drawing.Graphics.FromImage(thumb))
                            {
                                g.Clear(System.Drawing.Color.Transparent);
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                g.DrawImage(original, 0, 0, 64, 64);
                            }
                            img = new System.Drawing.Bitmap(thumb);
                            original.Dispose();
                        }
                    }
                    catch
                    {
                        img = null;
                    }
                }

                nr["imgPreview"] = img ?? (object)DBNull.Value;
                dt2.Rows.Add(nr);
            }

            // Ajustes en el DataGridView
            // reducir altura de filas para que los iconos y celdas se vean más compactos
            dgvProducts.RowTemplate.Height = 60;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProducts.AllowUserToResizeRows = false;
            dgvProducts.AllowUserToResizeColumns = true;
            // Desactivar fila de nuevo registro (fila vacía al final)
            dgvProducts.AllowUserToAddRows = false;
            // Mostrar los datos como de solo lectura (edición vía botones)
            dgvProducts.ReadOnly = true;
            dgvProducts.DataSource = dt2;
            // ocultar encabezados de fila (columna gris vacía a la izquierda)
            dgvProducts.RowHeadersVisible = false;

            // Formatear columna precio para mostrar signo de dólar
            if (dgvProducts.Columns.Contains("precio"))
            {
                var colPrecio = dgvProducts.Columns["precio"];
                colPrecio.DefaultCellStyle.Format = "$#,##0.00";
                colPrecio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            // Personalizar color del encabezado
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(30, 144, 255); // DodgerBlue
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(dgvProducts.Font, System.Drawing.FontStyle.Bold);

            // Remover columnas previas de botones si existen para evitar duplicados
            foreach (var name in new[] { "Edit", "Delete", "View" })
            {
                if (dgvProducts.Columns.Contains(name))
                    dgvProducts.Columns.Remove(name);
            }

            // Crear iconos con fondo pequeño y forma definida (estilo más pulido)
            System.Drawing.Bitmap CreateActionIconStyled(string action)
            {
                int size = 18;
                var bmp = new System.Drawing.Bitmap(size, size);
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(System.Drawing.Color.Transparent);

                    // helper: recta redondeada
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
                        using var bg = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(37, 211, 102)); // verde
                        var rect = new System.Drawing.RectangleF(1.5f, 1.5f, size - 3, size - 3);
                        using var path = RoundRect(rect, 4f);
                        g.FillPath(bg, path);

                        // cuerpo del lápiz (forma rellena más clara para mejor visibilidad)
                        using var bodyBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                        var body = new System.Drawing.PointF[] {
                            new System.Drawing.PointF(5f, 12f),
                            new System.Drawing.PointF(11f, 6f),
                            new System.Drawing.PointF(13f, 8f),
                            new System.Drawing.PointF(7f, 14f)
                        };
                        g.FillPolygon(bodyBrush, body);

                        // punta del lápiz (gris oscuro)
                        using var tipBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(80, 80, 80));
                        var tip = new System.Drawing.PointF[] {
                            new System.Drawing.PointF(11.5f, 5f),
                            new System.Drawing.PointF(13.5f, 7f),
                            new System.Drawing.PointF(11f, 8.5f)
                        };
                        g.FillPolygon(tipBrush, tip);

                        // detalle: línea de separación en el cuerpo
                        using var penDetail = new System.Drawing.Pen(System.Drawing.Color.FromArgb(200,200,200), 1f);
                        g.DrawLine(penDetail, 6f, 13f, 12f, 7f);
                    }
                    else if (action == "delete")
                    {
                        using var bg = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(220, 53, 69)); // rojo
                        var rect = new System.Drawing.RectangleF(1.5f, 1.5f, size - 3, size - 3);
                        using var path = RoundRect(rect, 4f);
                        g.FillPath(bg, path);

                        using var pen = new System.Drawing.Pen(System.Drawing.Color.White, 1.4f);
                        // tapa
                        g.DrawLine(pen, 5, 5, 13, 5);
                        // cuerpo
                        g.DrawRectangle(pen, 6, 6, 6, 8);
                        // líneas internas
                        g.DrawLine(pen, 8, 8, 8, 12);
                        g.DrawLine(pen, 10, 8, 10, 12);
                    }
                    else if (action == "view")
                    {
                        using var bg = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0, 123, 255)); // azul
                        var rect = new System.Drawing.RectangleF(1.5f, 1.5f, size - 3, size - 3);
                        using var path = RoundRect(rect, 4f);
                        g.FillPath(bg, path);

                        // letra i en blanco centrada
                        using var f = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                        using var b = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                        var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center };
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        g.DrawString("i", f, b, new System.Drawing.RectangleF(0, 0, size, size), sf);
                    }
                    else
                    {
                        using var bg = new System.Drawing.SolidBrush(System.Drawing.Color.Gray);
                        var rect = new System.Drawing.RectangleF(1.5f, 1.5f, size - 3, size - 3);
                        using var path = RoundRect(rect, 4f);
                        g.FillPath(bg, path);
                    }
                }
                return bmp;
            }

            // Iconos más discretos y con fondo coloreado
            var imgEdit = CreateActionIconStyled("edit");
            var imgDelete = CreateActionIconStyled("delete");
            var imgView = CreateActionIconStyled("view");

            var editColImg = new DataGridViewImageColumn() { Name = "Edit", HeaderText = "", Image = imgEdit, ImageLayout = DataGridViewImageCellLayout.Normal };
            var delColImg = new DataGridViewImageColumn() { Name = "Delete", HeaderText = "", Image = imgDelete, ImageLayout = DataGridViewImageCellLayout.Normal };
            var viewColImg = new DataGridViewImageColumn() { Name = "View", HeaderText = "", Image = imgView, ImageLayout = DataGridViewImageCellLayout.Normal };

            // Ajustes de tamaño y estilo para aspecto compacto
            editColImg.Width = 28; editColImg.ReadOnly = true; editColImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; editColImg.DefaultCellStyle.Padding = new Padding(2);
            delColImg.Width = 28; delColImg.ReadOnly = true; delColImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; delColImg.DefaultCellStyle.Padding = new Padding(2);
            viewColImg.Width = 28; viewColImg.ReadOnly = true; viewColImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; viewColImg.DefaultCellStyle.Padding = new Padding(2);

            dgvProducts.Columns.Add(editColImg);
            dgvProducts.Columns.Add(delColImg);
            dgvProducts.Columns.Add(viewColImg);
            // Eliminar columnas completamente vacías (sin valores en ninguna fila)
            try
            {
                var cols = new System.Collections.Generic.List<DataGridViewColumn>();
                foreach (DataGridViewColumn c in dgvProducts.Columns)
                    cols.Add(c);

                foreach (var c in cols)
                {
                    // no tocar columnas de acción ni la columna de imagen previa
                    if (c.Name == "Edit" || c.Name == "Delete" || c.Name == "View" || c.Name == "imgPreview")
                        continue;

                    // eliminar columnas con encabezado vacío (posible columna extra) o sin datos
                    if (string.IsNullOrWhiteSpace(c.HeaderText))
                    {
                        try { dgvProducts.Columns.Remove(c); continue; } catch { }
                    }

                    bool allEmpty = true;
                    foreach (DataGridViewRow r in dgvProducts.Rows)
                    {
                        if (r.IsNewRow) continue;
                        var v = r.Cells[c.Index].Value;
                        if (v != null && v != System.DBNull.Value)
                        {
                            if (v is string s)
                            {
                                if (!string.IsNullOrWhiteSpace(s)) { allEmpty = false; break; }
                            }
                            else
                            {
                                allEmpty = false; break;
                            }
                        }
                    }

                    if (allEmpty)
                    {
                        try { dgvProducts.Columns.Remove(c); } catch { }
                    }
                }
            }
            catch { }

            // Evitar que quede una celda seleccionada al cargar (quita el color azul inicial)
            try
            {
                dgvProducts.ClearSelection();
                if (dgvProducts.CurrentCell != null)
                    dgvProducts.CurrentCell = null;
            }
            catch { }
        }

        private void DgvProducts_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvProducts.Rows[e.RowIndex];
            var prevBack = row.DefaultCellStyle.BackColor;
            var prevFore = row.DefaultCellStyle.ForeColor;
            var prevSelBack = row.DefaultCellStyle.SelectionBackColor;
            try
            {
                // marcar en rojo y forzar repintado antes de mostrar el diálogo
                row.DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                row.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                row.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.DarkRed;
                dgvProducts.Refresh();

                var id = Convert.ToInt32(row.Cells["idProducto"].Value);
                var r = MessageBox.Show("¿Eliminar este producto?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    Database.DeleteProduct(id);
                    LoadProducts();
                }
                else
                {
                    // revertir estilos
                    row.DefaultCellStyle.BackColor = prevBack;
                    row.DefaultCellStyle.ForeColor = prevFore;
                    row.DefaultCellStyle.SelectionBackColor = prevSelBack;
                    dgvProducts.Refresh();
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

        private void DgvProducts_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
        {
            // seleccionar fila al hacer clic con el botón derecho
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvProducts.ClearSelection();
                dgvProducts.Rows[e.RowIndex].Selected = true;
            }
        }

        private void DgvProducts_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = dgvProducts.Columns[e.ColumnIndex].Name;
            var id = Convert.ToInt32(dgvProducts.Rows[e.RowIndex].Cells["idProducto"].Value);
            if (colName == "Edit")
            {
                using var f = new frmPrincipal(id);
                f.ShowDialog();
                LoadProducts();
            }
            else if (colName == "Delete")
            {
                var r = MessageBox.Show("¿Eliminar este producto?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    Database.DeleteProduct(id);
                    LoadProducts();
                }
            }
            else if (colName == "View")
            {
                var row = Database.GetProductById(id);
                using var v = new ProductoViewForm(row);
                v.ShowDialog();
            }
        }
    }
}
