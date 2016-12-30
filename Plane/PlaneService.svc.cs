using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
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

        // делается для того, чтобы заставить статические объекты инициализироваться
        public void Start()
        {
            return;
        }
    }
}
