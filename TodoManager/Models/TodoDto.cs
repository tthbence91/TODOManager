using System.ComponentModel.DataAnnotations;

namespace TodoManager.Models
{
    public class TodoDto
    {
        [Required(ErrorMessage = "User is required")]
        public string User { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(140, ErrorMessage = "Description cannot exceed 140 characters")]
        public required string Description { get; set; }

        public bool IsDone { get; set; }
    }
}
