using System;
using System.Collections.Generic;
using FoodArrival.Models;

namespace FoodArrival.Services
{
    public interface IOrderTrackingService
    {
        OrderTrackingViewModel GetTrackingDetails(int orderId, int customerId);
        List<Order> GetCustomerOrders(int customerId);
        bool SubmitRating(OrderRating rating);
        bool CancelOrder(int orderId, int customerId, string reason);
    }
}
