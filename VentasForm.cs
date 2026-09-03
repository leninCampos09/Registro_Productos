using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Registro_Productos
{
    public class VentasForm : Form
    {
        private ComboBox cboProducto;
        private NumericUpDown nudCantidad;
        private Label lblPrecio, lblTotal, lblStock;
        private Button btnVender, btnRefresh, btnEliminar, btnPrint;
        private Button btnVerDetalles;
        private Button btnAgregarCarrito, btnQuitarCarrito, btnVenderCarrito;
        private DataGridView dgvCart;
        private DataTable cartTable;
        private DataGridView dgv;
        private Label lblCartTotal;
        private TextBox txtClienteNombre, txtClienteTelefono, txtClienteEmail;

        public VentasForm()
        {
            this.Text = "Ventas";
            this.Size = new Size(800, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponents();
            LoadProductos();
            LoadVentas();
        }

        private void BtnAgregarCarrito_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!(cboProducto.SelectedItem is ComboboxItem it)) return;
                var productoId = Convert.ToInt32(it.Value);
                var nombre = it.Text;
                var precio = it.Price;
                var qty = Convert.ToInt32(nudCantidad.Value);
                if (qty <= 0) return;
                if (it.Stock < qty)
                {
                    SweetAlert.ShowError(this, "Stock insuficiente", $"Stock disponible: {it.Stock}. No puede agregar {qty} unidades.");
                    return;
                }

                // si ya existe en el carrito, sumar cantidades
                var existing = cartTable.AsEnumerable().FirstOrDefault(r => r.Field<int>("productoId") == productoId);
                if (existing != null)
                {
                    var newQty = existing.Field<int>("Cantidad") + qty;
                    existing.SetField("Cantidad", newQty);
                    existing.SetField("Total", newQty * existing.Field<decimal>("PrecioUnit"));
                }
                else
                {
                    var row = cartTable.NewRow();
                    row["productoId"] = productoId;
                    row["Producto"] = nombre;
                    row["PrecioUnit"] = precio;
                    row["Cantidad"] = qty;
                    row["Total"] = precio * qty;
                    cartTable.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                SweetAlert.ShowError(this, "Error", ex.Message);
            }
        }

        private void BtnQuitarCarrito_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvCart.CurrentRow == null) return;
                var prodId = Convert.ToInt32(dgvCart.CurrentRow.Cells["productoId"].Value);
                var row = cartTable.AsEnumerable().FirstOrDefault(r => r.Field<int>("productoId") == prodId);
                if (row != null) cartTable.Rows.Remove(row);
            }
            catch (Exception ex)
            {
                SweetAlert.ShowError(this, "Error", ex.Message);
            }
        }

        private void BtnVenderCarrito_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cartTable.Rows.Count == 0)
                {
                    SweetAlert.ShowInfo(this, "Carrito vacío", "Agregue productos al carrito antes de vender.");
                    return;
                }

                // obtener datos cliente
                string? clienteNombre = null;
                string? clienteTelefono = null;
                string? clienteEmail = null;
                try
                {
                    if (txtClienteNombre != null && txtClienteNombre.ForeColor != Color.Gray)
                    {
                        var s = txtClienteNombre.Text?.Trim(); if (!string.IsNullOrEmpty(s)) clienteNombre = s;
                    }
                    if (txtClienteTelefono != null && txtClienteTelefono.ForeColor != Color.Gray)
                    {
                        var s = txtClienteTelefono.Text?.Trim(); if (!string.IsNullOrEmpty(s)) clienteTelefono = s;
                    }
                    if (txtClienteEmail != null && txtClienteEmail.ForeColor != Color.Gray)
                    {
                        var s = txtClienteEmail.Text?.Trim(); if (!string.IsNullOrEmpty(s)) clienteEmail = s;
                    }
                }
                catch { }

                // Validar stock globalmente
                foreach (DataRow r in cartTable.Rows)
                {
                    var pid = r.Field<int>("productoId");
                    var qty = r.Field<int>("Cantidad");
                    // buscar en productos actuales
                    var prod = Database.GetProductById(pid);
                    if (prod == null) { SweetAlert.ShowError(this, "Error", "Producto no encontrado en base de datos."); return; }
                    // Determinar columna de stock/cantidad (case-insensitive)
                    var stockCol = prod.Table.Columns.Cast<System.Data.DataColumn>().FirstOrDefault(c => string.Equals(c.ColumnName, "cantidad", StringComparison.OrdinalIgnoreCase) || string.Equals(c.ColumnName, "stock", StringComparison.OrdinalIgnoreCase));
                    if (stockCol == null)
                    {
                        SweetAlert.ShowError(this, "Error", "No se pudo determinar la columna de stock para el producto.");
                        return;
                    }
                    var stock = Convert.ToInt32(prod[stockCol]);
                    if (stock < qty) { SweetAlert.ShowError(this, "Stock insuficiente", $"Producto {r.Field<string>("Producto")} tiene stock {stock}."); return; }
                }

                // Confirmar
                var dr = SweetAlert.ShowConfirm(this, "Confirmar venta", "Vender todos los productos del carrito? Esto generará varias ventas.");
                if (dr != DialogResult.Yes) return;

                // Procesar la venta como una venta maestra con detalles (un solo registro en ventas)
                var newVentaId = Database.InsertVentaWithDetails(cartTable, clienteNombre, clienteTelefono, clienteEmail);

                SweetAlert.ShowSuccess(this, "Venta registrada", "La venta con detalles se ha registrado.");
                cartTable.Clear();
                // reset cliente y campos
                try
                {
                    if (txtClienteNombre != null) { txtClienteNombre.Text = "Nombre cliente"; txtClienteNombre.ForeColor = Color.Gray; }
                    if (txtClienteTelefono != null) { txtClienteTelefono.Text = "Teléfono"; txtClienteTelefono.ForeColor = Color.Gray; }
                    if (txtClienteEmail != null) { txtClienteEmail.Text = "Email"; txtClienteEmail.ForeColor = Color.Gray; }
                    if (nudCantidad != null) nudCantidad.Value = 1;
                    if (cboProducto != null && cboProducto.Items.Count > 0) cboProducto.SelectedIndex = 0;
                    lblTotal.Text = "Total: 0.00";
                    lblCartTotal.Text = "Total carrito: 0.00";
                }
                catch { }
                LoadVentas();
                LoadProductos();
            }
            catch (Exception ex)
            {
                SweetAlert.ShowError(this, "Error al vender carrito", ex.Message);
            }
        }

        private void BtnPrint_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgv.CurrentRow == null)
                {
                    SweetAlert.ShowInfo(this, "Seleccione venta", "Seleccione una venta en la tabla para imprimir el ticket.");
                    return;
                }
                var idObj = dgv.CurrentRow.Cells["idVenta"]?.Value;
                if (idObj == null) { SweetAlert.ShowError(this, "Error", "ID de venta no disponible."); return; }
                var id = Convert.ToInt32(idObj);
                ShowPrintPreviewForSale(id);
            }
            catch (Exception ex)
            {
                SweetAlert.ShowError(this, "Error al imprimir", ex.Message);
            }
        }

        private void ShowPrintPreviewForSale(int saleId)
        {
            var row = Database.GetVentaById(saleId);
            if (row == null) return;
            var detalles = Database.GetVentaDetalles(saleId);

            var doc = new System.Drawing.Printing.PrintDocument();
            doc.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(40, 40, 40, 40);
            doc.PrintPage += (s, e) =>
            {
                var g = e.Graphics;
                g.Clear(Color.White);
                var pageWidth = e.MarginBounds.Width;
                float y = e.MarginBounds.Top;

                // Fuentes para el nuevo estilo tipo ticket
                using var titleFont = new Font("Arial", 18, FontStyle.Bold);
                using var subtitleFont = new Font("Arial", 10, FontStyle.Regular);
                using var bodyFont = new Font("Arial", 9, FontStyle.Regular);
                using var boldFont = new Font("Arial", 9, FontStyle.Bold);
                using var totalFont = new Font("Arial", 12, FontStyle.Bold);
                using var barcodeFont = new Font("Consolas", 16, FontStyle.Regular);

                // Header grande centrado
                var header = "CASH RECEIPT";
                var headerSize = g.MeasureString(header, titleFont);
                g.DrawString(header, titleFont, Brushes.Black, e.MarginBounds.Left + (pageWidth - headerSize.Width) / 2, y);
                y += headerSize.Height + 6;

                // Empresa / direccion / telefono centrados en gris
                var company = "Registro Productos";
                var address = "Direccion: Ciudad, Calle 123";
                var phone = "Tel: 000-000-000";
                var compSize = g.MeasureString(company, subtitleFont);
                g.DrawString(company, subtitleFont, Brushes.DarkGray, e.MarginBounds.Left + (pageWidth - compSize.Width) / 2, y);
                y += subtitleFont.GetHeight(g) + 2;
                var addrSize = g.MeasureString(address, bodyFont);
                g.DrawString(address, bodyFont, Brushes.DarkGray, e.MarginBounds.Left + (pageWidth - addrSize.Width) / 2, y);
                y += bodyFont.GetHeight(g) + 1;
                var phSize = g.MeasureString(phone, bodyFont);
                g.DrawString(phone, bodyFont, Brushes.DarkGray, e.MarginBounds.Left + (pageWidth - phSize.Width) / 2, y);
                y += bodyFont.GetHeight(g) + 8;

                // Línea punteada
                using (var pen = new Pen(Color.DarkGray)) { pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; g.DrawLine(pen, e.MarginBounds.Left, y, e.MarginBounds.Left + pageWidth, y); }
                y += 8;

                // Fecha a la izquierda, hora/ID a la derecha para mantener campos originales
                var fechaValor = Convert.ToDateTime(row["fecha"]);
                fechaValor = Database.ConvertUtcToElSalvador(fechaValor);
                var fechaText = fechaValor.ToString("dd/MM/yyyy");
                var horaText = fechaValor.ToString("hh:mm tt");
                g.DrawString("Date: " + fechaText, bodyFont, Brushes.Black, e.MarginBounds.Left, y);
                var rightText = horaText;
                var rightSize = g.MeasureString(rightText, bodyFont);
                g.DrawString(rightText, bodyFont, Brushes.Black, e.MarginBounds.Left + pageWidth - rightSize.Width, y);
                y += bodyFont.GetHeight(g) + 8;

                // Otra línea punteada
                using (var pen = new Pen(Color.LightGray)) { pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; g.DrawLine(pen, e.MarginBounds.Left, y, e.MarginBounds.Left + pageWidth, y); }
                y += 8;

                // Mostrar datos del cliente si existen
                var cliente = row.Table.Columns.Contains("clienteNombre") && row["clienteNombre"] != DBNull.Value ? row["clienteNombre"].ToString() : string.Empty;
                var telefono = row.Table.Columns.Contains("clienteTelefono") && row["clienteTelefono"] != DBNull.Value ? row["clienteTelefono"].ToString() : string.Empty;
                var email = row.Table.Columns.Contains("clienteEmail") && row["clienteEmail"] != DBNull.Value ? row["clienteEmail"].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(cliente))
                {
                    g.DrawString("Cliente: ", boldFont, Brushes.Black, e.MarginBounds.Left, y);
                    var clienteSize = g.MeasureString("Cliente: ", boldFont);
                    g.DrawString(cliente, bodyFont, Brushes.Black, e.MarginBounds.Left + clienteSize.Width, y);
                    y += bodyFont.GetHeight(g) + 2;
                    if (!string.IsNullOrEmpty(telefono))
                    {
                        g.DrawString("Tel: ", boldFont, Brushes.Black, e.MarginBounds.Left, y);
                        var telSize = g.MeasureString("Tel: ", boldFont);
                        g.DrawString(telefono, bodyFont, Brushes.Black, e.MarginBounds.Left + telSize.Width, y);
                        y += bodyFont.GetHeight(g) + 2;
                    }
                    if (!string.IsNullOrEmpty(email))
                    {
                        g.DrawString("Email: ", boldFont, Brushes.Black, e.MarginBounds.Left, y);
                        var emSize = g.MeasureString("Email: ", boldFont);
                        g.DrawString(email, bodyFont, Brushes.Black, e.MarginBounds.Left + emSize.Width, y);
                        y += bodyFont.GetHeight(g) + 6;
                    }

                    // Línea separadora entre datos del cliente y los items
                    using (var penSep = new Pen(Color.LightGray)) { penSep.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; g.DrawLine(penSep, e.MarginBounds.Left, y, e.MarginBounds.Left + pageWidth, y); }
                    y += 10;
                }

                // Columnas: producto (izq) - qty (centro) - precio unitario (centro-derecha) - subtotal (derecha)
                float col1 = e.MarginBounds.Left;
                float col2 = e.MarginBounds.Left + pageWidth * 0.52f;
                float col3 = e.MarginBounds.Left + pageWidth * 0.75f;
                float col4 = e.MarginBounds.Left + pageWidth * 0.95f;

                decimal total = 0m;
                if (detalles != null && detalles.Rows.Count > 0)
                {
                    // Dibujar cada linea
                    foreach (DataRow d in detalles.Rows)
                    {
                        var producto = d.Table.Columns.Contains("producto") && d["producto"] != DBNull.Value ? d["producto"].ToString() : (d.Table.Columns.Contains("productoId") ? d["productoId"].ToString() : "");
                        var cantidad = d.Table.Columns.Contains("cantidad") ? d["cantidad"].ToString() : "0";
                        var precioUnit = d.Table.Columns.Contains("precioUnit") ? Convert.ToDecimal(d["precioUnit"]) : 0m;
                        var subtotal = d.Table.Columns.Contains("total") ? Convert.ToDecimal(d["total"]) : precioUnit * Convert.ToDecimal(cantidad);

                        // Nombre
                        var prodRect = new RectangleF(col1, y, col2 - col1 - 4, 200);
                        g.DrawString(producto, bodyFont, Brushes.Black, prodRect);

                        // Cantidad
                        var qtyText = cantidad;
                        var qtySize = g.MeasureString(qtyText, bodyFont);
                        g.DrawString(qtyText, bodyFont, Brushes.Black, col2 + ((col3 - col2) - qtySize.Width) / 2, y);

                        // Precio unitario
                        var priceText = precioUnit.ToString("F2");
                        var puSize = g.MeasureString(priceText, bodyFont);
                        g.DrawString(priceText, bodyFont, Brushes.Black, col3 + ((col4 - col3) - puSize.Width) / 2, y);

                        // Subtotal
                        var stText = subtotal.ToString("F2");
                        var stSize = g.MeasureString(stText, boldFont);
                        g.DrawString(stText, boldFont, Brushes.Black, col4 - stSize.Width, y);

                        y += Math.Max(g.MeasureString(producto, bodyFont, (int)(col2 - col1 - 4)).Height, bodyFont.GetHeight(g)) + 6;
                        total += subtotal;
                    }
                }
                else
                {
                    // Fallback a venta individual antigua
                    var producto = row.Table.Columns.Contains("producto") && row["producto"] != DBNull.Value ? row["producto"].ToString() : string.Empty;
                    var cantidad = row.Table.Columns.Contains("cantidad") ? row["cantidad"].ToString() : "0";
                    total = row.Table.Columns.Contains("total") ? Convert.ToDecimal(row["total"]) : 0m;
                    var prodRect = new RectangleF(col1, y, col2 - col1 - 4, 200);
                    g.DrawString(producto, bodyFont, Brushes.Black, prodRect);
                    var qtyPref = "cant: " + cantidad;
                    var qtySize = g.MeasureString(qtyPref, bodyFont);
                    g.DrawString(qtyPref, bodyFont, Brushes.Black, col2 + ((col3 - col2) - qtySize.Width) / 2, y);
                    var itemPriceText = "total: $" + total.ToString("F2");
                    var stSize = g.MeasureString(itemPriceText, boldFont);
                    g.DrawString(itemPriceText, boldFont, Brushes.Black, col4 - stSize.Width, y);
                    y += Math.Max(g.MeasureString(producto, bodyFont, (int)(col2 - col1 - 4)).Height, bodyFont.GetHeight(g)) + 12;
                }

                // Línea separadora antes del total
                using (var pen = new Pen(Color.DarkGray)) { pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; g.DrawLine(pen, e.MarginBounds.Left, y, e.MarginBounds.Left + pageWidth, y); }
                y += 10;

                // Total grande (estilo ejemplo) y detalle abajo
                var totalLabel = "Total";
                var totalText = total.ToString("F2");
                var totalLabelSize = g.MeasureString(totalLabel, totalFont);
                var totalValueSize = g.MeasureString(totalText, totalFont);
                // Dibujar Total grande: etiqueta a la izquierda del bloque y valor a la derecha
                g.DrawString(totalLabel, totalFont, Brushes.Black, e.MarginBounds.Left + col1 - e.MarginBounds.Left + 4, y);
                g.DrawString(totalText, totalFont, Brushes.Black, e.MarginBounds.Left + pageWidth - totalValueSize.Width, y);
                y += totalFont.GetHeight(g) + 12;

                // Detalle: Sub-total, Sales Tax, Balance (alineados a la derecha)
                decimal salesTax = 0.00m; // ajustar si aplica
                decimal subTotal = total - salesTax;
                decimal balance = total;

                void DrawDetailLine(string label, decimal value)
                {
                    var labSize = g.MeasureString(label, bodyFont);
                    var valText = value.ToString("F2");
                    var valSize = g.MeasureString(valText, bodyFont);
                    g.DrawString(label, bodyFont, Brushes.Black, e.MarginBounds.Left + 4, y);
                    g.DrawString(valText, bodyFont, Brushes.Black, e.MarginBounds.Left + pageWidth - valSize.Width, y);
                    y += bodyFont.GetHeight(g) + 4;
                }

                DrawDetailLine("Sub-total", subTotal);
                DrawDetailLine("Sales Tax", salesTax);
                DrawDetailLine("Balance", balance);

                y += 8;

                // THANK YOU centrado
                var thanks = "THANK YOU";
                var thanksSize = g.MeasureString(thanks, boldFont);
                g.DrawString(thanks, boldFont, Brushes.Black, e.MarginBounds.Left + (pageWidth - thanksSize.Width) / 2, y);
                y += thanksSize.Height + 12;

                // Dibujar un código de barras estilizado (simulado)
                var bcLeft = e.MarginBounds.Left + (pageWidth - 220) / 2;
                var bcTop = y;
                var rnd = new Random(Convert.ToInt32(row["idVenta"]));
                int lines = 40;
                int bcWidth = 220;
                for (int i = 0; i < lines; i++)
                {
                    int h = 20 + (i % 3 == 0 ? 6 : 0);
                    int x = bcLeft + i * (bcWidth / lines);
                    g.DrawLine(Pens.Black, x, bcTop, x, bcTop + h);
                }
                y += 28;

                // ID debajo del codigo
                var idText = $"ID: {row["idVenta"]}";
                var idSize = g.MeasureString(idText, bodyFont);
                g.DrawString(idText, bodyFont, Brushes.Gray, e.MarginBounds.Left + (pageWidth - idSize.Width) / 2, y);

                // Footer
                y += totalFont.GetHeight(g) + 12;
                g.DrawLine(Pens.LightGray, e.MarginBounds.Left, y, e.MarginBounds.Left + pageWidth, y);
                y += 8;
                g.DrawString("Gracias por su compra", bodyFont, Brushes.Gray, e.MarginBounds.Left, y);
            };

            using var pp = new PrintPreviewDialog { Document = doc, Width = 900, Height = 700 };
            pp.ShowDialog(this);
        }

        private void InitializeComponents()
        {
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 100 };

            var lblProd = new Label { Text = "Producto:", Left = 10, Top = 12, AutoSize = true };
            cboProducto = new ComboBox { Left = 80, Top = 10, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };

            lblPrecio = new Label { Text = "Precio: 0.00", Left = 400, Top = 12, AutoSize = true };
            var lblStockTitle = new Label { Text = "Stock:", Left = 520, Top = 12, AutoSize = true };
            lblStock = new Label
            {
                Name = "lblStock",
                Text = "-",
                Left = 570,
                Top = 10,
                AutoSize = false,
                Size = new Size(48, 22),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(230, 245, 230),
                ForeColor = Color.FromArgb(34, 139, 34),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblQty = new Label { Text = "Cantidad:", Left = 10, Top = 46, AutoSize = true };
            nudCantidad = new NumericUpDown { Left = 80, Top = 44, Width = 80, Minimum = 1, Maximum = 100000, Value = 1 };

            lblTotal = new Label { Text = "Total: 0.00", Left = 180, Top = 46, AutoSize = true };
            btnVender = new Button { Text = "Registrar venta", Left = 260, Top = 42, Width = 120 };
            btnVerDetalles = new Button { Text = "Ver detalles", Left = 390, Top = 42, Width = 100 };
            btnAgregarCarrito = new Button { Text = "Agregar al carrito", Left = 200, Top = 42, Width = 130 };
            btnQuitarCarrito = new Button { Text = "Quitar del carrito", Left = 510, Top = 42, Width = 120 };
            btnVenderCarrito = new Button { Text = "Vender carrito", Left = 640, Top = 42, Width = 120 };
            btnRefresh = new Button { Text = "Actualizar ventas", Left = 770, Top = 42, Width = 120 };
            btnEliminar = new Button { Text = "Eliminar venta", Left = 900, Top = 42, Width = 120 };
            btnPrint = new Button { Text = "Imprimir ticket", Left = 1030, Top = 42, Width = 120 };
            btnVender.Click += BtnVender_Click;
            btnAgregarCarrito.Click += BtnAgregarCarrito_Click;
            btnQuitarCarrito.Click += BtnQuitarCarrito_Click;
            btnVenderCarrito.Click += BtnVenderCarrito_Click;
            btnVerDetalles.Click += (s, e) => ShowVentaDetallesFromSelection();
            btnRefresh.Click += (s, e) => LoadVentas();
            btnEliminar.Click += BtnEliminar_Click;
            btnPrint.Click += BtnPrint_Click;
            cboProducto.SelectedIndexChanged += (s, e) =>
            {
                // placeholder no-op to ensure handler exists; still call update logic
                UpdatePrecioYTotal();
            };
            nudCantidad.ValueChanged += (s, e) => UpdatePrecioYTotal();

            // Campos de cliente en segunda fila con texto placeholder gris
            var lblCliente = new Label { Text = "Cliente:", Left = 10, Top = 78, AutoSize = true };
            txtClienteNombre = new TextBox { Left = 80, Top = 74, Width = 220, Text = "Nombre cliente", ForeColor = Color.Gray };
            txtClienteTelefono = new TextBox { Left = 310, Top = 74, Width = 140, Text = "Teléfono", ForeColor = Color.Gray };
            txtClienteEmail = new TextBox { Left = 460, Top = 74, Width = 240, Text = "Email", ForeColor = Color.Gray };

            // Eventos para simular placeholder compatible
            txtClienteNombre.Enter += (s, e) => { if (txtClienteNombre.ForeColor == Color.Gray) { txtClienteNombre.Text = ""; txtClienteNombre.ForeColor = Color.Black; } };
            txtClienteNombre.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtClienteNombre.Text)) { txtClienteNombre.Text = "Nombre cliente"; txtClienteNombre.ForeColor = Color.Gray; } };

            txtClienteTelefono.Enter += (s, e) => { if (txtClienteTelefono.ForeColor == Color.Gray) { txtClienteTelefono.Text = ""; txtClienteTelefono.ForeColor = Color.Black; } };
            txtClienteTelefono.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtClienteTelefono.Text)) { txtClienteTelefono.Text = "Teléfono"; txtClienteTelefono.ForeColor = Color.Gray; } };

            txtClienteEmail.Enter += (s, e) => { if (txtClienteEmail.ForeColor == Color.Gray) { txtClienteEmail.Text = ""; txtClienteEmail.ForeColor = Color.Black; } };
            txtClienteEmail.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtClienteEmail.Text)) { txtClienteEmail.Text = "Email"; txtClienteEmail.ForeColor = Color.Gray; } };

            pnlTop.Controls.AddRange(new Control[] { lblProd, cboProducto, lblPrecio, lblStockTitle, lblStock, lblQty, nudCantidad, lblTotal, btnVender, btnVerDetalles, btnRefresh, btnEliminar, btnPrint, lblCliente, txtClienteNombre, txtClienteTelefono, txtClienteEmail });

            // Estilos de colores
            pnlTop.BackColor = Color.FromArgb(245, 248, 250);
            lblProd.ForeColor = Color.FromArgb(44, 62, 80);
            lblQty.ForeColor = Color.FromArgb(44, 62, 80);
            lblCliente.ForeColor = Color.FromArgb(44, 62, 80);
            lblPrecio.ForeColor = Color.FromArgb(33, 150, 243);
            lblTotal.ForeColor = Color.FromArgb(33, 150, 243);

            void StyleButton(Button b, Color bg, Color fg)
            {
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.BackColor = bg;
                b.ForeColor = fg;
            }

            StyleButton(btnVender, Color.FromArgb(40, 167, 69), Color.White);
            StyleButton(btnRefresh, Color.FromArgb(0, 123, 255), Color.White);
            StyleButton(btnEliminar, Color.FromArgb(220, 53, 69), Color.White);
            StyleButton(btnPrint, Color.FromArgb(102, 16, 242), Color.White);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = true,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                ColumnHeadersHeight = 30,
                RowTemplate = { Height = 26 },
                BorderStyle = BorderStyle.FixedSingle
            };

            // Ajustes visuales para el grid de ventas
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            // el doble clic para mostrar detalles se ha eliminado (se conserva el doble clic de celda para eliminar)
            // doble clic en celda: marcar en rojo y confirmar eliminación (comportamiento similar a Productos)
            dgv.CellDoubleClick += Dgv_CellDoubleClick;

            // DataGridView header styling (apply after instantiation to avoid null refs)
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Contenedor principal: añadir pnlTop Dock=Top y dgv Dock=Fill dentro de un panel para evitar solapamientos
            // Crear panel principal y panel de carrito debajo del DataGridView
            var mainPanel = new Panel { Dock = DockStyle.Fill };

            // Panel para carrito: fijo en la parte inferior con padding para evitar solapamiento
            var cartPanel = new Panel { Dock = DockStyle.Bottom, Height = 160, BackColor = Color.FromArgb(250, 250, 250), Padding = new Padding(6) };

            // DataGridView para el carrito
            dgvCart = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Tabla en memoria para el carrito
            cartTable = new DataTable();
            cartTable.Columns.Add("productoId", typeof(int));
            cartTable.Columns.Add("Producto", typeof(string));
            cartTable.Columns.Add("PrecioUnit", typeof(decimal));
            cartTable.Columns.Add("Cantidad", typeof(int));
            cartTable.Columns.Add("Total", typeof(decimal));

            dgvCart.DataSource = cartTable;

            // Nuevo estilo: barra inferior integrada en el área del carrito (no lateral, no flotante)
            var bottomToolbar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                BackColor = Color.FromArgb(245, 247, 249),
                Padding = new Padding(8)
            };

            // Ajustar lblCartTotal para quedar a la izquierda dentro de la barra
            // Inicializar lblCartTotal antes de usarlo (evita NullReference)
            lblCartTotal = new Label { Text = "Total carrito: 0.00" };
            lblCartTotal.AutoSize = false;
            lblCartTotal.Width = 260;
            lblCartTotal.Dock = DockStyle.Left;
            lblCartTotal.TextAlign = ContentAlignment.MiddleLeft;
            lblCartTotal.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCartTotal.ForeColor = Color.FromArgb(33, 37, 41);
            lblCartTotal.BackColor = Color.Transparent;
            lblCartTotal.Margin = new Padding(6, 18, 6, 18);

            // Panel para botones alineados a la derecha, horizontal
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(6),
                BackColor = Color.Transparent
            };

            // Botones principales alineados horizontalmente
            btnAgregarCarrito.AutoSize = false; btnAgregarCarrito.Width = 140; btnAgregarCarrito.Height = 40; btnAgregarCarrito.Margin = new Padding(6, 10, 6, 10);
            btnQuitarCarrito.AutoSize = false; btnQuitarCarrito.Width = 140; btnQuitarCarrito.Height = 40; btnQuitarCarrito.Margin = new Padding(6, 10, 6, 10);
            btnVenderCarrito.AutoSize = false; btnVenderCarrito.Width = 140; btnVenderCarrito.Height = 40; btnVenderCarrito.Margin = new Padding(6, 10, 6, 10);

            StyleButton(btnAgregarCarrito, Color.FromArgb(0, 123, 255), Color.White);
            StyleButton(btnQuitarCarrito, Color.FromArgb(220, 53, 69), Color.White);
            StyleButton(btnVenderCarrito, Color.FromArgb(40, 167, 69), Color.White);

            buttonsPanel.Controls.Add(btnAgregarCarrito);
            buttonsPanel.Controls.Add(btnQuitarCarrito);
            buttonsPanel.Controls.Add(btnVenderCarrito);

            bottomToolbar.Controls.Add(lblCartTotal);
            bottomToolbar.Controls.Add(buttonsPanel);

            // Añadir controles al panel del carrito: dgv arriba y barra inferior integrada
            cartPanel.Controls.Add(dgvCart);
            cartPanel.Controls.Add(bottomToolbar);

            // Actualizar total del carrito cuando cambie la tabla
            cartTable.RowChanged += (s, e) => { lblCartTotal.Text = $"Total carrito: {cartTable.AsEnumerable().Sum(r => r.Field<decimal>("Total")):F2}"; };
            // Añadir controles al panel principal en orden correcto para Dock: primero dgv (Fill), luego cartPanel (Bottom) y finalmente pnlTop (Top)
            mainPanel.Controls.Add(dgv);
            mainPanel.Controls.Add(cartPanel);
            mainPanel.Controls.Add(pnlTop);
            this.Controls.Add(mainPanel);
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

                var id = Convert.ToInt32(row.Cells["idVenta"].Value);
                var dr = SweetAlert.ShowConfirm(this, "Confirmar", "Eliminar venta seleccionada?");
                if (dr == DialogResult.Yes)
                {
                    Database.DeleteVenta(id);
                    LoadVentas();
                    SweetAlert.ShowSuccess(this, "Eliminado", "Venta eliminada correctamente.");
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

        private void ShowVentaDetallesFromSelection()
        {
            try
            {
                if (dgv.CurrentRow == null) return;
                var idObj = dgv.CurrentRow.Cells["idVenta"]?.Value ?? dgv.CurrentRow.Cells["id"]?.Value ?? dgv.CurrentRow.Cells[0]?.Value;
                if (idObj == null) return;
                var id = Convert.ToInt32(idObj);
                var detalles = Database.GetVentaDetalles(id);
                var ventaRow = Database.GetVentaById(id);
                var dlg = new VentaDetallesForm(detalles, ventaRow);
                dlg.ShowDialog(this);
            }
            catch (Exception ex)
            {
                SweetAlert.ShowError(this, "Error", ex.Message);
            }
        }

        private void LoadProductos()
        {
            try
            {
                try { Database.EnsureMigrationsApplied(); } catch { }
                var dt = Database.GetAllProducts();
                var tbl = new DataTable();
                tbl = dt.Copy();
                // create a simple list for combobox
                cboProducto.Items.Clear();
                foreach (DataRow r in tbl.Rows)
                {
                    var id = Convert.ToInt32(r["idProducto"]);
                    var name = r["producto"]?.ToString() ?? "";
                    var precio = r.Table.Columns.Contains("precio") && r["precio"] != DBNull.Value ? Convert.ToDecimal(r["precio"]) : 0m;
                    var stock = r.Table.Columns.Contains("cantidad") && r["cantidad"] != DBNull.Value ? Convert.ToInt32(r["cantidad"]) : 0;
                    cboProducto.Items.Add(new ComboboxItem { Text = name, Value = id, Price = precio, Stock = stock });
                }
                if (cboProducto.Items.Count > 0) cboProducto.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                SweetAlert.ShowError(this, "Error al cargar productos", ex.Message);
                cboProducto.Items.Clear();
            }
        }

        private void LoadVentas()
        {
            try
            {
                try { Database.EnsureMigrationsApplied(); } catch { }
                var dt = Database.GetAllVentas();
                // Convertir columnas de fecha (almacenadas en UTC) a hora de El Salvador para mostrar
                try
                {
                    if (dt != null && dt.Columns.Contains("fecha"))
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            if (r["fecha"] != DBNull.Value)
                            {
                                var d = Convert.ToDateTime(r["fecha"]);
                                r["fecha"] = Database.ConvertUtcToElSalvador(d);
                            }
                        }
                    }
                }
                catch { }
                dgv.DataSource = dt;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                // Ajustar encabezados y formatos de columnas (matching case-insensitive)
                try
                {
                    dgv.ColumnHeadersVisible = true;
                    foreach (DataGridViewColumn col in dgv.Columns)
                    {
                        var name = (col.Name ?? col.DataPropertyName ?? string.Empty).ToLowerInvariant();
                        switch (name)
                        {
                            case "idventa": col.HeaderText = "ID"; break;
                            case "fecha": col.HeaderText = "Fecha"; col.DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt"; break;
                            case "producto": col.HeaderText = "Producto"; break;
                            case "productoid": col.Visible = false; break;
                            case "cantidad": col.HeaderText = "Cantidad"; break;
                            case "total": col.HeaderText = "Total"; col.DefaultCellStyle.Format = "F2"; break;
                            case "clientenombre": col.HeaderText = "Cliente"; break;
                            case "clientetelefono": col.HeaderText = "Teléfono"; break;
                            case "clienteemail": col.HeaderText = "Email"; break;
                            default:
                                // si el nombre contiene 'nombre' y no está mapeado, renombrar prudente
                                if (name.Contains("nombre") && col.HeaderText == col.Name) col.HeaderText = "Nombre";
                                break;
                        }
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                SweetAlert.ShowError(this, "Error al cargar ventas", ex.Message);
                dgv.DataSource = null;
            }
        }

        private void UpdatePrecioYTotal()
        {
            try
            {
                if (cboProducto.SelectedItem is ComboboxItem it)
                {
                    var precio = it.Price;
                    lblPrecio.Text = $"Precio: {precio:F2}";
                    var total = precio * nudCantidad.Value;
                    lblTotal.Text = $"Total: {total:F2}";
                    // actualizar label de stock si existe
                    try { var lbl = this.Controls.Find("lblStock", true).FirstOrDefault() as Label; if (lbl != null) lbl.Text = it.Stock.ToString(); } catch { }
                }
            }
            catch { }
        }

        private void BtnVender_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!(cboProducto.SelectedItem is ComboboxItem it)) return;
                var productoId = Convert.ToInt32(it.Value);
                var precio = it.Price;
                var qty = Convert.ToInt32(nudCantidad.Value);
                var total = precio * qty;
                // Preparar datos del cliente: ignorar placeholders grises
                string? clienteNombre = null;
                string? clienteTelefono = null;
                string? clienteEmail = null;
                try
                {
                    if (txtClienteNombre != null && txtClienteNombre.ForeColor != Color.Gray)
                    {
                        var s = txtClienteNombre.Text?.Trim();
                        if (!string.IsNullOrEmpty(s)) clienteNombre = s;
                    }
                    if (txtClienteTelefono != null && txtClienteTelefono.ForeColor != Color.Gray)
                    {
                        var s = txtClienteTelefono.Text?.Trim();
                        if (!string.IsNullOrEmpty(s)) clienteTelefono = s;
                    }
                    if (txtClienteEmail != null && txtClienteEmail.ForeColor != Color.Gray)
                    {
                        var s = txtClienteEmail.Text?.Trim();
                        if (!string.IsNullOrEmpty(s)) clienteEmail = s;
                    }
                }
                catch { }

                // Validar stock en UI antes de intentar la inserción
                if (it.Stock < qty)
                {
                    SweetAlert.ShowError(this, "Stock insuficiente", $"Stock disponible: {it.Stock}. No puede vender {qty} unidades.");
                    return;
                }

                var newId = Database.InsertVenta(productoId, qty, total, clienteNombre, clienteTelefono, clienteEmail);
                SweetAlert.ShowSuccess(this, "Venta registrada", "La venta se guardó correctamente.");
                LoadVentas();
                LoadProductos();
                try { ShowPrintPreviewForSale(newId); } catch { }
                // Restaurar placeholders de cliente y resetear campos y carrito
                try
                {
                    if (txtClienteNombre != null) { txtClienteNombre.Text = "Nombre cliente"; txtClienteNombre.ForeColor = Color.Gray; }
                    if (txtClienteTelefono != null) { txtClienteTelefono.Text = "Teléfono"; txtClienteTelefono.ForeColor = Color.Gray; }
                    if (txtClienteEmail != null) { txtClienteEmail.Text = "Email"; txtClienteEmail.ForeColor = Color.Gray; }
                    if (nudCantidad != null) nudCantidad.Value = 1;
                    if (cboProducto != null && cboProducto.Items.Count > 0) cboProducto.SelectedIndex = 0;
                    lblTotal.Text = "Total: 0.00";
                    // limpiar carrito
                    try { cartTable?.Clear(); lblCartTotal.Text = "Total carrito: 0.00"; } catch { }
                }
                catch { }
            }
            catch (Exception ex)
            {
                SweetAlert.ShowError(this, "Error al registrar venta", ex.Message);
            }
        }

        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgv.CurrentRow == null) return;
                var id = Convert.ToInt32(dgv.CurrentRow.Cells["idVenta"].Value);
                var dr = SweetAlert.ShowConfirm(this, "Confirmar", "Eliminar venta seleccionada?");
                if (dr == DialogResult.Yes)
                {
                    Database.DeleteVenta(id);
                    LoadVentas();
                    SweetAlert.ShowSuccess(this, "Eliminado", "Venta eliminada correctamente.");
                }
            }
            catch (Exception ex)
            {
                SweetAlert.ShowError(this, "Error al eliminar", ex.Message);
            }
        }

        private class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public override string ToString() => Text;
        }
    }
}
