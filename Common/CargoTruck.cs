using System;
using System.Collections.Generic;

namespace Common
{
    public class CargoTruck
    {
        public const int CARGO_CAPACITY = 10;
        public const int LOAD_SPEED = 5;
        public string Id { get; set; }
        public Zone CurrentZone;
        public EntityState State;
        public List<Tuple<string, Zone, PlaneServiceStage>> Commands;
        public Tuple<string, Zone, PlaneServiceStage> CurrentCommand;
        public int CurrentCargo { get; set; }

        public CargoTruck()
        {
            Id = Guid.NewGuid().ToString();
            CurrentZone = Zone.CARGO_AREA;
            CurrentCargo = 0;
            State = EntityState.WAITING_FOR_COMMAND;
            Commands = new List<Tuple<string, Zone, PlaneServiceStage>>();
        }
    }
}