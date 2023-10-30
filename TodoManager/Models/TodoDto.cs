namespace TodoManager.Models
{
    public class TodoDto
    {
        public string User { get; set; }
        public required string Description { get; set; }
        public bool IsDone { get; set; }
    }
}
