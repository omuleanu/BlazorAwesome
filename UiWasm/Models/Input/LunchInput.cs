using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UiWasm.Models.Input
{
    public class LunchInput
    {
        public int? Id { get; set; }

        public string Person { get; set; }

        public string Food { get; set; }

        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        public DateTime? Date { get; set; }

        public int? ChefId { get; set; }

        [Display(Name = "Meals")]
        public IEnumerable<int> MealsIds { get; set; }
    }
}