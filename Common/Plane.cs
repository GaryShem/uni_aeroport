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
        private const int MIN_GENERATED_FUEL = 10;
        private const int MAX_GENERATED_FUEL = 80;
        public const int PassengerCapacity = 10;
        public const int FuelCapacity = 100;
        public string Id { get; set; }
        public int PassengerCount { get; set; }
        public Int32 CargoCount { get; set; }
        public int FuelCount { get; set; }
        public Zone CurrentZone { get; set; }
        public DateTime NextActionTime { get; set; }
        public EntityState State { get; set; }
        public DateTime? ActionTime { get; set; }
        public DateTime SpawnTime { get; set; }
        //TODO: добавить список пассажиров
        public Plane(){}

        public Plane(string id, int passengerCount, int cargoCount, int fuelCount)
        {
            Id = id;
            PassengerCount = passengerCount;
            //TODO: высчитывать груз по пассажирам
            CargoCount = cargoCount;
            FuelCount = fuelCount;
            State = EntityState.WAITING_FOR_COMMAND;
            CurrentZone = Zone.PLANE_SPAWN;
            ActionTime = null;
            SpawnTime = DateTime.Now;
        }

        public static Plane CreatePlane()
        {
            int fuelCount = RandomGen.Next(MIN_GENERATED_FUEL, MAX_GENERATED_FUEL + 1);
            int passengerCount = RandomGen.Next(1, PassengerCapacity + 1);
            string id = Guid.NewGuid().ToString();
            return new Plane(id, passengerCount, -1, fuelCount);
        }
    }
}
