using System;
using System.Collections.Generic;
using FoodArrival.Models;

namespace FoodArrival.Services
{
    public class AllocationRouteDto
    {
        public int RiderId { get; set; }
        public string RiderName { get; set; }
        public string VehicleType { get; set; }
        public List<RouteRestaurantStopDto> Restaurants { get; set; } = new List<RouteRestaurantStopDto>();
        public DateTime EstimatedDeliveryTime { get; set; }
        public double TotalDistanceKm { get; set; }
        public string SLAStatus { get; set; } = "On Time"; // On Time, At Risk, Delayed
        public double RouteScore { get; set; }
    }

    public class RouteRestaurantStopDto
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public int SubOrderId { get; set; }
        public int PickupSequence { get; set; }
        public DateTime EstimatedPickupTime { get; set; }
        public DateTime EstimatedReadyTime { get; set; }
    }

    public class AllocationResult
    {
        public int MasterOrderId { get; set; }
        public string OrderNumber { get; set; }
        public int RequiredRiderCount => Routes.Count;
        public List<AllocationRouteDto> Routes { get; set; } = new List<AllocationRouteDto>();
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public DateTime AllocatedAt { get; set; } = DateTime.Now;
    }

    public class RiderCandidateDto
    {
        public int RiderId { get; set; }
        public string RiderName { get; set; }
        public string VehicleType { get; set; }
        public string CurrentStatus { get; set; }
        public bool IsActive { get; set; }
        public int MaxCapacity { get; set; } = 3;
        public decimal Rating { get; set; } = 4.8m;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int CurrentWorkload { get; set; }
        public HashSet<int> ApprovedRestaurantIds { get; set; } = new HashSet<int>();
    }

    public interface IRiderAllocationService
    {
        AllocationResult AllocateRiders(int masterOrderId);
        List<RiderCandidateDto> FindEligibleRiders();
        bool CanBatchRestaurants(RiderCandidateDto rider, List<OrderSubOrder> subOrders, decimal customerLat, decimal customerLng);
        double CalculateRouteScore(RiderCandidateDto rider, List<OrderSubOrder> subOrders, decimal customerLat, decimal customerLng);
        DateTime CalculateEstimatedDeliveryTime(List<OrderSubOrder> subOrders, RiderCandidateDto rider, decimal customerLat, decimal customerLng);
        bool AssignRider(int subOrderId, int riderId, int pickupSequence, DateTime estimatedDeliveryAt);
        bool ReassignRider(int subOrderId, int newRiderId, string reason);
        AllocationResult RecalculateAssignments(int masterOrderId);
    }
}
