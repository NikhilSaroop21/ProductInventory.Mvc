using System.ComponentModel.DataAnnotations;

namespace ProductInventory.Mvc.Models
{
    public class Product
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductID must be a positive integer.")]
        public int ProductID { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required, Range(0, 1_000_000)]
        public decimal Price { get; set; }
    }
}
