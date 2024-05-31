using System.ComponentModel.DataAnnotations;

namespace UiServer.Models.Input
{
    public class TreeNodeInput : EntityInput
    {
        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; }
    }
}