using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CargoTruck
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CargoTruckService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CargoTruckService.svc or CargoTruckService.svc.cs at the Solution Explorer and start debugging.
    public class CargoTruckService : ICargoTruckService
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }
    }
}
