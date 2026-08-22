namespace Registro_Productos
{
    partial class ProductoViewForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlpInfo = new System.Windows.Forms.TableLayoutPanel();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblPrecio = new System.Windows.Forms.Label();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.lblDisponible = new System.Windows.Forms.Label();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.lblImgStatus = new System.Windows.Forms.Label();
            this.tlpMain.SuspendLayout();
            this.tlpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 1;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(470, 230);
            this.tlpMain.TabIndex = 0;
            // 
            // pnlInfo
            // 
            // tlpInfo (left column) - organiza nombre + espacio + otros datos
            this.tlpInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpInfo.Padding = new System.Windows.Forms.Padding(12);
            this.tlpInfo.ColumnCount = 1;
            this.tlpInfo.RowCount = 7;
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // nombre
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F)); // espaciador
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // descripcion
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // categoria
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // precio
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // cantidad
            this.tlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // disponible
            this.tlpInfo.Controls.Add(this.lblNombre, 0, 0);
            // spacer: empty panel
            var spacer = new System.Windows.Forms.Panel();
            spacer.Size = new System.Drawing.Size(10, 8);
            this.tlpInfo.Controls.Add(spacer, 0, 1);
            // descripcion label
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Margin = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.lblDescripcion.ForeColor = System.Drawing.Color.DimGray;
            this.tlpInfo.Controls.Add(this.lblDescripcion, 0, 2);
            // categoria label (se mostrará en verde)
            this.lblCategoria = new System.Windows.Forms.Label();
            this.lblCategoria.AutoSize = true;
            this.lblCategoria.Name = "lblCategoria";
            this.lblCategoria.Margin = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.lblCategoria.ForeColor = System.Drawing.Color.Green;
            this.tlpInfo.Controls.Add(this.lblCategoria, 0, 3);
            this.tlpInfo.Controls.Add(this.lblPrecio, 0, 4);
            this.tlpInfo.Controls.Add(this.lblCantidad, 0, 5);
            this.tlpInfo.Controls.Add(this.lblDisponible, 0, 6);
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.MaximumSize = new System.Drawing.Size(360, 0);
            this.lblNombre.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(88)))), ((int)(((byte)(88)))));
            this.lblNombre.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            // 
            // lblPrecio
            // 
            this.lblPrecio.AutoSize = true;
            this.lblPrecio.Name = "lblPrecio";
            this.lblPrecio.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            // 
            // lblCantidad
            // 
            this.lblCantidad.AutoSize = true;
            this.lblCantidad.Name = "lblCantidad";
            this.lblCantidad.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            // 
            // lblDisponible
            // 
            this.lblDisponible.AutoSize = true;
            this.lblDisponible.Name = "lblDisponible";
            this.lblDisponible.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            // 
            // pbImage
            // 
            this.pbImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbImage.Margin = new System.Windows.Forms.Padding(12);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(216, 206);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // 
            // lblImgStatus
            // 
            this.lblImgStatus.AutoSize = true;
            this.lblImgStatus.Location = new System.Drawing.Point(12, 115);
            this.lblImgStatus.Name = "lblImgStatus";
            this.lblImgStatus.Size = new System.Drawing.Size(0, 15);
            this.lblImgStatus.Visible = false;
            // 
            // ProductoViewForm
            // 
            this.ClientSize = new System.Drawing.Size(470, 230);
            this.Controls.Add(this.tlpMain);
            this.tlpMain.Controls.Add(this.tlpInfo, 0, 0);
            this.tlpMain.Controls.Add(this.pbImage, 1, 0);
            this.Name = "ProductoViewForm";
            this.Text = "Ver producto";
            this.tlpMain.ResumeLayout(false);
            this.tlpInfo.ResumeLayout(false);
            this.tlpInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpInfo;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblPrecio;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.Label lblDisponible;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.Label lblCategoria;
        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.Label lblImgStatus;
    }
}
