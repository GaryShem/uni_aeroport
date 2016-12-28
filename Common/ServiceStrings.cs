using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ServiceStrings
    {
        public static readonly string Plane = "http://localhost:" + Ports.Plane + "/PlaneService.svc";
        public static readonly string Passenger = "http://localhost:" + Ports.Passenger + "/PassengerService.svc";
        public static readonly string Bus = "http://localhost:" + Ports.Bus + "/BusService.svc";
        public static readonly string Fuel = "http://localhost:" + Ports.FuelTruck + "/FuelTruckService.svc";
        public static readonly string Cargo = "http://localhost:" + Ports.CargoTruck + "/CargoTruckService.svc";
        public static readonly string RegStand = "http://localhost:" + Ports.RegStand + "/RegistrationStandService.svc";
        public static readonly string GrControl = "http://localhost:" + Ports.GroundControl + "/GroundControlService.svc";
        public static readonly string Vis = "http://localhost:" + Ports.Visualizer + "/VisualizerService.svc";
    }
}
