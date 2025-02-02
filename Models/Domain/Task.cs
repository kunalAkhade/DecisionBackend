using System.ComponentModel.DataAnnotations;

namespace DecisionBackend.Models.Domain
{

    public enum StatusType { Pending, Completed }

    public enum Priority { Low, Medium, High }

    public class Task
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? Title { get; set; }


        public string? Description { get; set; }


        public StatusType Status {  get; set; }

        [Required]
        public User? User { get; set; }

        public Category? Category { get; set; }

        public Priority PriorityType { get; set; }


    }
}
