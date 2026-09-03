using System.Windows.Forms;

namespace Registro_Productos
{
    public class StockForm : Form
    {
        public StockForm()
        {
            this.Text = "Stock";
            this.Width = 600;
            this.Height = 400;
            var lbl = new Label() { Text = "Aquí se gestionará el stock.", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter };
            this.Controls.Add(lbl);
        }
    }
}
