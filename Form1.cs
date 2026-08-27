using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Registro_Productos
{
    public partial class frmPrincipal : Form
    {
        private int editingId = 0; // 0 => nuevo
        private string currentImagePath = null; // ruta relativa guardada en DB
        private string? tempSelectedFullPath = null; // ruta temporal del archivo seleccionado (antes de guardar)
        // Flags para mostrar alertas de ayuda solo una vez
        private bool shownProductoAlert = false;
        private bool shownPrecioAlert = false;
        private bool shownCantidadAlert = false;

        public frmPrincipal()
        {
            InitializeComponent();
            AttachEvents();
            LoadCategories();
            // Embebar formularios de Productos y Categorías dentro de la pestaña Administración
            try
            {
                var prodForm = new ProductosForm() { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
                var catForm = new CategoriesForm() { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };

                var adminTabs = new TabControl { Dock = DockStyle.Fill };
                var tpProducts = new TabPage("Productos");
                var tpCategories = new TabPage("Categorías");
                adminTabs.TabPages.Add(tpProducts);
                adminTabs.TabPages.Add(tpCategories);

                tpProducts.Controls.Add(prodForm);
                tpCategories.Controls.Add(catForm);

                // Recargar categorías al cambiar de pestaña para asegurar que los combos siempre tengan datos
                adminTabs.SelectedIndexChanged += (s, e) => {
                    try
                    {
                        if (adminTabs.SelectedIndex == 0)
                            prodForm.LoadCategories();
                        else
                            catForm.LoadCategories();
                    }
                    catch { }
                };

                this.tabPageAdmin.Controls.Add(adminTabs);

                prodForm.Show();
                catForm.Show();
            }
            catch { }
            // colocar icono de foto por defecto en el PictureBox si no hay imagen seleccionada
            try
            {
                if (this.pbImage != null && this.pbImage.Image == null)
                {
                    this.pbImage.SizeMode = PictureBoxSizeMode.Zoom;
                    this.pbImage.Image = CreatePhotoPlaceholder(this.pbImage.Width > 0 ? this.pbImage.Width : 160,
                                                               this.pbImage.Height > 0 ? this.pbImage.Height : 90);
                }
            }
            catch { }
        }

        private Image CreatePhotoPlaceholder(int width, int height)
        {
            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                // fondo
                g.Clear(Color.FromArgb(250, 250, 250));

                // borde
                using (var pen = new Pen(Color.FromArgb(200, 200, 200), 2))
                    g.DrawRectangle(pen, 2, 2, width - 5, height - 5);

                // montañas
                using (var brush = new SolidBrush(Color.FromArgb(230, 230, 230)))
                {
                    var points = new Point[] {
                        new Point((int)(width*0.05), (int)(height*0.8)),
                        new Point((int)(width*0.35), (int)(height*0.45)),
                        new Point((int)(width*0.6), (int)(height*0.7)),
                        new Point((int)(width*0.95), (int)(height*0.35)),
                        new Point((int)(width*0.95), (int)(height*0.8))
                    };
                    g.FillPolygon(brush, points);
                }

                // icono cámara (pequeño) en esquina superior izquierda
                using (var camBrush = new SolidBrush(Color.FromArgb(200, 200, 200)))
                using (var camPen = new Pen(Color.FromArgb(160,160,160), 1.5f))
                {
                    var rx = 8; var ry = 8; var rw = 36; var rh = 24;
                    g.FillRectangle(camBrush, rx, ry, rw, rh);
                    g.DrawRectangle(camPen, rx, ry, rw, rh);
                    g.FillEllipse(Brushes.WhiteSmoke, rx + rw - 12, ry - 6, 12, 12);
                }

                // sol/bola amarilla
                g.FillEllipse(Brushes.Gold, width - 22, 8, 12, 12);

                // texto "Imagen" centrado abajo
                using (var f = new Font("Segoe UI", Math.Max(8, width / 24), FontStyle.Regular, GraphicsUnit.Pixel))
                using (var b = new SolidBrush(Color.FromArgb(150, 150, 150)))
                {
                    var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    g.DrawString("Imagen", f, b, new RectangleF(0, height * 0.6f, width, height * 0.3f), sf);
                }
            }
            return bmp;
        }

        // KeyPress handlers para validar entrada en tiempo real
        private void txtProducto_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Permitir letras, control (backspace) y espacios
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
                try
                {
                    if (!shownProductoAlert)
                    {
                        MessageBox.Show("Sólo se permiten letras y espacios en este campo.", "Entrada no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        shownProductoAlert = true;
                    }
                }
                catch { }
            }
        }

        private void txtPrecio_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Permitir dígitos, separador decimal según cultura y teclas de control
            var decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar.ToString() != decimalSeparator)
            {
                e.Handled = true;
                try { if (!shownPrecioAlert) { MessageBox.Show("Precio: solo dígitos y un separador decimal válido.", "Entrada no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning); shownPrecioAlert = true; } } catch { }
                return;
            }
            var tb = sender as TextBox;
            if (e.KeyChar.ToString() == decimalSeparator && tb != null && tb.Text.Contains(decimalSeparator))
            {
                // Ya existe un separador decimal
                e.Handled = true;
                try { if (!shownPrecioAlert) { MessageBox.Show("Ya existe un separador decimal en el valor.", "Entrada no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning); shownPrecioAlert = true; } } catch { }
            }
        }

        private void txtCantidad_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Permitir solo dígitos y teclas de control
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                try { if (!shownCantidadAlert) { MessageBox.Show("Sólo se permiten números enteros en este campo.", "Entrada no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning); shownCantidadAlert = true; } } catch { }
            }
        }

        private void LoadCategories()
        {
            try
            {
                var dt = Database.GetAllCategories();
                // insertar opción por defecto al inicio
                if (dt == null) dt = new System.Data.DataTable();
                if (!dt.Columns.Contains("idCategoria")) dt.Columns.Add("idCategoria", typeof(int));
                if (!dt.Columns.Contains("nombre")) dt.Columns.Add("nombre", typeof(string));
                var empty = dt.NewRow();
                empty["idCategoria"] = DBNull.Value;
                empty["nombre"] = "-- Seleccione una categoría --";
                dt.Rows.InsertAt(empty, 0);

                cboCategoria.DisplayMember = "nombre";
                cboCategoria.ValueMember = "idCategoria";
                cboCategoria.DataSource = dt;
                cboCategoria.SelectedIndex = 0;
            }
            catch { }
        }

        // Constructor para editar
        public frmPrincipal(int id) : this()
        {
            LoadProductForEdit(id);
        }

        private void AttachEvents()
        {
            this.btnGuardar.Click += BtnGuardar_Click;
            // Adjuntar eventos directamente a los controles creados en el diseñador
            // permitir abrir selector al hacer click sobre la vista previa (pbImage)
            if (this.pbImage != null)
            {
                this.pbImage.Click += BtnBrowseImage_Click;
                this.pbImage.Cursor = Cursors.Hand;
            }
            if (this.btnVer != null)
                this.btnVer.Click += BtnVer_Click;
            if (this.btnCategorias != null)
                this.btnCategorias.Click += BtnCategorias_Click;
            // Validaciones en tiempo real y mostrar advertencia al entrar en cada campo
            try
            {
                if (this.txtProducto != null)
                {
                    this.txtProducto.KeyPress += txtProducto_KeyPress;
                }

                if (this.txtPrecio != null)
                {
                    this.txtPrecio.KeyPress += txtPrecio_KeyPress;
                }

                if (this.txtCantidad != null)
                {
                    this.txtCantidad.KeyPress += txtCantidad_KeyPress;
                }
            }
            catch { }
        }

        private void BtnVer_Click(object? sender, EventArgs e)
        {
            using var f = new ProductosForm();
            f.ShowDialog();
        }

        private void BtnCategorias_Click(object? sender, EventArgs e)
        {
            using var f = new CategoriesForm();
            f.ShowDialog();
            // refrescar categorias en el combo al volver
            try { LoadCategories(); } catch { }
        }

        private void BtnBrowseImage_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp|Todos los archivos|*.*";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            var src = ofd.FileName;
            try
            {
                // Guardar ruta temporal; la copia final se hará al guardar el producto
                tempSelectedFullPath = src;
                // Mostrar vista previa en picturebox
                if (this.pbImage != null)
                {
                    var pb = this.pbImage;
                    pb.Image?.Dispose();
                    using var fs = new FileStream(src, FileMode.Open, FileAccess.Read);
                    using var loaded = Image.FromStream(fs);
                    // crear una copia independiente para evitar dependencia del stream
                    pb.Image = new Bitmap(loaded);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            // No usamos placeholders; nada que limpiar aquí
            // Validaciones básicas
            var nombre = txtProducto.Text.Trim();
            var descripcion = txtDescripcion?.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese el nombre del producto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProducto.Focus();
                return;
            }



            if (!decimal.TryParse(txtPrecio.Text.Trim(), out var precio))
            {
                MessageBox.Show("Precio inválido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return;
            }

            if (!int.TryParse(txtCantidad.Text.Trim(), out var cantidad))
            {
                MessageBox.Show("Cantidad inválida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidad.Focus();
                return;
            }

            var disponible = rbDisponible.Checked;
            var categoriaId = cboCategoria?.SelectedValue == null || cboCategoria.SelectedValue is DBNull ? (int?)null : Convert.ToInt32(cboCategoria.SelectedValue);

            try
            {
                // Si se seleccionó una imagen temporal, copiarla a uploads y usar su ruta relativa
                if (!string.IsNullOrEmpty(tempSelectedFullPath) && File.Exists(tempSelectedFullPath))
                {
                    var uploadsDir = Path.Combine(Application.StartupPath, "uploads");
                    if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);
                    var destFileName = Guid.NewGuid().ToString() + Path.GetExtension(tempSelectedFullPath);
                    var destPath = Path.Combine(uploadsDir, destFileName);
                    File.Copy(tempSelectedFullPath, destPath, true);
                    currentImagePath = Path.Combine("uploads", destFileName);
                    // limpiar temporal
                    tempSelectedFullPath = null;
                }

                if (editingId == 0)
                {
                    var newId = Database.InsertProduct(nombre, precio, cantidad, disponible, currentImagePath, descripcion, categoriaId);
                    MessageBox.Show("Producto guardado. ID: " + newId, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                else
                {
                    var rows = Database.UpdateProduct(editingId, nombre, precio, cantidad, disponible, currentImagePath, descripcion, categoriaId);
                    MessageBox.Show("Producto actualizado. Filas afectadas: " + rows, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.editingId = 0;
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Mostrar alertas informativas cuando el usuario entra en un campo (una sola vez)
        private void TxtProducto_Enter_ShowAlert(object? sender, EventArgs e)
        {
            if (!shownProductoAlert)
            {
                MessageBox.Show("Ingrese sólo letras y espacios.", "Formato esperado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                shownProductoAlert = true;
            }
        }

        private void TxtPrecio_Enter_ShowAlert(object? sender, EventArgs e)
        {
            if (!shownPrecioAlert)
            {
                MessageBox.Show("Ingrese un número válido. Use el separador decimal de su configuración regional.", "Formato esperado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                shownPrecioAlert = true;
            }
        }

        private void TxtCantidad_Enter_ShowAlert(object? sender, EventArgs e)
        {
            if (!shownCantidadAlert)
            {
                MessageBox.Show("Ingrese sólo números enteros (sin decimales).", "Formato esperado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                shownCantidadAlert = true;
            }
        }

        public void LoadProductForEdit(int id)
        {
            var row = Database.GetProductById(id);
            if (row == null) return;
            this.editingId = Convert.ToInt32(row["idProducto"]);
            txtProducto.Text = row["producto"].ToString();
            txtDescripcion.Text = row.Table.Columns.Contains("descripcion") && row["descripcion"] != DBNull.Value ? row["descripcion"].ToString() : string.Empty;
            // seleccionar categoria si viene
            if (row.Table.Columns.Contains("categoriaId") && row["categoriaId"] != DBNull.Value)
            {
                try { cboCategoria.SelectedValue = Convert.ToInt32(row["categoriaId"]); } catch { }
            }
            txtPrecio.Text = Convert.ToDecimal(row["precio"]).ToString("F2");
            txtCantidad.Text = row["cantidad"].ToString();
            rbDisponible.Checked = Convert.ToBoolean(row["disponible"]);
            currentImagePath = row["img"] == DBNull.Value ? null : row["img"].ToString();
            // clear temporary selection
            tempSelectedFullPath = null;
            if (!string.IsNullOrEmpty(currentImagePath) && this.pbImage != null)
            {
                var fullPath = Path.Combine(Application.StartupPath, currentImagePath);
                if (File.Exists(fullPath))
                {
                    this.pbImage.Image?.Dispose();
                    using var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                    using var loaded = Image.FromStream(fs);
                    // crear una copia independiente para que la imagen funcione aunque el stream se cierre
                    this.pbImage.Image = new Bitmap(loaded);
                }
            }
        }



        private void ClearForm()
        {
            // Restablecer campos a estado inicial
            editingId = 0;
            txtProducto.Text = "";
            if (this.txtDescripcion != null) txtDescripcion.Text = "";
            txtPrecio.Text = "";
            txtCantidad.Text = "";
            rbDisponible.Checked = false;
            currentImagePath = null;
            tempSelectedFullPath = null;

            // Resetear banderas de alertas para que vuelvan a mostrarse si el usuario comete
            // un error en la siguiente entrada
            shownProductoAlert = false;
            shownPrecioAlert = false;
            shownCantidadAlert = false;

            // Restaurar imagen por defecto en el PictureBox
            if (this.pbImage != null)
            {
                this.pbImage.Image?.Dispose();
                this.pbImage.Image = null;
                try
                {
                    this.pbImage.SizeMode = PictureBoxSizeMode.Zoom;
                    this.pbImage.Image = CreatePhotoPlaceholder(this.pbImage.Width > 0 ? this.pbImage.Width : 160,
                                                               this.pbImage.Height > 0 ? this.pbImage.Height : 90);
                }
                catch { }
            }

            // Restaurar selección de categoría al valor por defecto si existe
            try { if (this.cboCategoria != null) this.cboCategoria.SelectedIndex = 0; } catch { }

            // Colocar foco en el primer campo
            try { txtProducto.Focus(); } catch { }
        }
    }
}

