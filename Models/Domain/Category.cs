using System.ComponentModel.DataAnnotations;

namespace DecisionBackend.Models.Domain
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? CategoryName { get; set; }

        public User? User { get; set; }

        public List<Task> Tasks { get; set; } = new List<Task>();
    }
}
