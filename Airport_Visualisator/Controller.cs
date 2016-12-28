using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Airport_Visualisator
{
    public partial class Controller : Form
    {
        public Controller()
        {
            InitializeComponent();
            //Form1 simulation = new Form1();
            //Application.Run(simulation);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            Form simulation = new Form1();
            simulation.Show();
        }
    }
}
