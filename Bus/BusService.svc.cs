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
        private static int passengerTransferCount = 2;
        public string UnloadPassengers()
        {
            List<string> result = new List<string>();
            for (int i = 0; i < passengerTransferCount; i++)
            {
                if (Bus.Passengers.Count > 0)
                {
                    result.Add(Bus.Passengers[0]);
                    Bus.Passengers.RemoveAt(0);
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public void AddAction(string flightId, int zoneNum, int actionNum)
        {
            if ((Zone)zoneNum != Zone.HANGAR_1 && (Zone)zoneNum != Zone.HANGAR_2)
                throw new ArgumentOutOfRangeException(nameof(zoneNum));
            lock (Bus.Commands)
            {
                Bus.Commands.Add(new Tuple<string, Zone, PlaneServiceStage>(flightId, (Zone)zoneNum, (PlaneServiceStage)actionNum));
            }
        }
    }
}
