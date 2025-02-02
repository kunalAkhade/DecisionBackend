using DecisionBackend.Models.Domain;
using System.ComponentModel.DataAnnotations;


namespace DecisionBackend.DTO
{
    
    public class TaskDTO
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public Models.Domain.StatusType Status { get; set; }


        public string? Category { get; set; }

        public Models.Domain.Priority PriorityType { get; set; }    
    }
}
