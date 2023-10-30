using Newtonsoft.Json;

namespace TodoManager.Models
{
    public class Todo
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
    }
}
