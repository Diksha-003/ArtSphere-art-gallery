using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using newUser.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Roles = "Admin,User,Artist")]
public class ArtistImagesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ArtistImagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var artistImages = await _context.ArtistImages.ToListAsync();
        return View(artistImages);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Only allow Admin role to delete images
    public IActionResult DeleteImage(int imageId)
    {
        var image = _context.ArtistImages.Find(imageId);

        if (image == null)
        {
            return NotFound(); // Image not found
        }

        _context.ArtistImages.Remove(image);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Artist")]
    public async Task<IActionResult> UserImages()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(currentUserId) || !User.IsInRole("Artist"))
        {
            return Forbid(); // User is not authorized
        }

        var userImages = await _context.ArtistImages
            .Where(img => img.ArtistId == currentUserId)
            .ToListAsync();

        return View(userImages);
    }

    [Authorize(Roles = "Artist")]
    public async Task<IActionResult> EditUserImage(int? id)
    {
        if (id == null)
        {
            return NotFound(); // If id is null, return Not Found
        }

        var image = await _context.ArtistImages.FindAsync(id);

        if (image == null)
        {
            return NotFound(); // If image with given id is not found, return Not Found
        }

        return View(image);
    }

    [HttpPost]
    [Authorize(Roles = "Artist")]
    public async Task<IActionResult> EditUserImage(int id, [Bind("Id,ImageDescription,ImageFilePath")] ArtistImage artistImage)
    {
        if(id != artistImage.Id)
        {
            return NotFound();
        }
        //var image = await _context.ArtistImages.FindAsync(id);

        //if (image == null)
        //{
            //return NotFound(); // Image not found
        //}

        //var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

       // if (!User.IsInRole("Admin") && image.ArtistId != currentUserId)
        //{
            //return Forbid(); // User is not authorized to edit this image
       // }

        if (ModelState.IsValid)
        {
            try
            {
                //image.ImageDescription = artistImage.ImageDescription;
                //image.ImageFilePath = artistImage.ImageFilePath;

                _context.Update(artistImage);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageExists(artistImage.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                } // Image not found
            }

            return RedirectToAction(nameof(UserImages));
        }

        return View(artistImage);
    }

    [HttpPost]
    [Authorize(Roles = "Artist")]
    public IActionResult DeleteUserImage(int imageId)
    {
        var image = _context.ArtistImages.Find(imageId);

        if (image == null)
        {
            return NotFound(); // Image not found
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!User.IsInRole("Admin") && image.ArtistId != currentUserId)
        {
            return Forbid(); // User is not authorized to delete this image
        }

        _context.ArtistImages.Remove(image);
        _context.SaveChanges();

        return RedirectToAction(nameof(UserImages));
    }

    private bool ImageExists(int imageId)
    {
        return _context.ArtistImages.Any(e => e.Id == imageId);
    }
}
