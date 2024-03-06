using Microsoft.AspNetCore.Identity;

namespace Project.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<ArtistImage> ArtistImages { get; set; }
    }
}
