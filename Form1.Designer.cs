namespace Registro_Productos
{
    partial class frmPrincipal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.lbProducto = new System.Windows.Forms.Label();
            this.txtProducto = new System.Windows.Forms.TextBox();
            this.lbDescripcion = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lbPrecio = new System.Windows.Forms.Label();
            this.txtPrecio = new System.Windows.Forms.TextBox();
            this.lbCantidad = new System.Windows.Forms.Label();
            this.txtCantidad = new System.Windows.Forms.TextBox();
            this.lbDisponible = new System.Windows.Forms.Label();
            this.rbDisponible = new System.Windows.Forms.RadioButton();
            this.flpImage = new System.Windows.Forms.FlowLayoutPanel();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tlpMain.RowCount = 8;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(10);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.Padding = new System.Windows.Forms.Padding(10);
            this.tlpMain.Size = new System.Drawing.Size(800, 450);
            this.tlpMain.TabIndex = 0;
            //
            // tabMain
            //
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPageProduct = new System.Windows.Forms.TabPage();
            this.tabPageAdmin = new System.Windows.Forms.TabPage();
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(800, 450);
            this.tabMain.TabIndex = 99;
            this.tabPageProduct.Text = "Producto";
            this.tabPageProduct.UseVisualStyleBackColor = true;
            this.tabPageAdmin.Text = "Administración";
            this.tabPageAdmin.UseVisualStyleBackColor = true;
            this.tabMain.Controls.Add(this.tabPageProduct);
            this.tabMain.Controls.Add(this.tabPageAdmin);
            // 
            // lbProducto
            // 
            this.lbProducto.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbProducto.AutoSize = true;
            this.lbProducto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lbProducto.Location = new System.Drawing.Point(13, 25);
            this.lbProducto.Name = "lbProducto";
            this.lbProducto.Size = new System.Drawing.Size(60, 15);
            this.lbProducto.TabIndex = 0;
            this.lbProducto.Text = "Producto";
            // 
            // txtProducto
            // 
            this.txtProducto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProducto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.txtProducto.Location = new System.Drawing.Point(294, 22);
            this.txtProducto.Name = "txtProducto";
            this.txtProducto.Size = new System.Drawing.Size(493, 23);
            this.txtProducto.TabIndex = 1;
            // 
            // lbDescripcion
            // 
            this.lbDescripcion.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbDescripcion.AutoSize = true;
            this.lbDescripcion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lbDescripcion.Location = new System.Drawing.Point(13, 70);
            this.lbDescripcion.Name = "lbDescripcion";
            this.lbDescripcion.Size = new System.Drawing.Size(70, 15);
            this.lbDescripcion.TabIndex = 2;
            this.lbDescripcion.Text = "Descripción";
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescripcion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.txtDescripcion.Location = new System.Drawing.Point(294, 60);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(493, 60);
            this.txtDescripcion.TabIndex = 3;
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.AcceptsReturn = true;
            this.txtDescripcion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            // 
            // lbCategoria
            // 
            this.lbCategoria = new System.Windows.Forms.Label();
            this.lbCategoria.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbCategoria.AutoSize = true;
            this.lbCategoria.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lbCategoria.Location = new System.Drawing.Point(13, 98);
            this.lbCategoria.Name = "lbCategoria";
            this.lbCategoria.Size = new System.Drawing.Size(57, 15);
            this.lbCategoria.TabIndex = 3;
            this.lbCategoria.Text = "Categoría";

            // 
            // cboCategoria
            // 
            this.cboCategoria = new System.Windows.Forms.ComboBox();
            this.cboCategoria.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCategoria.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboCategoria.Location = new System.Drawing.Point(294, 95);
            this.cboCategoria.Name = "cboCategoria";
            this.cboCategoria.Size = new System.Drawing.Size(493, 23);
            this.cboCategoria.TabIndex = 4;
            // 
            // lbPrecio
            // 
            this.lbPrecio.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbPrecio.AutoSize = true;
            this.lbPrecio.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lbPrecio.Location = new System.Drawing.Point(13, 115);
            this.lbPrecio.Name = "lbPrecio";
            this.lbPrecio.Size = new System.Drawing.Size(43, 15);
            this.lbPrecio.TabIndex = 4;
            this.lbPrecio.Text = "Precio";
            // 
            // txtPrecio
            // 
            this.txtPrecio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrecio.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.txtPrecio.Location = new System.Drawing.Point(294, 112);
            this.txtPrecio.Name = "txtPrecio";
            this.txtPrecio.Size = new System.Drawing.Size(493, 23);
            this.txtPrecio.TabIndex = 5;
            // 
            // lbCantidad
            // 
            this.lbCantidad.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbCantidad.AutoSize = true;
            this.lbCantidad.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lbCantidad.Location = new System.Drawing.Point(13, 205);
            this.lbCantidad.Name = "lbCantidad";
            this.lbCantidad.Size = new System.Drawing.Size(58, 15);
            this.lbCantidad.TabIndex = 6;
            this.lbCantidad.Text = "Cantidad";
            // 
            // txtCantidad
            // 
            this.txtCantidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCantidad.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.txtCantidad.Location = new System.Drawing.Point(294, 202);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.Size = new System.Drawing.Size(493, 23);
            this.txtCantidad.TabIndex = 7;
            // 
            // lbDisponible
            // 
            this.lbDisponible.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbDisponible.AutoSize = true;
            this.lbDisponible.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lbDisponible.Location = new System.Drawing.Point(13, 295);
            this.lbDisponible.Name = "lbDisponible";
            this.lbDisponible.Size = new System.Drawing.Size(69, 15);
            this.lbDisponible.TabIndex = 8;
            this.lbDisponible.Text = "Disponible";
            // 
            // rbDisponible
            // 
            this.rbDisponible.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rbDisponible.AutoSize = true;
            this.rbDisponible.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.rbDisponible.Location = new System.Drawing.Point(294, 293);
            this.rbDisponible.Name = "rbDisponible";
            this.rbDisponible.Size = new System.Drawing.Size(104, 19);
            this.rbDisponible.TabIndex = 9;
            this.rbDisponible.TabStop = true;
            this.rbDisponible.Text = "Disponible";
            this.rbDisponible.UseVisualStyleBackColor = true;
            // 
            // flpImage
            // 
            this.flpImage.AutoSize = true;
            this.flpImage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpImage.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpImage.Location = new System.Drawing.Point(294, 260);
            this.flpImage.Name = "flpImage";
            this.flpImage.Size = new System.Drawing.Size(200, 120);
            this.flpImage.TabIndex = 10;
            // 
            // pbImage
            // 
            this.pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbImage.Size = new System.Drawing.Size(160, 90);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            // 

            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(350, 360);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 30);
            this.btnGuardar.TabIndex = 13;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            // 
            // frmPrincipal
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            // add the table layout into the product tab
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPageProduct.Controls.Add(this.tlpMain);
            this.Controls.Add(this.tabMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.MinimumSize = new System.Drawing.Size(600, 360);
            this.Name = "frmPrincipal";
            this.Text = "Registro de Productos";
            // Add controls to table layout
            this.tlpMain.Controls.Add(this.lbProducto, 0, 0);
            this.tlpMain.Controls.Add(this.txtProducto, 1, 0);
            this.tlpMain.Controls.Add(this.lbDescripcion, 0, 1);
            this.tlpMain.Controls.Add(this.txtDescripcion, 1, 1);
            this.tlpMain.Controls.Add(this.lbCategoria, 0, 2);
            this.tlpMain.Controls.Add(this.cboCategoria, 1, 2);
            this.tlpMain.Controls.Add(this.lbPrecio, 0, 3);
            this.tlpMain.Controls.Add(this.txtPrecio, 1, 3);
            this.tlpMain.Controls.Add(this.lbCantidad, 0, 3);
            this.tlpMain.Controls.Add(this.txtCantidad, 1, 3);
            this.tlpMain.Controls.Add(this.lbDisponible, 0, 4);
            this.tlpMain.Controls.Add(this.rbDisponible, 1, 4);
            // Add image controls into a flow panel and then to layout
            this.flpImage.Controls.Add(this.pbImage);
            this.tlpMain.Controls.Add(this.flpImage, 1, 5);
            // Add guardar button below layout and a botón para ver productos
            this.pnlActions = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlActions.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlActions.WrapContents = false;
            this.pnlActions.AutoSize = true;

            this.btnVer = new System.Windows.Forms.Button();
            this.btnVer.Name = "btnVer";
            this.btnVer.Size = new System.Drawing.Size(100, 30);
            this.btnVer.Text = "Ver productos";
            this.btnVer.UseVisualStyleBackColor = true;
            this.btnVer.Visible = false;

            this.btnCategorias = new System.Windows.Forms.Button();
            this.btnCategorias.Name = "btnCategorias";
            this.btnCategorias.Size = new System.Drawing.Size(120, 30);
            this.btnCategorias.Text = "Administrar categorías";
            this.btnCategorias.UseVisualStyleBackColor = true;
            this.btnCategorias.Visible = false;

            this.pnlActions.Controls.Add(this.btnVer);
            this.pnlActions.Controls.Add(this.btnCategorias);

            this.tlpMain.Controls.Add(this.pnlActions, 0, 6);
            this.tlpMain.Controls.Add(this.btnGuardar, 1, 6);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPageProduct;
        private System.Windows.Forms.TabPage tabPageAdmin;
        private System.Windows.Forms.Label lbProducto;
        private System.Windows.Forms.TextBox txtProducto;
        private System.Windows.Forms.Label lbPrecio;
        private System.Windows.Forms.TextBox txtPrecio;
        private System.Windows.Forms.Label lbCantidad;
        private System.Windows.Forms.TextBox txtCantidad;
        private System.Windows.Forms.Label lbDisponible;
        private System.Windows.Forms.RadioButton rbDisponible;
        private System.Windows.Forms.Label lbDescripcion;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lbCategoria;
        private System.Windows.Forms.ComboBox cboCategoria;
        private System.Windows.Forms.FlowLayoutPanel flpImage;
        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.FlowLayoutPanel pnlActions;
        private System.Windows.Forms.Button btnCategorias;
        private System.Windows.Forms.Button btnVer;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.ErrorProvider errorProvider1;

        #endregion
    }
}
