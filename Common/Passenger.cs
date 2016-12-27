using System;

namespace Common
{
    public class Passenger
    {
        private const int MIN_CARGO_COUNT = 1;
        private const int MAX_CARGO_COUNT = 5;
        public const int MinGeneratedPassengerCount = 6;
        public const int MaxGeneratedPassengerCount = 15;
        public string Id { get; set; }
        public string FlightId { get; set; }
        public int CargoCount { get; set; }
        public Zone CurrentZone { get; set; }
        public EntityState State { get; set; }
        public RegistrationState RegState { get; set; }
        public bool IsFlyingAway { get; set; }
        public bool HoldingCargo { get; set; }

        public Passenger() { }
        public Passenger(string flightId, bool isOnPlane)
        {
            Id = Guid.NewGuid().ToString();
            FlightId = flightId;
            CargoCount = RandomGen.Next(MIN_CARGO_COUNT, MAX_CARGO_COUNT + 1);
            CurrentZone = isOnPlane ? Zone.PLANE : Zone.PASSENGER_SPAWN;
            State = EntityState.WAITING_FOR_COMMAND;
            RegState = isOnPlane ? RegistrationState.REGISTERED : RegistrationState.NOT_REGISTERED;
            IsFlyingAway = isOnPlane ? false : true;
            HoldingCargo = isOnPlane ? false : true;
        }
    }
}