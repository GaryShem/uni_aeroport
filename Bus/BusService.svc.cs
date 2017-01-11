using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.UI.WebControls;
using Common;
using Newtonsoft.Json;

namespace Bus
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BusService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select BusService.svc or BusService.svc.cs at the Solution Explorer and start debugging.
    public class BusService : IBusService
    {
        public string UnloadPassengers()
        {
            List<string> result = new List<string>();
            for (int i = 0; i < Common.Bus.TAKE_PASSENGERS; i++)
            {
                if (BusHandler._bus.Passengers.Count > 0)
                {
                    result.Add(BusHandler._bus.Passengers[0]);
                    BusHandler._bus.Passengers.RemoveAt(0);
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public void AddAction(string flightId, int zoneNum, int actionNum)
        {
            if ((Zone)zoneNum != Zone.HANGAR_1 && (Zone)zoneNum != Zone.HANGAR_2)
                throw new ArgumentOutOfRangeException(nameof(zoneNum));
            lock (BusHandler._bus.Commands)
            {
                BusHandler._bus.Commands.Add(new Tuple<string, Zone, PlaneServiceStage>(flightId, (Zone)zoneNum, (PlaneServiceStage)actionNum));
            }
        }

        public void CompleteMove(string id, int zoneNum)
        {
            Zone zone = (Zone) zoneNum;
            BusHandler._bus.CurrentZone = zone;
            BusHandler._bus.State = EntityState.WAITING_FOR_COMMAND;
        }

        public void Start()
        {
            lock (BusHandler._bus.Commands)
            {
                int a = 0;
            }
        }
    }
}
