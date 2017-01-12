using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Threading;
using Common;
using Newtonsoft.Json;

namespace Airport_Visualisator
{
    public partial class Form1 : Form
    {
        //Thread Airport;
        private Graphics graphVisible;
        private BufferedGraphicsContext context;
        private BufferedGraphics graph;
        private Graphics graphNonVisible;
        private List<Tuple<int, Point, int, int, int>> planesList;
        private List<Tuple<int, Point, int>> vehicleList;
        private List<Tuple<int, Point, int>> passengerList;
        Image backgroundImage = Image.FromFile("airport14.png");
        Image bus = Image.FromFile("bus.png");
        Image airplane = Image.FromFile("airplane.png");
        Image fuelcar2 = Image.FromFile("fuelcar2.png");
        Image human = Image.FromFile("human.png");
        private Thread networkThread;
        public Form1()
        {
            InitializeComponent();

            networkThread = new Thread(Network);
            networkThread.Start();

            this.DoubleBuffered = true;

            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            graph = context.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
            graphNonVisible = graph.Graphics;

            this.Paint += new PaintEventHandler(Form1_Paint);
            //AirportPaint();
            //Airport = new Thread(AirportPaint);
            //Airport.Start();

        }

        private void DrawPlane(Graphics g, Point point, int passengerCount, int cargoCount, int fuelCount) //рисуем самолёт и данные
        {
            Image img = airplane;
            g.DrawImage(img, new Point(point.X - img.Width / 2, point.Y - img.Height / 2)); //картинка с серединой на точке расположения самолёта
            g.DrawString(String.Format("Pass {0}\nCargo {1}\nFuel {2}", passengerCount, cargoCount, fuelCount),
                new Font("Arial", 10), new SolidBrush(Color.Black), point.X + img.Width / 2, point.Y - img.Height / 2); 
        }

        private void DrawEntity(Graphics g, Entity entity, Point point, int cargoCount) //отрисовка объектов
        {
            Image img;
            switch (entity)
            {
                case Entity.PASSENGER:
                    g.DrawEllipse(Pens.Black, point.X-1, point.Y-1, 3, 3);
                    break;
                case Entity.FUEL_TRUCK:
                    img = bus;
                    g.DrawImage(img, new Point(point.X - img.Width / 2, point.Y - img.Height / 2));
                    g.DrawString(String.Format("Fuel {0}", cargoCount),
                        new Font("Arial", 10), new SolidBrush(Color.Black), point.X - img.Width / 2, point.Y + img.Height / 2);
                    break;
                case Entity.CARGO_TRUCK:
                    img = bus;
                    g.DrawImage(img, new Point(point.X - img.Width / 2, point.Y - img.Height / 2));
                    g.DrawString(String.Format("Cargo {0}", cargoCount),
                        new Font("Arial", 10), new SolidBrush(Color.Black), point.X - img.Width / 2, point.Y + img.Height / 2);
                    break;
                case Entity.BUS:
                    img = bus;
                    g.DrawImage(img, new Point(point.X - img.Width / 2, point.Y - img.Height / 2));
                    g.DrawString(String.Format("Pass {0}", cargoCount),
                        new Font("Arial", 10), new SolidBrush(Color.Black), point.X - img.Width / 2, point.Y + img.Height / 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entity), entity, null);
            }
        }

        public void Network() //связывается с первой частью визуализатара (логической)
        {
            planesList = new List<Tuple<int, Point, int, int, int>>();
            vehicleList = new List<Tuple<int, Point, int>>();
            passengerList = new List<Tuple<int, Point, int>>();

            while (true)
            {
                string planeString = Util.MakeRequest(String.Format("{0}/GetAllPlanes", ServiceStrings.Vis)); //запрашивает самолёты
//                planesList = JsonConvert.DeserializeObject<List<Tuple<int, Point, int, int, int>>>(planeString);
                string vehicleString = Util.MakeRequest(String.Format("{0}/GetAllVehicles", ServiceStrings.Vis)); //запрашивает машинки
//                vehicleList = JsonConvert.DeserializeObject<List<Tuple<int, Point, int>>>(vehicleString);
                string passengerString = Util.MakeRequest(String.Format("{0}/GetAllPassengers", ServiceStrings.Vis)); //запрашивает пассажиров
                
                //тип, координаты, пассажиры, груз, топливо
                List<Tuple<int, Point, int, int, int>> bufferPlane = JsonConvert.DeserializeObject<List<Tuple<int, Point, int, int, int>>>(planeString);
                //тип, координаты, груз/топливо/кол-во людей
                List<Tuple<int, Point, int>> bufferVehicle = JsonConvert.DeserializeObject<List<Tuple<int, Point, int>>>(vehicleString);
                //тип, координаты, груз
                List<Tuple<int, Point, int>> bufferPassenger = JsonConvert.DeserializeObject<List<Tuple<int, Point, int>>>(passengerString);

                //обновление списков
                lock (passengerList)
                {
                    passengerList = bufferPassenger;
                }
                lock (vehicleList)
                {
                    vehicleList = bufferVehicle;
                }
                lock (planesList)
                {
                    planesList = bufferPlane;
                }
            }
        }

        public void AirportPaint()
        {
            //this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(Form1_Paint);
        }

        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {   
            graphVisible = e.Graphics;
            drawObjects(graphNonVisible);
            graph.Render(graphVisible);
            Invalidate();

            Thread.Sleep(33);
            //Rectangle p = new Rectangle(170, 0, 1366, 768);
        }

        private void drawObjects(Graphics g)
        {
            g.DrawImage(backgroundImage, 0, 0, 1366, 767); //отрисовка заднего фона

            lock (planesList) 
            {
                //отрисовка самолётов
                foreach (Tuple<int, Point, int, int, int> tuple in planesList)
                {
                    DrawPlane(g, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);
//                    DrawEntity(g, (Entity) tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
                }
            }

            lock (passengerList)
            {
                //отрисовка пассажиров
                foreach (Tuple<int, Point, int> tuple in passengerList)
                {
                    DrawEntity(g, (Entity)tuple.Item1, tuple.Item2, tuple.Item3);
                }
            }

            lock (vehicleList)
            {
                //отрисовка машинок
                foreach (Tuple<int, Point, int> tuple in vehicleList)
                {
                    DrawEntity(g, (Entity)tuple.Item1, tuple.Item2, tuple.Item3);
                }
            }
        }
    }
}
