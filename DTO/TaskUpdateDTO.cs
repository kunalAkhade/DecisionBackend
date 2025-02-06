namespace DecisionBackend.DTO
{
    public class TaskUpdateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Models.Domain.StatusType? Status { get; set; }
        public Models.Domain.Priority? PriorityType { get; set; }
        public string? CategoryId { get; set; }
    }
}
