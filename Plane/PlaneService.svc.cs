using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using Common;
using Newtonsoft.Json;

namespace Plane
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PlaneService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PlaneService.svc or PlaneService.svc.cs at the Solution Explorer and start debugging.
    public class PlaneService : IPlaneService
    {
        // завершение движения - посадки или отлёта
        void IPlaneService.CompleteMove(string id, int zone)
        {
            lock (PlaneHandler.Planes)
            {
                Common.Plane plane = PlaneHandler.Planes.Find(x => x.Id.Equals(id));
                PlaneHandler.FreeHangar(plane.CurrentZone);
                plane.CurrentZone = (Zone) zone;
                plane.State = EntityState.FINISHED_TASK;
            }
        }

        // получение данных о всех существующих самолётах
        public string GetAllPlanes()
        {
            lock (PlaneHandler.Planes)
            {
                string result = JsonConvert.SerializeObject(PlaneHandler.Planes);
                return result;
            }
        }
        

        public void GoAway(string id)
        {
            lock (PlaneHandler.Planes)
            {
                Common.Plane plane = PlaneHandler.Planes.Find(x => x.Id.Equals(id));
                if (plane == null) return;
                plane.State = EntityState.WAITING_FOR_COMMAND;
            }
        }

        public void TakePassengersFromBus(string id)
        {
            Common.Plane plane;
            lock (PlaneHandler.Planes)
            {
                plane = PlaneHandler.Planes.Find(x => x.Id.Equals(id));
            }
            string URL = String.Format("{0}/UnloadPassengers", ServiceStrings.Bus);
            string response = Util.MakeRequest(URL);
            List<string> passengers = JsonConvert.DeserializeObject<List<string>>(response);
            foreach (string passenger in passengers)
            {
                plane.Passengers.Add(passenger);
                URL = String.Format("{0}/CompleteMove?id={1}&zone={2}", ServiceStrings.Passenger, passenger, (int)Zone.PLANE);
                Util.MakeRequest(URL);
            }
            URL = String.Format("{0}/GetPassengers?flightId={1}", ServiceStrings.RegStand, plane.Id);
            response = Util.MakeRequest(URL);
            List<string> passengerList = JsonConvert.DeserializeObject<List<string>>(response);
            if (plane.Passengers.Count == passengerList.Count)
            {
                URL = String.Format("{0}/FinishLoadingPassengers?id={1}&zone={2}", ServiceStrings.GrControl, plane.Id, (int)plane.CurrentZone);
                Util.MakeRequest(URL);
            }
        }

        public string UnloadPassengers(string flightId, int count)
        {
            List<string> result = new List<string>();
            Common.Plane plane;
            lock (PlaneHandler.Planes)
            {
                plane = PlaneHandler.Planes.Find(x => x.Id.Equals(flightId));
            }
            for (int i = 0; i < count; i++)
            {
                if (plane.Passengers.Count <= 0)
                {
                    break;
                }
                result.Add(plane.Passengers[0]);
                plane.Passengers.RemoveAt(0);
            }
            if (plane.Passengers.Count == 0)
            {
                string URL = String.Format("{0}/FinishUnloadingPassengers?id={1}&zone={2}", ServiceStrings.GrControl, plane.Id, (int) plane.CurrentZone);
                Util.MakeRequest(URL);
            }
            return JsonConvert.SerializeObject(result);
        }

        public string AcceptFuel(string flightId, int count)
        {
            Common.Plane plane;
            int acceptedFuel = 0;
            lock (PlaneHandler.Planes)
            {
                plane = PlaneHandler.Planes.Find(x => x.Id.Equals(flightId));
            }
            acceptedFuel = Math.Min(count, Common.Plane.FuelCapacity - plane.FuelCount);
            plane.FuelCount += acceptedFuel;
            if (plane.FuelCount == Common.Plane.FuelCapacity)
            {
                string URL = String.Format("{0}/FinishRefueling?id={1}&zone={2}", ServiceStrings.GrControl, plane.Id, (int)plane.CurrentZone);
                Util.MakeRequest(URL);
            }
            return JsonConvert.SerializeObject(acceptedFuel);
        }

        // делается для того, чтобы заставить статические объекты инициализироваться
        public void Start()
        {
            lock (PlaneHandler.Planes)
            {
                int a = PlaneHandler.Planes.Count;
            }
        }

        public string GetRemainingCargoToLoad(string flightId)
        {
            string URL = String.Format("{0}/GetCargo?flightId={1}", ServiceStrings.RegStand, flightId);
            string response = Util.MakeRequest(URL);
            int registeredCargo = JsonConvert.DeserializeObject<int>(response);
            Common.Plane plane;
            lock (PlaneHandler.Planes)
            {
                plane = PlaneHandler.Planes.Find(x => x.Id.Equals(flightId));
            }
            int remainingCargo = registeredCargo - plane.CargoCount;
            return JsonConvert.SerializeObject(remainingCargo);
        }

        public void TakeCargoFromTruck(string id, int cargoCount)
        {
            Common.Plane plane;
            lock (PlaneHandler.Planes)
            {
                plane = PlaneHandler.Planes.Find(x => x.Id.Equals(id));
            }
            plane.CargoCount += cargoCount;
            string URL = String.Format("{0}/GetCargo?flightId={1}", ServiceStrings.RegStand, plane.Id);
            string response = Util.MakeRequest(URL);
            int registeredCargo = JsonConvert.DeserializeObject<int>(response);
            if (plane.CargoCount >= registeredCargo)
            {
                URL = String.Format("{0}/FinishLoadingCargo?id={1}&zone={2}", ServiceStrings.GrControl, plane.Id, (int)plane.CurrentZone);
                Util.MakeRequest(URL);
            }
        }

        public string GiveCargoToTruck(string id, int cargoCount)
        {
            Common.Plane plane;
            lock (PlaneHandler.Planes)
            {
                plane = PlaneHandler.Planes.Find(x => x.Id.Equals(id));
            }
            int givenCargo = Math.Min(plane.CargoCount, cargoCount);
            plane.CargoCount -= givenCargo;
            if (plane.CargoCount == 0)
            {
                string URL = String.Format("{0}/FinishUnloadingCargo?id={1}&zone={2}", ServiceStrings.GrControl, plane.Id, (int)plane.CurrentZone);
                Util.MakeRequest(URL);
            }
            return JsonConvert.SerializeObject(givenCargo);
        }
    }
}
