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
        // генерация нового самолёта
        void IPlaneService.GeneratePlane(int passengerCount, int fuelCount)
        {
            throw new NotImplementedException();
        }

        // завершение движения - посадки или отлёта
        void IPlaneService.CompleteMove(string id, int zone)
        {
            lock (PlaneHandler.Planes)
            {
                Common.Plane plane = PlaneHandler.Planes.Find(x => x.Id.Equals(id));
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

        // сказать самолёту, что автобус готов отдавать пассажиров на нужный рейс
        // самолёт в ответ попросит автобус погрузить пассажиров на нужный рейс
        public void LoadPassengers(string id)
        {
            //TODO: послать обращение автобусу, чтобы отдал нам пассажиров
            // этот финт ушами делается из-за того, что в параметрах запроса не передать массив сложных объектов
            throw new NotImplementedException();
        }

        // делается для того, чтобы заставить статические объекты инициализироваться
        public void Start()
        {
            return;
        }
    }
}
