using Newtonsoft.Json;

namespace TodoManager.Models;

/// <summary>
/// Represents a TODOitem.
/// </summary>
public class Todo
{
    /// <summary>
    /// Gets or sets the unique identifier of the TODOitem.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the user associated with the TODOitem.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Gets or sets the description of the TODOitem.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the TODOitem is marked as done.
    /// </summary>
    public bool IsDone { get; set; }
}