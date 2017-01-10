using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;

namespace RegistrationStand
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RegistrationStandService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RegistrationStandService.svc or RegistrationStandService.svc.cs at the Solution Explorer and start debugging.
    public class RegistrationStandService : IRegistrationStandService
    {
        public void OpenRegistration(string flightId)
        {
            RegistrationStand.OpenRegistration(flightId);
        }

        public string Register(string flightId, string passengerId, int cargoCount)
        {
            bool result = RegistrationStand.Register(flightId, passengerId, cargoCount);
            return JsonConvert.SerializeObject(result);
        }

        public void CloseRegistration(string flightId)
        {
            RegistrationStand.CloseRegistration(flightId);
        }

        public string GetPassengers(string flightId)
        {
            List<string> passengers = RegistrationStand.GetPassengers(flightId);
            return JsonConvert.SerializeObject(passengers);
        }

        public string GetCargo(string flightId)
        {
            int cargo = RegistrationStand.GetCargo(flightId);
            return JsonConvert.SerializeObject(cargo);
        }

        public void DeleteList(string flightId)
        {
            RegistrationStand.DeleteList(flightId);
        }
    }
}
