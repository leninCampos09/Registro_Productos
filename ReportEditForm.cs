using System;
using System.Drawing;
using System.Windows.Forms;

namespace Registro_Productos
{
    public class ReportEditForm : Form
    {
        private DateTimePicker dtpInicio;
        private DateTimePicker dtpFin;
        private TextBox txtReportData;
        private Button btnOk;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        [System.ComponentModel.Browsable(false)]
        public DateTime? PeriodoInicio { get; set; }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        [System.ComponentModel.Browsable(false)]
        public DateTime? PeriodoFin { get; set; }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        [System.ComponentModel.Browsable(false)]
        public string ReportData { get; set; }

        public ReportEditForm(string tipo)
        {
            this.Text = tipo == "Venta" ? "Nuevo informe de ventas" : "Nuevo informe de compras";
            this.Width = 500;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterParent;

            var lbl1 = new Label { Text = "Periodo inicio:", Left = 10, Top = 20, AutoSize = true };
            dtpInicio = new DateTimePicker { Left = 110, Top = 16, Width = 120, Format = DateTimePickerFormat.Short };
            var lbl2 = new Label { Text = "Periodo fin:", Left = 250, Top = 20, AutoSize = true };
            dtpFin = new DateTimePicker { Left = 320, Top = 16, Width = 120, Format = DateTimePickerFormat.Short };

            var lbl3 = new Label { Text = "Datos (JSON opcional):", Left = 10, Top = 56, AutoSize = true };
            txtReportData = new TextBox { Left = 10, Top = 76, Width = 440, Height = 120, Multiline = true, ScrollBars = ScrollBars.Vertical };

            btnOk = new Button { Text = "Aceptar", Left = 360, Top = 210 };
            btnOk.Click += (s, e) =>
            {
                var inicio = dtpInicio.Value.Date;
                var fin = dtpFin.Value.Date.AddDays(1).AddTicks(-1);
                // validar que no sean fechas futuras
                if (inicio.Date > DateTime.Now.Date)
                {
                    SweetAlert.ShowError(this, "Fecha inválida", "La fecha de inicio no puede ser en el futuro.");
                    return;
                }
                if (fin > DateTime.Now)
                {
                    SweetAlert.ShowError(this, "Fecha inválida", "La fecha final no puede ser en el futuro.");
                    return;
                }
                if (inicio > fin)
                {
                    SweetAlert.ShowError(this, "Fechas inválidas", "La fecha de inicio debe ser anterior o igual a la fecha final.");
                    return;
                }
                PeriodoInicio = inicio;
                PeriodoFin = fin;
                ReportData = txtReportData.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(lbl1);
            this.Controls.Add(dtpInicio);
            this.Controls.Add(lbl2);
            this.Controls.Add(dtpFin);
            this.Controls.Add(lbl3);
            this.Controls.Add(txtReportData);
            this.Controls.Add(btnOk);

            this.AcceptButton = btnOk;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Aplicar valores iniciales si el llamador los estableció después del constructor
            try
            {
                if (PeriodoInicio.HasValue)
                {
                    dtpInicio.Value = PeriodoInicio.Value;
                }
                if (PeriodoFin.HasValue)
                {
                    // Mostrar la fecha final como fecha (sin time)
                    dtpFin.Value = PeriodoFin.Value.Date;
                }
                if (!string.IsNullOrEmpty(ReportData))
                {
                    txtReportData.Text = ReportData;
                }
            }
            catch
            {
                // ignorar errores de asignación de fechas
            }
        }
    }
}
