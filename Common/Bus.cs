using System;
using System.Collections.Generic;

namespace Common
{
    public class Bus
    {
        public const int MAX_COUNT_OF_PASSENGERS = 6;
        public const int TAKE_PASSENGERS = 2;
        public const int TIME_TO_SLEEP = 1000;
        public string Id { get; set; }
        public Zone CurrentZone;
        public EntityState State;
        public List<Tuple<string, Zone, PlaneServiceStage>> Commands;
        public Tuple<string, Zone, PlaneServiceStage> CurrentCommand;
        public List<string> Passengers;

        public Bus()
        {
            Id = Guid.NewGuid().ToString();
            CurrentZone = Zone.BUS_STATION;
            State = EntityState.WAITING_FOR_COMMAND;
            Commands = new List<Tuple<string, Zone, PlaneServiceStage>>();
            Passengers = new List<string>(MAX_COUNT_OF_PASSENGERS);
        }
    }
}