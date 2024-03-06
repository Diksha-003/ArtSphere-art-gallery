using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Exhb
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Venue { get; set; }

        [Required]
        public string ImageFilePath { get; set; }
    }
}
