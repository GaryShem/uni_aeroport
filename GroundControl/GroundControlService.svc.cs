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
        public void StartService(string id, int zoneNum)
        {
            Zone zone = (Zone)zoneNum;
            switch (zone)
            {
                case Zone.HANGAR_1:
                    h1stage.Item2 = PlaneServiceStage.UNLOAD_PASSENGERS;
                    h1stage.Item1 = id;
                    break;
                case Zone.HANGAR_2:
                    h2stage.Item2 = PlaneServiceStage.UNLOAD_PASSENGERS;
                    h2stage.Item1 = id;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
            }
            string URL = String.Format("{0}/AddAction?flightId={1}&zone={2}&action={3}",
                ServiceStrings.Bus, id, zoneNum, (int)PlaneServiceStage.UNLOAD_PASSENGERS);
            Util.MakeRequest(URL);
        }

        public void FinishUnloadingPassengers(string id, int zoneNum)
        {
            Zone zone = (Zone) zoneNum;
            switch (zone)
            {
                case Zone.HANGAR_1:
                    if (h1stage.Item1.Equals(id) == false)
                        throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
                    //                    h1stage.Item2 = PlaneServiceStage.UNLOAD_CARGO;
                    h1stage.Item2 = PlaneServiceStage.LOAD_PASSENGERS;
                    break;
                case Zone.HANGAR_2:
                    if (h2stage.Item1.Equals(id) == false)
                        throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
                    //                    h2stage.Item2 = PlaneServiceStage.UNLOAD_CARGO;
                    h2stage.Item2 = PlaneServiceStage.LOAD_PASSENGERS;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
            }
            string URL = String.Format("{0}/AddAction?flightId={1}&zone={2}&action={3}",
                ServiceStrings.Bus, id, zoneNum, (int)PlaneServiceStage.LOAD_PASSENGERS);
            Util.MakeRequest(URL);
        }

        public void FinishUnloadingCargo(string id, int zoneNum)
        {
            Zone zone = (Zone)zoneNum;
            switch (zone)
            {
                case Zone.HANGAR_1:
                    if (h1stage.Item1.Equals(id) == false)
                    {
                        throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
                    }
                    h1stage.Item2 = PlaneServiceStage.REFUEL;
                    break;
                case Zone.HANGAR_2:
                    if (h2stage.Item1.Equals(id) == false)
                    {
                        throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
                    }
                    h2stage.Item2 = PlaneServiceStage.REFUEL;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
            }
        }

        public void FinishRefueling(string id, int zoneNum)
        {
            Zone zone = (Zone)zoneNum;
            switch (zone)
            {
                case Zone.HANGAR_1:
                    if (h1stage.Item1.Equals(id) == false)
                    {
                        throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
                    }
                    h1stage.Item2 = PlaneServiceStage.LOAD_CARGO;
                    break;
                case Zone.HANGAR_2:
                    if (h2stage.Item1.Equals(id) == false)
                    {
                        throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
                    }
                    h2stage.Item2 = PlaneServiceStage.LOAD_CARGO;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
            }
        }

        public void FinishLoadingCargo(string id, int zoneNum)
        {
            Zone zone = (Zone)zoneNum;
            switch (zone)
            {
                case Zone.HANGAR_1:
                    if (h1stage.Item1.Equals(id) == false)
                    {
                        throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
                    }
                    h1stage.Item2 = PlaneServiceStage.LOAD_PASSENGERS;
                    break;
                case Zone.HANGAR_2:
                    if (h2stage.Item1.Equals(id) == false)
                    {
                        throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
                    }
                    h2stage.Item2 = PlaneServiceStage.LOAD_PASSENGERS;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
            }
        }

        public void FinishLoadingPassengers(string id, int zoneNum)
        {
            Zone zone = (Zone)zoneNum;
            switch (zone)
            {
                case Zone.HANGAR_1:
                    if (h1stage.Item1.Equals(id) == false)
                    {
                        throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
                    }
                    h1stage.Item2 = PlaneServiceStage.TAKEOFF;
                    break;
                case Zone.HANGAR_2:
                    if (h2stage.Item1.Equals(id) == false)
                    {
                        throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
                    }
                    h2stage.Item2 = PlaneServiceStage.TAKEOFF;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zoneNum), zoneNum, null);
            }
            string URL = String.Format("{0}/GoAway?id={1}", ServiceStrings.Plane, id);
            Util.MakeRequest(URL);
        }

        public string CheckStage(string id, int zoneNum)
        {
            Zone zone = (Zone) zoneNum;
            if (zone == Zone.HANGAR_1)
            {
                if (h1stage.Item1.Equals(id))
                {
                    return JsonConvert.SerializeObject((int)h1stage.Item2);
                }
            }
            else if (zone == Zone.HANGAR_2)
            {
                if (h2stage.Item1.Equals(id))
                {
                    return JsonConvert.SerializeObject((int)h2stage.Item2);
                }
            }
            else throw new ArgumentOutOfRangeException(nameof(zoneNum));
            return JsonConvert.SerializeObject((int)PlaneServiceStage.NOT_IN_SERVICE);
        }
    }
}
