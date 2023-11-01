namespace TodoManager;

/// <summary>
/// Represents the settings required to connect to a CosmosDB instance.
/// </summary>
public class CosmosDbSettings
{
    /// <summary>
    /// Gets or sets the CosmosDB endpoint URL.
    /// </summary>
    public string Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the master key for accessing the CosmosDB instance.
    /// </summary>
    public string MasterKey { get; set; }

    /// <summary>
    /// Gets or sets the name of the database within the CosmosDB.
    /// </summary>
    public string DatabaseName { get; set; }

    /// <summary>
    /// Gets or sets the name of the container within the specified database.
    /// </summary>
    public string ContainerName { get; set; }
}