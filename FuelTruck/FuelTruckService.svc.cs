using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Common;
using Newtonsoft.Json;

namespace FuelTruck
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FuelTruckService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select FuelTruckService.svc or FuelTruckService.svc.cs at the Solution Explorer and start debugging.
    public class FuelTruckService : IFuelTruckService
    {
        public void CompleteMove(string id, int zoneNum)
        {
            Zone zone = (Zone) zoneNum;
            FuelTruckHandler._FuelTruck.CurrentZone = zone;
            FuelTruckHandler._FuelTruck.State = EntityState.WAITING_FOR_COMMAND;
        }

        public void AddAction(string flightId, int zoneNum)
        {
            Zone zone = (Zone) zoneNum;
            lock (FuelTruckHandler._FuelTruck.Commands)
            {
                FuelTruckHandler._FuelTruck.Commands.Add(new Tuple<string, Zone>(flightId, zone));
            }
        }

        public string GetCargo()
        {
            return JsonConvert.SerializeObject(FuelTruckHandler._FuelTruck.CurrentFuel);
        }

        public void Start()
        {
            lock (FuelTruckHandler._FuelTruck.Commands)
            {
                int a = 3;
            }
        }
    }
}
