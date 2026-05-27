using System.ComponentModel.DataAnnotations;

namespace Weatherapplication.Models
{
    public class StudentDetails
    {
        [Key]
        public int Id { get; set; }

        public string StudentName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Course { get; set; }
    }
}
