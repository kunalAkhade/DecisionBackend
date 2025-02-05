using DecisionBackend.Models.Domain;

namespace DecisionBackend.DTO
{
    public class TaskResDTO
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }

        public Models.Domain.StatusType Status { get; set; }


        public Category? Category { get; set; }

        public Models.Domain.Priority PriorityType { get; set; }
    }
}
