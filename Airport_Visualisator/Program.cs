using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Airport_Visualisator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Controller visualisator = new Controller();
            Application.Run(visualisator);

            //Form1 visualisator = new Form1();
            //Application.Run(visualisator);



        }

    }
}
