using System.ComponentModel.DataAnnotations;

namespace TodoManager.Models;

/// <summary>
/// Data transfer object representing a TODOitem.
/// </summary>
public class TodoDto
{
    /// <summary>
    /// Gets or sets the user associated with the TODOitem.
    /// </summary>
    [Required(ErrorMessage = "User is required")]
    public string User { get; set; }

    /// <summary>
    /// Gets or sets the description of the TODOitem.
    /// </summary>
    [Required(ErrorMessage = "Description is required")]
    [StringLength(140, ErrorMessage = "Description cannot exceed 140 characters")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the TODOitem is marked as done.
    /// </summary>
    public bool IsDone { get; set; }
}