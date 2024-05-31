using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UiWasm.Data;

namespace UiWasm.Models.Input
{
    public class DinnerInput : IValidatableObject
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        [Required]
        public int? ChefId { get; set; }

        [Required]
        [Display(Name = "Meals")]
        public IEnumerable<int> MealsIds { get; set; }

        [Required]
        public int? BonusMealId { get; set; }

        public bool Organic { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Db.Dinners.Any(d => d.Name == Name && (d.Id != Id)))
            {
                yield return new ValidationResult($"Name {Name} not unique", new[] { "Name" });
            }
        }
    }
}