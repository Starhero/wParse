using System;
using System.Diagnostics;
using System.Windows.Forms;
using EWOS;
using Hero;


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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            //So the program has been started by some noob in his shack.
            //Que SplashScreen!
            Hero.SplashScreen.ShowSplashScreen();
            sw.Stop();
            Hero.SplashScreen.SetStatus("Starting up....");
            double ms = (sw.ElapsedTicks * 1000.0) / Stopwatch.Frequency;
            System.Diagnostics.Trace.WriteLine(ms.ToString());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.DoEvents();
            Application.Run(new FormMain());

            //TODO: Make sure what ever MIGHT be open now, close before things die.
        }
    }
}