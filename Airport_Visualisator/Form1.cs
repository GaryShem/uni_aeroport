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
        int x = 0;
        int y = 0; 
        int x2 = -140;
        int y2 = 210;
        //Thread Airport;
        private Graphics graphVisible;
        private BufferedGraphicsContext context;
        private BufferedGraphics graph;
        private Graphics graphNonVisible;
        private List<Tuple<int, Point>> planesList;
        private List<Tuple<int, Point>> vehicleList;
        private List<Tuple<int, Point>> passengerList;
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

        private void DrawEntity(Graphics g, Entity entity, Point point)
        {
            Image img;
            switch (entity)
            {
                case Entity.PLANE:
                    img = airplane;
                    g.DrawImage(img, new Point(point.X - img.Width / 2, point.Y - img.Height / 2));
                    break;
                case Entity.PASSENGER:
                    g.DrawEllipse(Pens.Black, point.X-1, point.Y-1, 3, 3);
                    break;
                case Entity.FUEL_TRUCK:
                    img = fuelcar2;
                    g.DrawImage(img, new Point(point.X - img.Width / 2, point.Y - img.Height / 2));
                    break;
                case Entity.CARGO_TRUCK:
//                    img = bus;
//                    g.DrawImage(img, new Point(point.X - img.Width / 2, point.Y - img.Height / 2));
                    g.DrawEllipse(Pens.Black, point.X - 4, point.Y - 4, 9, 9);
                    break;
                case Entity.BUS:
                    img = bus;
                    g.DrawImage(img, new Point(point.X - img.Width / 2, point.Y - img.Height / 2));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entity), entity, null);
            }
        }

        public void Network()
        {
            planesList = new List<Tuple<int, Point>>();
            vehicleList = new List<Tuple<int, Point>>();
            passengerList = new List<Tuple<int, Point>>();

            while (true)
            {
                string planeString =
                    Util.MakeRequest(String.Format("{0}/GetAllPlanes", ServiceStrings.Vis));
                    planesList = JsonConvert.DeserializeObject<List<Tuple<int, Point>>>(planeString);
                string vehicleString =
                    Util.MakeRequest(String.Format("{0}/GetAllVehicles", ServiceStrings.Vis));
                    vehicleList = JsonConvert.DeserializeObject<List<Tuple<int, Point>>>(vehicleString);
                string passengerString =
                    Util.MakeRequest(String.Format("{0}/GetAllPassengers", ServiceStrings.Vis));

                List<Tuple<int, Point>> bufferPlane = JsonConvert.DeserializeObject<List<Tuple<int, Point>>>(planeString);
                List<Tuple<int, Point>> bufferVehicle = JsonConvert.DeserializeObject<List<Tuple<int, Point>>>(vehicleString);
                List<Tuple<int, Point>> bufferPassenger = JsonConvert.DeserializeObject<List<Tuple<int, Point>>>(passengerString);

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
            //graphVisible = graphNonVisible;
            //Thread.Sleep(33);
            graph.Render(graphVisible);
            Invalidate();

            Thread.Sleep(33);
            //OnPaint(e);
            //Rectangle p = new Rectangle(170, 0, 1366, 768);
            //Invalidate(Rectangle.Inflate(p, 3, 3));
            
            //Update();

        }

        private void drawObjects(Graphics g)
        {
       
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, 1366, 768); 
            g.DrawRectangle(Pens.Black, new Rectangle(50, 110, 229, 269)); // регистрация
            g.DrawRectangle(Pens.Black, new Rectangle(50, 379, 229, 294)); // зона ожидания
            g.DrawRectangle(Pens.Black, new Rectangle(279, 443, 72, 166)); //автобусная
            g.DrawRectangle(Pens.Black, new Rectangle(192, 320, 87, 59)); // хуманы сдают чемоданы
            g.DrawRectangle(Pens.Black, new Rectangle(279, 301, 69, 79)); // багажная тележка

            g.DrawRectangle(Pens.Black, new Rectangle(599, 533, 228, 184)); // бензинная

            g.DrawRectangle(Pens.Black, new Rectangle(518, 78, 106, 109)); // ангар 1
            g.DrawRectangle(Pens.Black, new Rectangle(748, 224, 106, 109)); // ангар 2


            Point p1 = new Point(624, 79);
            Point p2 = new Point(1366, 109);
            g.DrawLine(Pens.Black, p1, p2); // полоса 1

            p1.X = 624;
            p1.Y = 187;
            p2.Y = 217;
            g.DrawLine(Pens.Black, p1, p2); // полоса 2

            p1.X = 852;
            p1.Y = 224;
            g.DrawLine(Pens.Black, p1, p2); // полоса 3

            p1.X = 853;
            p1.Y = 332;
            p2.X = 1366;
            p2.Y = 311;
            g.DrawLine(Pens.Black, p1, p2); // полоса 4

            g.DrawImage(bus, new PointF(x, y)); // весёлый автобус
            g.DrawImage(airplane, new PointF(x2, y2)); // и его друг самолёт
            g.DrawImage(fuelcar2, new PointF(x+50, y+100));

            g.DrawImage(human, new PointF(x2 + 50, y2 + 100));
            x -= 5;
            y += 0;
            x2 += 5;
            y2 += 0;

            //Реальные зоны:
            g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(60, 127, 204, 193)); // регистрация
            g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(192, 320, 39, 26)); // хуманы отдают багаж сюда

            g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(306, 317, 36, 42)); // забор багажа в регистрации
            g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(56, 419, 187, 219)); // ждуны

            g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(287, 486, 56, 78)); // автобусная
            g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(637, 533, 148, 134)); // бензинная

            g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(557, 120, 21, 24)); // ангар 1
            g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(788, 268, 20, 22)); // ангар 2

            g.DrawRectangle(Pens.Black, new Rectangle(-20, 175, 1, 134)); // спаун людей
            g.DrawRectangle(Pens.Black, new Rectangle(2004, 182, 61, 55));// спаун самолётов

            lock (planesList)
            {
                foreach (Tuple<int, Point> tuple in planesList)
                {
                    DrawEntity(g, (Entity) tuple.Item1, tuple.Item2);
                }
            }

            lock (passengerList)
            {
                foreach (Tuple<int, Point> tuple in passengerList)
                {
                    DrawEntity(g, (Entity)tuple.Item1, tuple.Item2);
                }
            }

            lock (vehicleList)
            {
                foreach (Tuple<int, Point> tuple in vehicleList)
                {
                    DrawEntity(g, (Entity)tuple.Item1, tuple.Item2);
                }
            }
        }



    }
    
}
