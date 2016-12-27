using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;

namespace RegistrationStand
{
    public class RegistrationList
    {
        private const int PassengerCapacity = Plane.PassengerCapacity;
        public string FlightId { get; set; }
        public List<string> Passengers { get; set; }
        public bool IsRegistrationOpened { get; set; }
        public int CargoCount { get; set; }

        public bool Register(string passengerId, int cargoCount)
        {
            lock (Passengers)
            {
                if (IsRegistrationOpened == false)
                    return false;
                if (Passengers.Any(x => x.Equals(passengerId)))
                    return false;
                if (Passengers.Count < PassengerCapacity)
                {
                    Passengers.Add(passengerId);
                    CargoCount += cargoCount;
                    return true;
                }
                else
                {
                    IsRegistrationOpened = false;
                    return false;
                }
            }
        }

        public RegistrationList(string flightId)
        {
            Passengers = new List<string>();
            FlightId = flightId;
            IsRegistrationOpened = true;
            CargoCount = 0;
        }
    }
}