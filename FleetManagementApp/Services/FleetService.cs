using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FleetManagementApp.Data.Interfaces;
using FleetManagementApp.Models;

namespace FleetManagementApp.Services
{
    /// <summary>
    /// Encapsulates fleet business logic, validation rules, and repository coordination.
    /// </summary>
    public class FleetService
    {
        private readonly IVehicleRepository _vehicleRepository;

        public FleetService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        }

        /// <summary>
        /// Onboard a new vehicle into the fleet with validation.
        /// </summary>
        public async Task<(bool Success, string Message, int VehicleId)> OnboardVehicleAsync(Vehicle vehicle)
        {
            if (vehicle == null)
                return (false, "Vehicle details cannot be null.", 0);

            if (string.IsNullOrWhiteSpace(vehicle.VIN) || vehicle.VIN.Trim().Length != 17)
                return (false, "VIN must be exactly 17 alphanumeric characters.", 0);

            if (string.IsNullOrWhiteSpace(vehicle.Make) || string.IsNullOrWhiteSpace(vehicle.Model))
                return (false, "Make and Model are required.", 0);

            if (vehicle.Year < 1980 || vehicle.Year > DateTime.UtcNow.Year + 1)
                return (false, $"Year must be between 1980 and {DateTime.UtcNow.Year + 1}.", 0);

            if (vehicle.OdometerReading < 0)
                return (false, "Initial odometer reading cannot be negative.", 0);

            // Business Check: Duplicate VIN check
            var existing = await _vehicleRepository.GetByVinAsync(vehicle.VIN);
            if (existing != null)
                return (false, $"A vehicle with VIN '{vehicle.VIN}' already exists (ID: {existing.Id}).", 0);

            vehicle.Status = VehicleStatus.Active;
            int newId = await _vehicleRepository.AddAsync(vehicle);

            return (true, "Vehicle onboarded successfully.", newId);
        }

        /// <summary>
        /// Retrieves all active vehicles in the fleet.
        /// </summary>
        public async Task<IReadOnlyList<Vehicle>> GetActiveFleetInventoryAsync()
        {
            return await _vehicleRepository.GetActiveVehiclesAsync();
        }

        /// <summary>
        /// Retrieves all vehicles regardless of status.
        /// </summary>
        public async Task<IReadOnlyList<Vehicle>> GetAllVehiclesAsync()
        {
            return await _vehicleRepository.GetAllAsync();
        }

        /// <summary>
        /// Retrieves a single vehicle by its primary ID.
        /// </summary>
        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            if (id <= 0) return null;
            return await _vehicleRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Logs telemetry: updates odometer mileage and vehicle operational status.
        /// </summary>
        public async Task<(bool Success, string Message)> LogTelemetryAsync(int vehicleId, decimal newOdometer, VehicleStatus newStatus)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
                return (false, $"Vehicle with ID {vehicleId} was not found.");

            // Business rule: Odometer cannot be rolled backwards
            if (newOdometer < vehicle.OdometerReading)
            {
                return (false, $"Invalid reading! New odometer ({newOdometer:N1}) cannot be less than current odometer ({vehicle.OdometerReading:N1}).");
            }

            bool updated = await _vehicleRepository.UpdateTelemetryAsync(vehicleId, newOdometer, newStatus);
            if (!updated)
                return (false, "Database update failed.");

            return (true, $"Telemetry updated: Vehicle ID {vehicleId} is now '{newStatus}' at {newOdometer:N1} km.");
        }

        /// <summary>
        /// Decommissions and purges a retired vehicle record.
        /// </summary>
        public async Task<(bool Success, string Message)> DecommissionVehicleAsync(int vehicleId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
                return (false, $"Vehicle with ID {vehicleId} was not found.");

            bool deleted = await _vehicleRepository.DeleteAsync(vehicleId);
            if (!deleted)
                return (false, "Could not delete vehicle record.");

            return (true, $"Vehicle '{vehicle.Make} {vehicle.Model}' (VIN: {vehicle.VIN}) successfully decommissioned and removed.");
        }
    }
}