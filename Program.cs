using System;
using System.Windows.Forms;

namespace Plugin_ICGFront
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            var eventName = ParseArgument(args);

            if (eventName == null) return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main(eventName));
        }

        private static string ParseArgument(string[] args)
        {
            if (args.Length == 0 || args.Length > 1) return null;

            return args[0];
        }
    }
}