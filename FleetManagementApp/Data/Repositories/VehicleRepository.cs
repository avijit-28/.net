using FleetManagementApp.Data.Interfaces;
using FleetManagementApp.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManagementApp.Data.Repositories
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(SqlHelper db) : base(db)
        {
        }
        /// <summary>
        /// Maps an ADO.NET data reader row directly to a Vehicle instance (zero ORM overhead).
        /// </summary>
        protected override Vehicle MapRecord(SqlDataReader reader)
        {
            return new Vehicle
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                VIN = reader.GetString(reader.GetOrdinal("VIN")),
                Make = reader.GetString(reader.GetOrdinal("Make")),
                Model = reader.GetString(reader.GetOrdinal("Model")),
                Year = reader.GetInt32(reader.GetOrdinal("Year")),
                OdometerReading = reader.GetDecimal(reader.GetOrdinal("OdometerReading")),
                Status = Enum.TryParse<VehicleStatus>(reader.GetString(reader.GetOrdinal("Status")), out var parsedStatus)
                    ? parsedStatus
                    : VehicleStatus.Active
            };
        }
        // CREATE: Fleet Onboarding
        public override async Task<int> AddAsync(Vehicle vehicle)
        {
            const string sql = @"
                INSERT INTO Vehicles (VIN, Make, Model, Year, OdometerReading, Status)
                VALUES (@VIN, @Make, @Model, @Year, @OdometerReading, @Status);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            var parameters = new[]
            {
                new SqlParameter("@VIN", SqlDbType.VarChar, 17) { Value = vehicle.VIN.Trim().ToUpper() },
                new SqlParameter("@Make", SqlDbType.NVarChar, 50) { Value = vehicle.Make.Trim() },
                new SqlParameter("@Model", SqlDbType.NVarChar, 50) { Value = vehicle.Model.Trim() },
                new SqlParameter("@Year", SqlDbType.Int) { Value = vehicle.Year },
                new SqlParameter("@OdometerReading", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = vehicle.OdometerReading },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = vehicle.Status.ToString() }
            };
            int generatedId = await Db.ExecuteScalarAsync<int>(sql, parameters);
            vehicle.Id = generatedId;
            return generatedId;
        }
        // READ: Fetch by Primary Key
        public override async Task<Vehicle> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT Id, VIN, Make, Model, Year, OdometerReading, Status
                FROM Vehicles
                WHERE Id = @Id;";
            var parameter = new SqlParameter("@Id", SqlDbType.Int) { Value = id };
            return await Db.ExecuteSingleAsync(sql, MapRecord, parameter);
        }
        // READ: Fetch by VIN
        public async Task<Vehicle> GetByVinAsync(string vin)
        {
            const string sql = @"
                SELECT Id, VIN, Make, Model, Year, OdometerReading, Status
                FROM Vehicles
                WHERE VIN = @VIN;";
            var parameter = new SqlParameter("@VIN", SqlDbType.VarChar, 17) { Value = vin.Trim().ToUpper() };
            return await Db.ExecuteSingleAsync(sql, MapRecord, parameter);
        }
        // READ: Fetch All
        public override async Task<IReadOnlyList<Vehicle>> GetAllAsync()
        {
            const string sql = @"
                SELECT Id, VIN, Make, Model, Year, OdometerReading, Status
                FROM Vehicles
                ORDER BY Id ASC;";
            return await Db.ExecuteReaderAsync(sql, MapRecord);
        }
        // READ: Real-time Active Fleet Inventory
        public async Task<IReadOnlyList<Vehicle>> GetActiveVehiclesAsync()
        {
            const string sql = @"
                SELECT Id, VIN, Make, Model, Year, OdometerReading, Status
                FROM Vehicles
                WHERE Status = @Status
                ORDER BY Id ASC;";
            var parameter = new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = VehicleStatus.Active.ToString() };
            return await Db.ExecuteReaderAsync(sql, MapRecord, parameter);
        }
        // UPDATE: Full Record
        public override async Task<bool> UpdateAsync(Vehicle vehicle)
        {
            const string sql = @"
                UPDATE Vehicles
                SET Make = @Make,
                    Model = @Model,
                    Year = @Year,
                    OdometerReading = @OdometerReading,
                    Status = @Status
                WHERE Id = @Id;";
            var parameters = new[]
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = vehicle.Id },
                new SqlParameter("@Make", SqlDbType.NVarChar, 50) { Value = vehicle.Make.Trim() },
                new SqlParameter("@Model", SqlDbType.NVarChar, 50) { Value = vehicle.Model.Trim() },
                new SqlParameter("@Year", SqlDbType.Int) { Value = vehicle.Year },
                new SqlParameter("@OdometerReading", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = vehicle.OdometerReading },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = vehicle.Status.ToString() }
            };
            int affectedRows = await Db.ExecuteNonQueryAsync(sql, parameters);
            return affectedRows > 0;
        }
        // UPDATE: Telemetry Logging (Mileage & Status)
        public async Task<bool> UpdateTelemetryAsync(int id, decimal newOdometer, VehicleStatus newStatus)
        {
            const string sql = @"
                UPDATE Vehicles
                SET OdometerReading = @OdometerReading,
                    Status = @Status
                WHERE Id = @Id;";
            var parameters = new[]
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = id },
                new SqlParameter("@OdometerReading", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = newOdometer },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = newStatus.ToString() }
            };
            int affectedRows = await Db.ExecuteNonQueryAsync(sql, parameters);
            return affectedRows > 0;
        }
        // DELETE: Decommissioning / Purge Record
        public override async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"
                DELETE FROM Vehicles
                WHERE Id = @Id;";
            var parameter = new SqlParameter("@Id", SqlDbType.Int) { Value = id };
            int affectedRows = await Db.ExecuteNonQueryAsync(sql, parameter);
            return affectedRows > 0;
        }
    }
}
