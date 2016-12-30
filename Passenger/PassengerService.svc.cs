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

        public void CompleteMove(string id, int zoneNum)
        {
            Zone zone = (Zone) zoneNum;
                Common.Passenger passenger = PassengerHandler.Passengers.Find(x => x.Id.Equals(id));
                passenger.CurrentZone = zone;
                passenger.State = EntityState.WAITING_FOR_COMMAND;
        }

        public void TakePassengersFromBus()
        {
            string URL = String.Format("{0}/UnloadPassengers", ServiceStrings.Bus);
            string response = Util.MakeRequest(URL);
            List<string> passengerList = JsonConvert.DeserializeObject<List<String>>(response);

            foreach (string passengerId in passengerList)
            {
                //"Spawn?type={entityNum}&id={id}&zone={zoneNum}&cargo={cargoCount}"
                Common.Passenger passenger;
                passenger = PassengerHandler.Passengers.Find(x => x.Id.Equals(passengerId));

                CompleteMove(passengerId, (int) Zone.WAITING_AREA);
                URL = String.Format("{0}/Spawn?type={1}&id={2}&zone={3}&cargo={4}", ServiceStrings.Vis,
                    (int) Entity.PASSENGER, passenger.Id, (int) passenger.CurrentZone, passenger.CargoCount);
                Util.MakeRequest(URL);
            }
        }

        public string GivePassengersToBus(string flightId)
        {
            List<string> passengerList =
                PassengerHandler.Passengers.Where(x => x.Id.Equals(flightId) && x.CurrentZone == Zone.WAITING_AREA)
                    .Take(2)
                    .Select(x => x.Id)
                    .ToList();
            foreach (string passengerId in passengerList)
            {
                CompleteMove(passengerId, (int)Zone.BUS);
                string URL = String.Format("{0}/Despawn?id={1}", ServiceStrings.Vis, passengerId);
                Util.MakeRequest(URL);
            }
            return JsonConvert.SerializeObject(passengerList);
        }

        public string GetAllPassengers()
        {
            lock (PassengerHandler.Passengers)
            {
                return JsonConvert.SerializeObject(PassengerHandler.Passengers);
            }
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
