using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Common
{
    public class Plane
    {
        public const int MIN_GENERATED_FUEL = 10;
        public const int MAX_GENERATED_FUEL = 80;
        public const int PassengerCapacity = 10;
        public const int FuelCapacity = 100;
        public string Id { get; set; }
        public int PassengerCount { get { return Passengers.Count; } }
        public int CargoCount { get; set; }
        public int FuelCount { get; set; }
        public Zone CurrentZone { get; set; }
        public DateTime NextActionTime { get; set; }
        public EntityState State { get; set; }
        public DateTime? ActionTime { get; set; }
        public DateTime SpawnTime { get; set; }
        public List<string> Passengers { get; set; }
        //TODO: добавить список пассажиров
        public Plane(){}

        public Plane(string id, List<string> passengerIds, int cargoCount, int fuelCount)
        {
            Id = id;
            CargoCount = cargoCount;
            FuelCount = fuelCount;
            State = EntityState.WAITING_FOR_COMMAND;
            CurrentZone = Zone.PLANE_SPAWN;
            ActionTime = null;
            SpawnTime = DateTime.Now;
            Passengers = new List<string>(passengerIds);
        }
    }
}
