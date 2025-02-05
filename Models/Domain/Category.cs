using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DecisionBackend.Models.Domain
{
    public class Category
    {
        [Key]
        public Guid? Id { get; set; }

        [Required]
        public string? CategoryName { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public List<Task> Tasks { get; set; } = new List<Task>();
    }
}
