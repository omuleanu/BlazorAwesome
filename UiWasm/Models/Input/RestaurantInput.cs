using System.ComponentModel.DataAnnotations;

namespace UiWasm.Models.Input
{
    public class RestaurantInput
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(35)]
        public string Name { get; set; }
    }
}