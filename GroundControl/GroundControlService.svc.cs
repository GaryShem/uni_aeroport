using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Common;
using Newtonsoft.Json;

namespace GroundControl
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GroundControlService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select GroundControlService.svc or GroundControlService.svc.cs at the Solution Explorer and start debugging.
    public class GroundControlService : IGroundControlService
    {
        private static Triple<string, PlaneServiceStage, Zone, int> h1stage = new Triple<string, PlaneServiceStage, Zone, int>("", PlaneServiceStage.NOT_IN_SERVICE, Zone.HANGAR_1, 0);
        private static Triple<string, PlaneServiceStage, Zone, int> h2stage = new Triple<string, PlaneServiceStage, Zone, int>("", PlaneServiceStage.NOT_IN_SERVICE, Zone.HANGAR_2, 0);

        private Triple<string, PlaneServiceStage, Zone, int> GetHangarInfo(string id, Zone zone)
        {
            switch (zone)
            {
                case Zone.HANGAR_1:
                    if (String.IsNullOrWhiteSpace(h1stage.Item1) == false && h1stage.Item1.Equals(id) == false)
                        throw new ArgumentOutOfRangeException(nameof(zone), zone, "There is a different plane in that zone");
                    return h1stage;
                case Zone.HANGAR_2:
                    if (String.IsNullOrWhiteSpace(h2stage.Item1) == false && h2stage.Item1.Equals(id) == false)
                        throw new ArgumentOutOfRangeException(nameof(zone), zone, "There is a different plane in that zone");
                    return h2stage;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zone), zone, "This zone is not a hangar");
            }
        }

        public void StartService(string id, int zoneNum)
        {
            Zone zone = (Zone)zoneNum;
            Triple<string, PlaneServiceStage, Zone, int> currentTriple = GetHangarInfo(id, zone);
            currentTriple.Item1 = id;
            currentTriple.Item2 = PlaneServiceStage.UNLOAD_PASSENGERS;
            string URL = String.Format("{0}/AddAction?flightId={1}&zone={2}&action={3}",
                ServiceStrings.Bus, id, zoneNum, (int)PlaneServiceStage.UNLOAD_PASSENGERS);
            Util.MakeRequest(URL);
        }

        public void FinishUnloadingPassengers(string id, int zoneNum)
        {
            Zone zone = (Zone) zoneNum;
            Triple<string, PlaneServiceStage, Zone, int> currentTriple = GetHangarInfo(id, zone);
            if (currentTriple.Item2 == PlaneServiceStage.UNLOAD_PASSENGERS)
            {
                currentTriple.Item2 = PlaneServiceStage.UNLOAD_CARGO;
                string URL = String.Format("{0}/AddAction?flightId={1}&zone={2}&action={3}",
                    ServiceStrings.Cargo, id, zoneNum, (int)PlaneServiceStage.UNLOAD_CARGO);
                Util.MakeRequest(URL);
            }
        }

        public void FinishUnloadingCargo(string id, int zoneNum)
        {
            Zone zone = (Zone)zoneNum;
            Triple<string, PlaneServiceStage, Zone, int> currentTriple = GetHangarInfo(id, zone);
            if (currentTriple.Item2 == PlaneServiceStage.UNLOAD_CARGO)
            {
                currentTriple.Item2 = PlaneServiceStage.REFUEL;
                string URL = String.Format("{0}/AddAction?flightId={1}&zone={2}",
                    ServiceStrings.Fuel, id, zoneNum);
                Util.MakeRequest(URL);
            }
        }

        public void FinishRefueling(string id, int zoneNum)
        {
            Zone zone = (Zone)zoneNum;
            Triple<string, PlaneServiceStage, Zone, int> currentTriple = GetHangarInfo(id, zone);
            if (currentTriple.Item2 == PlaneServiceStage.REFUEL)
            {
                currentTriple.Item2 = PlaneServiceStage.LOAD_CARGO;
                string URL = String.Format("{0}/AddAction?flightId={1}&zone={2}&action={3}",
                    ServiceStrings.Cargo, id, zoneNum, (int) PlaneServiceStage.LOAD_CARGO);
                Util.MakeRequest(URL);
            }
        }

        public void FinishLoadingCargo(string id, int zoneNum)
        {
            Zone zone = (Zone)zoneNum;
            Triple<string, PlaneServiceStage, Zone, int> currentTriple = GetHangarInfo(id, zone);
            if (currentTriple.Item2 == PlaneServiceStage.LOAD_CARGO)
            {
                currentTriple.Item2 = PlaneServiceStage.LOAD_PASSENGERS;
                string URL = String.Format("{0}/AddAction?flightId={1}&zone={2}&action={3}",
                    ServiceStrings.Bus, id, zoneNum, (int)PlaneServiceStage.LOAD_PASSENGERS);
                Util.MakeRequest(URL);
            }
        }

        public void FinishLoadingPassengers(string id, int zoneNum)
        {
            Zone zone = (Zone)zoneNum;
            Triple<string, PlaneServiceStage, Zone, int> currentTriple = GetHangarInfo(id, zone);
            if (currentTriple.Item2 == PlaneServiceStage.LOAD_PASSENGERS)
            {
                currentTriple.Item2 = PlaneServiceStage.TAKEOFF;
                string URL = String.Format("{0}/GoAway?id={1}", ServiceStrings.Plane, id);
                currentTriple.Item1 = "";
                Util.MakeRequest(URL);
            }
        }

        public string CheckStage(string id)
        {
            if (h1stage.Item1.Equals(id))
            {
                return JsonConvert.SerializeObject((int) h1stage.Item2);
            }
            else if (h2stage.Item1.Equals(id))
            {
                return JsonConvert.SerializeObject((int) h2stage.Item2);
            }
            return JsonConvert.SerializeObject((int) PlaneServiceStage.NOT_IN_SERVICE);
        }
    }
}
