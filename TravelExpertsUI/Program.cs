using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravelExpertsUI.MenuForms;
namespace TravelExpertsUI
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmMainMenu());
            Application.Run(new frmLoginForm());
            //Application.Run(new frmMenuPackage());
            //Application.Run(new frmMenuProduct());
            //Application.Run(new frmMenuSupplier());
        }
    }
}
