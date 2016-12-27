using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Bus
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BusService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select BusService.svc or BusService.svc.cs at the Solution Explorer and start debugging.
    public class BusService : IBusService
    {

        public string LoadPassengers(string flightId)
        {
            throw new NotImplementedException();
        }
    }
}
