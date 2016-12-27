using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Bus
{
    public static class BusHandler
    {
        private static Thread BusHandlerThread;
        static BusHandler()
        {
            BusHandlerThread = new Thread(HandleBus);
            BusHandlerThread.Start();
        }

        private static void HandleBus()
        {
            while (true)
            {
                Thread.Sleep(1000);

                //добавлять действия сюда
            }
        }
    }
}