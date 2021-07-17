using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SuperShop.Models
{
    public class AddItemViewModel
    {

        [Display(Name = "Product")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a product.")]
        public int ProductId { get; set; }


        [Range(0.0001, double.MaxValue, ErrorMessage = "Invalid quantity.")]
        public double Quantity { get; set; }


        public IEnumerable<SelectListItem> Products { get; set; }
    }
}
