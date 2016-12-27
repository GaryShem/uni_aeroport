using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Common;
using Newtonsoft.Json;
using RegistrationStand;

namespace Passenger
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PassengerService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PassengerService.svc or PassengerService.svc.cs at the Solution Explorer and start debugging.
    public class PassengerService : IPassengerService
    {
        public string GetPassengerInfo(string id)
        {
            Common.Passenger passenger = new Common.Passenger();
            passenger.CurrentZone = Zone.PASSENGER_SPAWN;
            passenger.CargoCount = 3;
            passenger.FlightId = Guid.NewGuid().ToString();
            string result = JsonConvert.SerializeObject(passenger);
            return result;
        }

        public string GeneratePassengers(string flightId, int passengerCount)
        {
            List<Common.Passenger> planePassengers = GeneratePlanePassengers(flightId, passengerCount);
            List<Common.Passenger> groundPassengers = GenerateGroundPassengers(flightId);

            lock (PassengerHandler.Passengers)
            {
                PassengerHandler.Passengers.AddRange(planePassengers);
                PassengerHandler.Passengers.AddRange(groundPassengers);
            }

            RegistrationList registrationList = new RegistrationList(flightId);
            registrationList.Passengers = planePassengers.Select(x => x.Id).ToList();
            registrationList.CargoCount = planePassengers.Sum(x => x.CargoCount);
            return JsonConvert.SerializeObject(registrationList);
        }

        public List<Common.Passenger> GeneratePlanePassengers(string flightId, int count)
        {
            List<Common.Passenger> result = new List<Common.Passenger>();
            for (int i = 0; i < count; i++)
            {
                result.Add(new Common.Passenger(flightId, true));
            }
            
            return result;
        }

        public List<Common.Passenger> GenerateGroundPassengers(string flightId)
        {
            List<Common.Passenger> result = new List<Common.Passenger>();
            int count = RandomGen.Next(Common.Passenger.MinGeneratedPassengerCount,
                Common.Passenger.MaxGeneratedPassengerCount + 1);
            for (int i = 0; i < count; i++)
            {
                result.Add(new Common.Passenger(flightId, false));
            }
            return result;
        }
    }
}
