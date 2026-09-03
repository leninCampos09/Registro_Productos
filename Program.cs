namespace Registro_Productos
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            // Aplicar migraciones SQL encontradas en la carpeta Migrations antes de iniciar la UI
            try
            {
                Database.EnsureMigrationsApplied();
            }
            catch (Exception ex)
            {
                // Mostrar error y permitir al usuario decidir si continuar
                var res = System.Windows.Forms.MessageBox.Show("Error aplicando migraciones: " + ex.Message + "\n\nContinuar de todos modos?", "Migraciones", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning);
                if (res == System.Windows.Forms.DialogResult.No) return;
            }

            Application.Run(new frmPrincipal());
        }
    }
}