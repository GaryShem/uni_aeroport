using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using Newtonsoft.Json;

namespace Airport_Visualisator
{
    public partial class PassengerListForm : Form
    {
        private List<Passenger> _passengers;
        public PassengerListForm()
        {
            InitializeComponent();
            listView1.Columns.Add("id", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("flightid", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Location", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("RegState", 100, HorizontalAlignment.Left);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string URL = String.Format("{0}/GetAllPassengers", ServiceStrings.Passenger);
            string response = Util.MakeRequest(URL);
            _passengers = JsonConvert.DeserializeObject<List<Passenger>>(response);
            listView1.Items.Clear();
            foreach (Passenger passenger in _passengers)
            {
                string[] row1 = { passenger.FlightId.ToString().Substring(0,8), passenger.CurrentZone.ToString(), passenger.RegState.ToString()};
                listView1.Items.Add(passenger.Id.Substring(0,8)).SubItems.AddRange(row1);
//                ListViewItem lvi = new ListViewItem(new [] {passenger.Id.Substring(0,8), passenger.CurrentZone.ToString(), passenger.RegState.ToString()});
//                listView1.Items.Add(lvi);
            }
        }
    }
}
