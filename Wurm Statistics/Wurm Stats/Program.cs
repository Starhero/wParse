using System;
using System.Windows.Forms;
using EWOS;

namespace EWOS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            EWOS.SplashScreen.SplashScreen.ShowSplashScreen();
            SplashScreen.SplashScreen.SetStatus("Starting up....");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.DoEvents();
            Application.Run(new FormMain());

            //TODO: Make sure what ever MIGHT be open now, close before things die.
        }
    }
}
