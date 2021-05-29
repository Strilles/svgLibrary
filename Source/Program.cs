using System;
using AutoUpdate;
using System.Windows.Forms;

namespace svgLibrary
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Updater.GitHubRepo = "/Strilles/svgLibrary";
            if (Updater.AutoUpdate())
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
