Assignment 02: Drone Surveillance System (Refined)
Demonstrates: encapsulation, state management, validation, exception handling, and realistic drone behaviour

using System;

namespace DroneSurveillanceSystem
{
    public enum DroneState
    {
        Grounded,
        Flying,
        Charging
    }

    public class Drone
    {
        // Constants
        private const int MaxBattery = 100;
        private const int MinBatteryForTakeoff = 20;
        private const int BatteryDrainPerHoverMinute = 2;
        private const int BatteryDrainPerMoveUnit = 1;       // per 10 metres
        private const int BatteryDrainPerPhoto = 3;
        private const int BatteryDrainLanding = 2;
        private const int BatteryDrainTakeoff = 5;
        private const double MaxAltitude = 500.0;            // metres

        // Private fields
        private int _batteryLife;          // 0..100
        private double _altitude;          // metres
        private DroneState _state;

        // Read-only properties
        public string DroneId { get; }
        public string Model   { get; }

        // Public read-only accessors
        public int BatteryLife => _batteryLife;
        public double Altitude => _altitude;
        public DroneState State => _state;

        public Drone(string id, string model, int initialBattery = 100)
        {
            if (initialBattery < 0 || initialBattery > MaxBattery)
                throw new ArgumentOutOfRangeException(nameof(initialBattery),
                    $"Initial battery must be between 0 and {MaxBattery}.");

            DroneId = id;
            Model   = model;
            _batteryLife = initialBattery;
            _altitude    = 0;
            _state       = DroneState.Grounded;
        }

        // Take off
        public void TakeOff(double targetAltitude)
        {
            if (_state == DroneState.Flying)
                throw new InvalidOperationException($"Drone {DroneId} is already flying at {_altitude} m.");

            if (_state == DroneState.Charging)
                throw new InvalidOperationException($"Drone {DroneId} is currently charging. Wait until finished.");

            if (_batteryLife < MinBatteryForTakeoff)
                throw new InvalidOperationException(
                    $"Battery too low ({_batteryLife}%). Minimum for takeoff is {MinBatteryForTakeoff}%.");

            if (targetAltitude <= 0 || targetAltitude > MaxAltitude)
                throw new ArgumentOutOfRangeException(nameof(targetAltitude),
                    $"Altitude must be between 0 and {MaxAltitude} metres.");

            _state = DroneState.Flying;
            _altitude = targetAltitude;
            _batteryLife = Math.Max(0, _batteryLife - BatteryDrainTakeoff);

            Console.WriteLine($"[{DroneId}] Took off to {_altitude} m. Battery = {_batteryLife}%");
        }

        // Ascend/Descend
        public void Ascend(double metres)
        {
            if (_state != DroneState.Flying)
                throw new InvalidOperationException("Cannot ascend – drone is not flying.");

            if (metres <= 0)
                throw new ArgumentOutOfRangeException(nameof(metres), "Ascend amount must be positive.");

            double newAltitude = _altitude + metres;
            if (newAltitude > MaxAltitude)
                throw new InvalidOperationException($"Cannot ascend above {MaxAltitude} m.");

            _altitude = newAltitude;
            _batteryLife = Math.Max(0, _batteryLife - (int)Math.Ceiling(metres / 10.0));
            Console.WriteLine($"[{DroneId}] Ascended to {_altitude} m. Battery = {_batteryLife}%");
        }

        public void Descend(double metres)
        {
            if (_state != DroneState.Flying)
                throw new InvalidOperationException("Cannot descend – drone is not flying.");

            if (metres <= 0)
                throw new ArgumentOutOfRangeException(nameof(metres), "Descend amount must be positive.");

            double newAltitude = _altitude - metres;
            if (newAltitude < 0)
                throw new InvalidOperationException("Cannot descend below ground level.");

            _altitude = newAltitude;
            _batteryLife = Math.Max(0, _batteryLife - (int)Math.Ceiling(metres / 10.0));
            Console.WriteLine($"[{DroneId}] Descended to {_altitude} m. Battery = {_batteryLife}%");
        }

        // Hover(simulates time passing)
        public void Hover(int minutes)
        {
            if (_state != DroneState.Flying)
                throw new InvalidOperationException("Cannot hover – drone is not flying.");

            if (minutes <= 0)
                throw new ArgumentOutOfRangeException(nameof(minutes), "Hover time must be positive.");

            int drain = minutes * BatteryDrainPerHoverMinute;
            _batteryLife = Math.Max(0, _batteryLife - drain);
            Console.WriteLine($"[{DroneId}] Hovered for {minutes} min. Battery = {_batteryLife}%");

            if (_batteryLife == 0)
            {
                Console.WriteLine($"[{DroneId}] Battery depleted during hover. Forcing landing.");
                ForceLand();
            }
        }

