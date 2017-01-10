using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.PeerResolvers;
using System.Web;

namespace RegistrationStand
{
    public static class RegistrationStand
    {
        public static List<RegistrationList> RegistrationLists = new List<RegistrationList>();

        public static bool Register(string flightId, string passengerId, int cargoCount)
        {
            lock (RegistrationLists)
            {
                RegistrationList list = RegistrationLists.Find(x => x.FlightId.Equals(flightId));
                return list != null && list.Register(passengerId, cargoCount);
            }
        }

        public static void OpenRegistration(string flightId)
        {
            lock (RegistrationLists)
            {
                RegistrationList list = RegistrationLists.Find(x => x.FlightId.Equals(flightId));
                if (list == null)
                {
                    RegistrationLists.Add(new RegistrationList(flightId));
                }
                else
                {
                    list.IsRegistrationOpened = true;
                }
            }
        }

        public static void CloseRegistration(string flightId)
        {
            lock (RegistrationLists)
            {
                RegistrationList list = RegistrationLists.Find(x => x.FlightId.Equals(flightId));
                if (list == null) return;
                list.IsRegistrationOpened = false;
            }

        }

        public static void DeleteList(string flightId)
        {
            lock (RegistrationLists)
            {
                RegistrationLists.RemoveAll(x => x.FlightId.Equals(flightId));
            }
        }

        public static List<string> GetPassengers(string flightId)
        {
            lock (RegistrationLists)
            {
                RegistrationList list = RegistrationLists.Find(x => x.FlightId.Equals(flightId));
                return list != null ? list.Passengers : new List<string>();
            }
        }

        public static int GetCargo(string flightId)
        {
            lock (RegistrationLists)
            {
                RegistrationList list = RegistrationLists.Find(x => x.FlightId.Equals(flightId));
                return list != null ? list.CargoCount : -1;
            }
        }
    }
}