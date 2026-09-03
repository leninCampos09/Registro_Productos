using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Registro_Productos
{
    public partial class frmPrincipal : Form
    {
        private bool productOnlyMode = false;
        private int editingId = 0; // 0 => nuevo
        private string currentImagePath = null; // ruta relativa guardada en DB
        private string? tempSelectedFullPath = null; // ruta temporal del archivo seleccionado (antes de guardar)
        // Flags para mostrar alertas de ayuda solo una vez
        private bool shownProductoAlert = false;
        private bool shownPrecioAlert = false;
        private bool shownCantidadAlert = false;

        public frmPrincipal(bool showProductTab = false)
        {
            InitializeComponent();
            // marcar modo solo producto si se solicita
            this.productOnlyMode = showProductTab;
            // Si no se solicita la pestaña de producto, eliminarla del TabControl
            try
            {
                if (!showProductTab && this.tabMain != null && this.tabPageProduct != null)
                {
                    this.tabMain.Controls.Remove(this.tabPageProduct);
                }
            }
            catch { }
            try { SetupMenuIcons(); } catch { }
            AttachEvents();
            LoadCategories();
            // Embebar formularios de Productos y Categorías dentro de la pestaña Administración
            try
            {
                if (!this.productOnlyMode)
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
            }
            catch { }

            // Si estamos en modo solo producto, ocultar menú, toolbar y mostrar solo el layout de producto
            try
            {
                if (this.productOnlyMode)
                {
                    if (this.menuStrip1 != null) this.menuStrip1.Visible = false;
                    if (this.toolStrip1 != null) this.toolStrip1.Visible = false;

                    // Mover tlpMain fuera de la pestaña y añadirlo directamente al formulario
                    if (this.tabPageProduct != null && this.tlpMain != null)
                    {
                        try { this.tabPageProduct.Controls.Remove(this.tlpMain); } catch { }
                        this.Controls.Add(this.tlpMain);
                        this.tlpMain.Dock = DockStyle.Fill;
                    }

                    // Eliminar control tabMain para que no se muestre
                    try { if (this.tabMain != null) this.Controls.Remove(this.tabMain); } catch { }
                }
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

        // Handlers para menú/toolbar
        private void menuVentas_Click(object? sender, EventArgs e)
        {
            try
            {
                new VentasForm() { StartPosition = FormStartPosition.CenterParent }.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir Ventas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuStock_Click(object? sender, EventArgs e)
        {
            try
            {
                // Seleccionar la pestaña Administración en el TabControl principal
                if (this.tabMain != null && this.tabPageAdmin != null)
                    this.tabMain.SelectedTab = this.tabPageAdmin;

                // Buscar el TabControl embebido dentro de tabPageAdmin y seleccionar la pestaña "Productos" (índice 0)
                var adminTabs = this.tabPageAdmin?.Controls.OfType<TabControl>().FirstOrDefault();
                if (adminTabs != null && adminTabs.TabCount > 0)
                    adminTabs.SelectedIndex = 0; // muestra el UserControl/Product form embebido
            }
            catch { }
        }

        private void menuProveedores_Click(object? sender, EventArgs e)
        {
            try
            {
                new ProveedoresForm() { StartPosition = FormStartPosition.CenterParent }.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir Proveedores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuCategorias_Click(object? sender, EventArgs e)
        {
            try
            {
                // Seleccionar la pestaña Administración en el TabControl principal
                if (this.tabMain != null && this.tabPageAdmin != null)
                    this.tabMain.SelectedTab = this.tabPageAdmin;

                // Buscar el TabControl embebido dentro de tabPageAdmin y seleccionar la pestaña "Categorías" (índice 1)
                var adminTabs = this.tabPageAdmin?.Controls.OfType<TabControl>().FirstOrDefault();
                if (adminTabs != null && adminTabs.TabCount > 1)
                    adminTabs.SelectedIndex = 1; // muestra la pestaña Categorías embebida
            }
            catch { }
        }

        private void menuInformes_Click(object? sender, EventArgs e)
        {
            try
            {
                new InformesForm() { StartPosition = FormStartPosition.CenterParent }.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir Informes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        public frmPrincipal(int id) : this(true)
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

        // Crea iconos simples (placeholders) y los asigna a los items del MenuStrip/ToolStrip
        private void SetupMenuIcons()
        {
            try { this.toolStrip1.ImageScalingSize = new System.Drawing.Size(48, 48); } catch { }

            // Intentar cargar imágenes desde la carpeta "images" del proyecto (ruta relativa desde el output).
            string[] candidateNames = new string[] { "registros.png", "ventas.png", "stock.png", "proveedor.png", "compras.png", "categorias.png", "informes.png" };
            var icons = new System.Collections.Generic.Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
            icons["registros"] = LoadProjectImage("registros.png") ?? CreateIconBitmap("R", Color.FromArgb(86, 173, 85));
            icons["ventas"] = LoadProjectImage("ventas.png") ?? CreateIconBitmap("V", Color.FromArgb(0, 120, 215));
            icons["stock"] = LoadProjectImage("stock.png") ?? CreateIconBitmap("S", Color.FromArgb(255, 165, 0));
            icons["proveedores"] = LoadProjectImage("proveedor.png") ?? CreateIconBitmap("P", Color.FromArgb(155, 89, 182));
            icons["compras"] = LoadProjectImage("compras.png") ?? CreateIconBitmap("Co", Color.FromArgb(120, 120, 200));
            icons["categorias"] = LoadProjectImage("categorias.png") ?? CreateIconBitmap("C", Color.FromArgb(52, 152, 219));
            icons["informes"] = LoadProjectImage("informes.png") ?? CreateIconBitmap("I", Color.FromArgb(26, 188, 156));

            try
            {
                if (this.registrosToolStripMenuItem != null) this.registrosToolStripMenuItem.Image = icons["registros"];
                if (this.ventasToolStripMenuItem != null) this.ventasToolStripMenuItem.Image = icons["ventas"];
                if (this.stockToolStripMenuItem != null) this.stockToolStripMenuItem.Image = icons["stock"];
                if (this.proveedoresToolStripMenuItem != null) this.proveedoresToolStripMenuItem.Image = icons["proveedores"];
                if (this.comprasToolStripMenuItem != null) this.comprasToolStripMenuItem.Image = icons["compras"];
                if (this.categoriasToolStripMenuItem != null) this.categoriasToolStripMenuItem.Image = icons["categorias"];
                if (this.informesToolStripMenuItem != null) this.informesToolStripMenuItem.Image = icons["informes"];
            }
            catch { }

            try
            {
                if (this.toolStripButtonRegistros != null)
                {
                    this.toolStripButtonRegistros.Image = icons["registros"];
                    this.toolStripButtonRegistros.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    this.toolStripButtonRegistros.TextImageRelation = TextImageRelation.ImageAboveText;
                }
                if (this.toolStripButtonInformes != null)
                {
                    this.toolStripButtonInformes.Image = icons["informes"];
                    this.toolStripButtonInformes.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    this.toolStripButtonInformes.TextImageRelation = TextImageRelation.ImageAboveText;
                }
                try { if (this.toolStripButtonVentas != null) this.toolStripButtonVentas.Image = icons["ventas"]; } catch { }
                try { if (this.toolStripButtonStock != null) this.toolStripButtonStock.Image = icons["stock"]; } catch { }
                try { if (this.toolStripButtonProveedores != null) this.toolStripButtonProveedores.Image = icons["proveedores"]; } catch { }
                try { if (this.toolStripButtonCompras != null) this.toolStripButtonCompras.Image = icons["compras"]; } catch { }
                try { if (this.toolStripButtonCategorias != null) this.toolStripButtonCategorias.Image = icons["categorias"]; } catch { }
            }
            catch { }
        }

        private void menuCompras_Click(object? sender, EventArgs e)
        {
            try
            {
                // Si existe un formulario ComprasForm lo abrimos, si no, abrimos Proveedores como alternativa
                // Abrir el listado de compras (si existe) para ver los registros guardados
                var t = typeof(frmPrincipal).Assembly.GetType("Registro_Productos.ComprasListForm");
                if (t != null && typeof(Form).IsAssignableFrom(t))
                {
                    var frm = (Form)Activator.CreateInstance(t)!;
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.Show();
                }
                else
                {
                    // Fallback: abrir el formulario de creación de compras
                    var t2 = typeof(frmPrincipal).Assembly.GetType("Registro_Productos.ComprasForm");
                    if (t2 != null && typeof(Form).IsAssignableFrom(t2))
                    {
                        var frm2 = (Form)Activator.CreateInstance(t2)!;
                        frm2.StartPosition = FormStartPosition.CenterParent;
                        frm2.Show();
                    }
                    else
                    {
                        new ProveedoresForm() { StartPosition = FormStartPosition.CenterParent }.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir Compras: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Al pulsar el botón principal "Registros" seleccionamos la pestaña Administración
        private void menuRegistros_Click(object? sender, EventArgs e)
        {
            try { if (this.tabMain != null && this.tabPageAdmin != null) this.tabMain.SelectedTab = this.tabPageAdmin; } catch { }
        }

        private Image LoadProjectImage(string fileName)
        {
            try
            {
                // Rutas candidatas desde el output (bin) hacia la carpeta del proyecto
                var outDir = Application.StartupPath; // e.g. ...\bin\Debug\net10.0-windows
                var pathsToTry = new string[] {
                    Path.Combine(outDir, "images", fileName),
                    Path.Combine(outDir, "..", "..", "..", "images", fileName),
                    Path.Combine(outDir, "..", "..", "images", fileName),
                    Path.Combine(Directory.GetCurrentDirectory(), "images", fileName),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", fileName)
                };
                foreach (var p in pathsToTry)
                {
                    try
                    {
                        var full = Path.GetFullPath(p);
                        if (File.Exists(full))
                        {
                            using var fs = new FileStream(full, FileMode.Open, FileAccess.Read);
                            var img = Image.FromStream(fs);
                            return new Bitmap(img);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return null;
        }

        private Image CreateIconBitmap(string letter, Color bg)
        {
            int size = 48;
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (var b = new SolidBrush(bg))
                    g.FillEllipse(b, 0, 0, size - 1, size - 1);
                using (var p = new Pen(Color.FromArgb(200, 200, 200), 2))
                    g.DrawEllipse(p, 1, 1, size - 3, size - 3);
                using (var f = new Font("Segoe UI", 18, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var sb = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    g.DrawString(letter, f, sb, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bmp;
        }
    }
}

