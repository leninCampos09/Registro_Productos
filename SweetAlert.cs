using System;
using System.Drawing;
using System.Windows.Forms;

namespace Registro_Productos
{
    public static class SweetAlert
    {
        private static Form CreateBase(Form owner, string title, string message, Color backColor)
        {
            var f = new Form();
            f.FormBorderStyle = FormBorderStyle.None;
            f.StartPosition = FormStartPosition.CenterParent;
            f.Width = 420;
            f.Height = 160;
            f.BackColor = backColor;
            f.ShowInTaskbar = false;
            f.TopMost = true;

            var lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.White, AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Top, Height = 36, Padding = new Padding(12,0,0,0) };
            var lblMsg = new Label { Text = message, Font = new Font("Segoe UI", 9), ForeColor = Color.WhiteSmoke, AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, Padding = new Padding(12) };

            var btnPanel = new Panel { Dock = DockStyle.Bottom, Height = 48, BackColor = Color.Transparent };

            f.Controls.Add(lblMsg);
            f.Controls.Add(lblTitle);
            f.Controls.Add(btnPanel);

            return f;
        }

        public static void ShowInfo(IWin32Window owner, string title, string message)
        {
            using var f = CreateBase(owner as Form, title, message, Color.FromArgb(52, 152, 219));
            var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90, Height = 30, BackColor = Color.White, ForeColor = Color.FromArgb(52,152,219), FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0;
            var panel = f.Controls.OfType<Panel>().FirstOrDefault();
            if (panel != null)
            {
                btnOk.Left = panel.ClientSize.Width - btnOk.Width - 16;
                btnOk.Top = (panel.ClientSize.Height - btnOk.Height) / 2;
                btnOk.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                panel.Controls.Add(btnOk);
            }
            else
            {
                btnOk.Left = f.ClientSize.Width - btnOk.Width - 16;
                btnOk.Top = f.ClientSize.Height - btnOk.Height - 18;
                btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                f.Controls.Add(btnOk);
            }
            f.AcceptButton = btnOk;
            f.ShowDialog(owner);
        }

        public static void ShowSuccess(IWin32Window owner, string title, string message)
        {
            using var f = CreateBase(owner as Form, title, message, Color.FromArgb(46, 204, 113));
            var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90, Height = 30, BackColor = Color.White, ForeColor = Color.FromArgb(46,204,113), FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0;
            var panel = f.Controls.OfType<Panel>().FirstOrDefault();
            if (panel != null)
            {
                btnOk.Left = panel.ClientSize.Width - btnOk.Width - 16;
                btnOk.Top = (panel.ClientSize.Height - btnOk.Height) / 2;
                btnOk.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                panel.Controls.Add(btnOk);
            }
            else
            {
                btnOk.Left = f.ClientSize.Width - btnOk.Width - 16;
                btnOk.Top = f.ClientSize.Height - btnOk.Height - 18;
                btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                f.Controls.Add(btnOk);
            }
            f.AcceptButton = btnOk;
            f.ShowDialog(owner);
        }

        public static void ShowError(IWin32Window owner, string title, string message)
        {
            using var f = CreateBase(owner as Form, title, message, Color.FromArgb(231, 76, 60));
            var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90, Height = 30, BackColor = Color.White, ForeColor = Color.FromArgb(231,76,60), FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0;
            var panel = f.Controls.OfType<Panel>().FirstOrDefault();
            if (panel != null)
            {
                btnOk.Left = panel.ClientSize.Width - btnOk.Width - 16;
                btnOk.Top = (panel.ClientSize.Height - btnOk.Height) / 2;
                btnOk.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                panel.Controls.Add(btnOk);
            }
            else
            {
                btnOk.Left = f.ClientSize.Width - btnOk.Width - 16;
                btnOk.Top = f.ClientSize.Height - btnOk.Height - 18;
                btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                f.Controls.Add(btnOk);
            }
            f.AcceptButton = btnOk;
            f.ShowDialog(owner);
        }

        public static DialogResult ShowConfirm(IWin32Window owner, string title, string message)
        {
            using var f = CreateBase(owner as Form, title, message, Color.FromArgb(52, 73, 94));
            var btnNo = new Button { Text = "No", DialogResult = DialogResult.No, Width = 90, Height = 30, BackColor = Color.White, ForeColor = Color.FromArgb(52,73,94), FlatStyle = FlatStyle.Flat };
            var btnYes = new Button { Text = "Sí", DialogResult = DialogResult.Yes, Width = 90, Height = 30, BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnNo.FlatAppearance.BorderSize = 0; btnYes.FlatAppearance.BorderSize = 0;
            var panel = f.Controls.OfType<Panel>().FirstOrDefault();
            if (panel != null)
            {
                btnYes.Left = panel.ClientSize.Width - btnYes.Width - 16;
                btnYes.Top = (panel.ClientSize.Height - btnYes.Height) / 2;
                btnYes.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                btnNo.Left = btnYes.Left - btnNo.Width - 8;
                btnNo.Top = btnYes.Top;
                btnNo.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                panel.Controls.Add(btnNo);
                panel.Controls.Add(btnYes);
            }
            else
            {
                btnYes.Left = f.ClientSize.Width - btnYes.Width - 16;
                btnYes.Top = f.ClientSize.Height - btnYes.Height - 18;
                btnYes.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                btnNo.Left = btnYes.Left - btnNo.Width - 8;
                btnNo.Top = btnYes.Top;
                btnNo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                f.Controls.Add(btnNo);
                f.Controls.Add(btnYes);
            }
            var dr = f.ShowDialog(owner);
            return dr;
        }
    }
}
