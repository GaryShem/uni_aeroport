using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Common;
using Newtonsoft.Json;

namespace CargoTruck
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CargoTruckService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CargoTruckService.svc or CargoTruckService.svc.cs at the Solution Explorer and start debugging.
    public class CargoTruckService : ICargoTruckService
    {
        public void CompleteMove(string id, int zoneNum)
        {
            Zone zone = (Zone)zoneNum;
            CargoTruckHandler._CargoTruck.CurrentZone = zone;
            CargoTruckHandler._CargoTruck.State = EntityState.WAITING_FOR_COMMAND;
        }

        public void AddAction(string flightId, int zoneNum, int planeServiceStageNum)
        {
            Zone zone = (Zone)zoneNum;
            PlaneServiceStage stage = (PlaneServiceStage) planeServiceStageNum;
            lock (CargoTruckHandler._CargoTruck.Commands)
            {
                CargoTruckHandler._CargoTruck.Commands.Add(new Tuple<string, Zone, PlaneServiceStage>(flightId, zone, stage));
            }
        }

        public string GetCargo()
        {
            return JsonConvert.SerializeObject(CargoTruckHandler._CargoTruck.CurrentCargo);
        }

        public void Start()
        {
            lock (CargoTruckHandler._CargoTruck.Commands)
            {
                int a = 3;
            }
        }
    }
}
