using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FuelTruck
    {
        public const int FUEL_CAPACITY = 50;
        public const int REFUEL_SPEED = 10;
        public string Id { get; set; }
        public Zone CurrentZone;
        public EntityState State;
        public List<Tuple<string, Zone>> Commands;
        public Tuple<string, Zone> CurrentCommand;
        public int CurrentFuel { get; set; }

        public FuelTruck()
        {
            Id = Guid.NewGuid().ToString();
            CurrentZone = Zone.FUEL_STATION;
            CurrentFuel = FUEL_CAPACITY;
            State = EntityState.WAITING_FOR_COMMAND;
            Commands = new List<Tuple<string, Zone>>();
        }
    }
}
