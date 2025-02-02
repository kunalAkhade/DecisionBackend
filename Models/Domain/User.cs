using System.ComponentModel.DataAnnotations;

namespace DecisionBackend.Models.Domain
{
    public class User
    {
        [Key]  
        public string? Username { get; set; }

        public string? Password { get; set; }


        public List<Category> Categories { get; set; }= new List<Category>();

        public List<Task> Tasks { get; set; } = new List<Task>();

    }
}
