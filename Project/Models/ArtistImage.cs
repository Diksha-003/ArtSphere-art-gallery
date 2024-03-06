using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class ArtistImage
    {
        public int Id { get; set; }

        [Required]
        public string ArtistId { get; set; }

        [Required]
        public string ImageDescription { get; set; }

        [Required]
        public string ImageFilePath { get; set; }

        public ApplicationUser Artist { get; set; }
    }
}
