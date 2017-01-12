using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

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

        private void button2_Click(object sender, EventArgs e)
        {
            string URL = String.Format("{0}/Start", ServiceStrings.Plane);
            Util.MakeRequest(URL);
            URL = String.Format("{0}/Start", ServiceStrings.Bus);
            Util.MakeRequest(URL);
            URL = String.Format("{0}/Start", ServiceStrings.Cargo);
            Util.MakeRequest(URL);
            URL = String.Format("{0}/Start", ServiceStrings.Fuel);
            Util.MakeRequest(URL);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form passList = new PassengerListForm();
            passList.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string passCountText = textBox1.Text;
            int passCount;
            if (int.TryParse(passCountText, out passCount) == false)
            {
                label1.Text = string.Format("Введите число от 0 до {0}", Plane.PassengerCapacity);
                return;
            }
            if (passCount < 0 || passCount > Common.Plane.PassengerCapacity)
            {
                label1.Text = string.Format("Введите число от 0 до {0}", Plane.PassengerCapacity);
                return;
            }
            string URL = string.Format("{0}/GeneratePlane?passengerCount={1}", ServiceStrings.Plane, passCount);
            Util.MakeRequest(URL);
        }
    }
}
