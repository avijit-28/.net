using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodArrival.Models
{
    public class RestaurantCard
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Cuisine { get; set; }
        public string City { get; set; }
        public string AddressLine { get; set; }
        public string ImageUrl { get; set; }
        public decimal? AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class RestaurantPageViewModel
    {
        public RestaurantCard Restaurant { get; set; }
        public IList<MenuItem> Menu { get; set; }
        public string Category { get; set; }
        public bool VegetarianOnly { get; set; }
    }

    public class CartLine
    {
        public int MenuItemId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get { return UnitPrice * Quantity; } }
    }

    public class CheckoutViewModel
    {
        [Required, StringLength(120)]
        [Display(Name = "Recipient name")]
        public string RecipientName { get; set; }
        [Required, StringLength(30)]
        [Display(Name = "Phone number")]
        public string Phone { get; set; }
        [Required, StringLength(250)]
        [Display(Name = "Street address")]
        public string AddressLine { get; set; }
        [StringLength(250)]
        [Display(Name = "Apartment, suite, etc. (optional)")]
        public string AddressLine2 { get; set; }
        [Required, StringLength(100)]
        public string City { get; set; }
        [StringLength(20)]
        [Display(Name = "Postal code")]
        public string PostalCode { get; set; }
        [StringLength(500)]
        [Display(Name = "Delivery instructions (optional)")]
        public string Instructions { get; set; }
        [Required]
        [Display(Name = "Payment method")]
        public string PaymentMethod { get; set; }
    }

    public class OrderSummary
    {
        public long OrderId { get; set; }
        public string OrderNumber { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeliveryStatus { get; set; }
    }

    public class OrderLine
    {
        public int MenuItemId { get; set; }
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class OrderDetailsViewModel
    {
        public OrderSummary Order { get; set; }
        public string RecipientName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public IList<OrderLine> Items { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string DeliveryStatus { get; set; }
    }

    public class RegisterViewModel
    {
        [Required, StringLength(120)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }
        [Required, EmailAddress, StringLength(254)]
        public string Email { get; set; }
        [Required, StringLength(30)]
        [Display(Name = "Phone number")]
        public string Phone { get; set; }
        [Required, StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Keep me signed in")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class MenuEditViewModel
    {
        public int? MenuItemId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        [Range(typeof(decimal), "0.01", "999999")]
        public decimal Price { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsAvailable { get; set; }
    }
}
