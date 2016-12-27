using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Common;

namespace Passenger
{
    public static class PassengerHandler
    {
        public static List<Common.Passenger> Passengers = new List<Common.Passenger>();
        private static Thread PassengerHandlerThread;

        static PassengerHandler()
        {
            PassengerHandlerThread = new Thread(HandlePassengers);
            PassengerHandlerThread.Start();
        }

        public static void HandlePassengers()
        {
            while (true)
            {
                lock (Passengers)
                {
                    for (int i = 0; i < Passengers.Count; i++)
                    {
                        
                    }
                }
            }
        }
    }
}