        // Move horizontally
        public void Move(double horizontalDistance)
        {
            if (_state != DroneState.Flying)
                throw new InvalidOperationException("Cannot move – drone is not flying.");

            if (horizontalDistance <= 0)
                throw new ArgumentOutOfRangeException(nameof(horizontalDistance), "Distance must be positive.");

            int drain = (int)Math.Ceiling(horizontalDistance / 10.0) * BatteryDrainPerMoveUnit;
            _batteryLife = Math.Max(0, _batteryLife - drain);
            Console.WriteLine($"[{DroneId}] Moved {horizontalDistance} m horizontally. Battery = {_batteryLife}%");

            if (_batteryLife == 0)
            {
                Console.WriteLine($"[{DroneId}] Battery depleted. Forcing landing.");
                ForceLand();
            }
        }

        // Take a photo(surveillance action)
        public void TakePhoto(string subject)
        {
            if (_state != DroneState.Flying)
                throw new InvalidOperationException("Cannot take photo – drone is not flying.");

            if (_batteryLife < BatteryDrainPerPhoto)
                throw new InvalidOperationException($"Insufficient battery ({_batteryLife}%) to take photo.");

            _batteryLife -= BatteryDrainPerPhoto;
            Console.WriteLine($"[{DroneId}] 📸 Photo taken of '{subject}'. Battery = {_batteryLife}%");
        }

        // Land
        public void Land()
        {
            if (_state == DroneState.Grounded)
                throw new InvalidOperationException($"Drone {DroneId} is already on the ground.");

            if (_state == DroneState.Charging)
                throw new InvalidOperationException("Cannot land while charging – stop charging first.");

            // If flying, perform landing
            _state = DroneState.Grounded;
            _altitude = 0;
            _batteryLife = Math.Max(0, _batteryLife - BatteryDrainLanding);
            Console.WriteLine($"[{DroneId}] Landed safely. Battery = {_batteryLife}%");
        }

        // Force landing(used when battery dies)
        private void ForceLand()
        {
            if (_state == DroneState.Flying)
            {
                _state = DroneState.Grounded;
                _altitude = 0;
                _batteryLife = 0;
                Console.WriteLine($"[{DroneId}] ⚠️ Forced landing due to battery exhaustion.");
            }
        }

        // Charge
        public void Charge(int amount)
        {
            if (_state == DroneState.Flying)
                throw new InvalidOperationException("Cannot charge while flying. Land first.");

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Charge amount must be positive.");

            _state = DroneState.Charging;
            int oldBattery = _batteryLife;
            _batteryLife = Math.Min(MaxBattery, _batteryLife + amount);
            int actualAdded = _batteryLife - oldBattery;
            Console.WriteLine($"[{DroneId}] Charged by {actualAdded}%. Battery = {_batteryLife}%");

            // Automatically return to grounded state after charging
            if (_batteryLife == MaxBattery)
                Console.WriteLine($"[{DroneId}] Battery full.");
            _state = DroneState.Grounded;
        }

        // Return to home(if flying)
        public void ReturnToHome()
        {
            if (_state != DroneState.Flying)
                throw new InvalidOperationException("Drone is not flying.");

            Console.WriteLine($"[{DroneId}] Returning to home...");
            Land();
        }

        public override string ToString() =>
            $"Drone {DroneId} ({Model}) — State={_state}, Alt={_altitude} m, Battery={_batteryLife}%";
    }

    class Program
    {
        static void Main()
        {
            try
            {
                var drone = new Drone("DR-01", "SkyHawk-X", initialBattery: 30);

                Console.WriteLine("Initial state: " + drone);
                Console.WriteLine();

                // Try invalid actions
                try { drone.Move(50); }
                catch (InvalidOperationException ex) { Console.WriteLine($"❌ {ex.Message}"); }

                try { drone.TakeOff(-10); }
                catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }

                Console.WriteLine();

                // Successful takeoff
                drone.TakeOff(100);
                drone.Hover(2);              // 2 min hover
                drone.Move(30);
                drone.Ascend(20);
                drone.TakePhoto("suspicious vehicle");
                drone.Descend(10);
                drone.ReturnToHome();

                // Cannot take off while flying
                try { drone.TakeOff(50); }
                catch (InvalidOperationException ex) { Console.WriteLine($"❌ {ex.Message}"); }

                // Charge after landing
                drone.Charge(50);
                Console.WriteLine("\nFinal state: " + drone);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}