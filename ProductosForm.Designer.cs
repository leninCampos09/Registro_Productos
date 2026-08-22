namespace Registro_Productos
{
    partial class ProductosForm
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
            this.dgvProducts = new System.Windows.Forms.DataGridView();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblCategoriaFilter = new System.Windows.Forms.Label();
            this.cboCategoriaFilter = new System.Windows.Forms.ComboBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlTop = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // pnlTop
            //
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height = 50;
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.pnlTop.WrapContents = false;
            this.pnlTop.Padding = new System.Windows.Forms.Padding(8);
            this.pnlTop.AutoSize = false;

            // lblCategoriaFilter
            // 
            this.lblCategoriaFilter.AutoSize = true;
            this.lblCategoriaFilter.Name = "lblCategoriaFilter";
            this.lblCategoriaFilter.Size = new System.Drawing.Size(60, 15);
            this.lblCategoriaFilter.TabIndex = 3;
            this.lblCategoriaFilter.Text = "Categoría:";
            this.lblCategoriaFilter.Margin = new System.Windows.Forms.Padding(12, 14, 6, 10);

            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(44, 15);
            this.lblSearch.TabIndex = 5;
            this.lblSearch.Text = "Buscar:";
            this.lblSearch.Margin = new System.Windows.Forms.Padding(12, 14, 6, 10);

            // txtSearch
            //
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(180, 23);
            this.txtSearch.TabIndex = 6;
            this.txtSearch.Margin = new System.Windows.Forms.Padding(6, 10, 12, 10);

            // cboCategoriaFilter
            // 
            this.cboCategoriaFilter.Name = "cboCategoriaFilter";
            this.cboCategoriaFilter.Size = new System.Drawing.Size(220, 23);
            this.cboCategoriaFilter.TabIndex = 4;

            // dgvProducts
            // 
            this.dgvProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProducts.Name = "dgvProducts";

            this.dgvProducts.ReadOnly = false;
            this.dgvProducts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProducts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            // 
            // btnNuevo
            // 
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(100, 30);
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Margin = new System.Windows.Forms.Padding(8, 10, 8, 10);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.Text = "Actualizar";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(8, 10, 8, 10);
            // 
            // ProductosForm
            // 
            this.ClientSize = new System.Drawing.Size(784, 461);
            // add controls to top panel
            this.pnlTop.Controls.Add(this.btnNuevo);
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Controls.Add(this.lblCategoriaFilter);
            this.pnlTop.Controls.Add(this.cboCategoriaFilter);
            this.pnlTop.Controls.Add(this.lblSearch);
            this.pnlTop.Controls.Add(this.txtSearch);

            this.Controls.Add(this.dgvProducts);
            this.Controls.Add(this.pnlTop);
            this.Name = "ProductosForm";
            this.Text = "Productos registrados";
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblCategoriaFilter;
        private System.Windows.Forms.ComboBox cboCategoriaFilter;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.FlowLayoutPanel pnlTop;
    }
}
