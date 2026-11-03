using System;
using System.ComponentModel.DataAnnotations;

namespace FoodArrival.Models
{
    public class FoodItem
    {
        public int FoodItemId { get; set; }

        [Required(ErrorMessage = "Item Name is required")]
        [StringLength(150)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 999999, ErrorMessage = "Price must be positive")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [StringLength(500)]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        [StringLength(100)]
        [Display(Name = "Category")]
        public string Category { get; set; } = "Snacks";

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
