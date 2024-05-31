using System.ComponentModel.DataAnnotations;

namespace UiWasm.Models.Input
{
    public class TreeNodeInput : EntityInput
    {
        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; }
    }
}