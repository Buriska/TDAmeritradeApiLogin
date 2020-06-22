using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDAmeritradeApiLogin
{
    static class Program
    {
        public static Dashboard Dashboard;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DialogResult result;
            using (var login = new Login())
            {
                result = login.ShowDialog();
            }

            if (result == DialogResult.OK)
            {
                Dashboard = new Dashboard();
                Application.Run(Dashboard);
            }
        }
    }
}
