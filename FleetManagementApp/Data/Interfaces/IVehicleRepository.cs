using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetManagementApp.Models;

namespace FleetManagementApp.Data.Interfaces
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<Vehicle> GetByVinAsync(string vin);
        Task<IReadOnlyList<Vehicle>> GetActiveVehiclesAsync();
        Task<bool> UpdateTelemetryAsync(int id, decimal newOdometer, VehicleStatus newStatus);
    }
}
