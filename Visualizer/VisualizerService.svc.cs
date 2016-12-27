using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Common;

namespace Visualizer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "VisualizerService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select VisualizerService.svc or VisualizerService.svc.cs at the Solution Explorer and start debugging.
    public class VisualizerService : IVisualizerService
    {
        public void Spawn(Entity entity, string id, Zone zone, int cargoCount)
        {
            switch (entity)
            {
                case Entity.BUS:
                case Entity.CARGO_TRUCK:
                case Entity.FUEL_TRUCK:
                case Entity.PASSENGER:
                    break;
            }
        }

        public void SpawnPlane(Entity entity, string id, Zone zone, int cargoCount, int passengerCount)
        {
            throw new NotImplementedException();
        }

        public void Despawn(string id, Zone zone)
        {
            throw new NotImplementedException();
        }

        public void Move(string id, Zone zone)
        {
            throw new NotImplementedException();
        }
    }
}
