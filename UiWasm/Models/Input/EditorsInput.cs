using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Omu.BlazorAwesome.Models;

namespace UiWasm.Models.Input
{
    public class EditorsInput
    {
        [Required]
        [StringLength(10, ErrorMessage = "Name is too long.")]
        public string Textbox { get; set; }

        [Required]
        public int? Numeric { get; set; }

        [Required]
        public double? NumericFloat { get; set; }

        [Required]
        public DateTime? DatePicker { get; set; }

        [Required]
        public int? DropdownList { get; set; }

        [Required]
        public int? RadioList { get; set; }

        [Required]
        public int? RadioListNative { get; set; }

        [Required]
        public object Combobox { get; set; }

        [Required]
        public IEnumerable<int> Multiselect { get; set; }

        [Required]
        public IEnumerable<int> Multicheck { get; set; }

        [Required]
        public IEnumerable<int> CheckboxList { get; set; }

        [Required]
        public IEnumerable<int> CheckboxListNative { get; set; }

        [Required]
        public int? Select { get; set; }

        [Required]
        public bool ToggleButton { get; set; }

        [Required]
        public bool CheckboxNative { get; set; }

        [Required]
        public bool Checkbox { get; set; }

        [Required]
        public IEnumerable<int> SelectList { get; set; }

        [Required]
        public DateTime? TimePicker { get; set; }
    }
}