using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FuelTruck
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FuelTruckService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select FuelTruckService.svc or FuelTruckService.svc.cs at the Solution Explorer and start debugging.
    public class FuelTruckService : IFuelTruckService
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }
    }
}
