// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using newUser.Data;
using Project.Models;

namespace Project.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public IFormFile Image { get; set; }

        [BindProperty]
        public string ImageDescription { get; set; }

        public async Task<IActionResult> OnPostUploadImage()
        {
            if (User.IsInRole("Artist"))
            {
                var user = await _userManager.GetUserAsync(User);

                if (Image != null && Image.Length > 0)
                {
                    // Save the image to the server or cloud storage
                    // Example: Save the image to wwwroot/images folder
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", Image.FileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }
                    ImageDescription += " : Artist Email: "+ user.Email;
                    // Save information in the ArtistImage table
                    var artistImage = new ArtistImage
                    {
                        ArtistId = user.Id,
                        ImageDescription = ImageDescription  ,
                        ImageFilePath = imagePath
                    };

                    _dbContext.ArtistImages.Add(artistImage);
                    await _dbContext.SaveChangesAsync();
                }

                // Redirect back to the profile page after the image upload
                return RedirectToPage("/Profile");
            }

            // If the user is not an artist, handle accordingly (e.g., show an error)
            return RedirectToPage("/Profile");
        }
        // Add properties to bind exhibition details
        // Add properties to bind exhibition details
        [BindProperty]
        public Exhb Exhibition { get; set; }

        [BindProperty]
        public IFormFile ExhibitionImage { get; set; }

        // Existing methods remain unchanged

        // Add a new method to handle exhibition details and image upload
        public async Task<IActionResult> OnPostAddExhibition()
        {
            if (User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);

                if (ExhibitionImage != null && ExhibitionImage.Length > 0)
                {
                    // Save the exhibition image to the server or cloud storage
                    // Example: Save the image to wwwroot/exhibitionImages folder
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/exhibitionImages", ExhibitionImage.FileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await ExhibitionImage.CopyToAsync(stream);
                    }

                    // Save exhibition information in the Exhb table
                    var exhibition = new Exhb
                    {
                        Title = Exhibition.Title,
                        Description = Exhibition.Description,
                        Venue = Exhibition.Venue,
                        ImageFilePath = imagePath
                    };

                    _dbContext.exhb.Add(exhibition);
                    await _dbContext.SaveChangesAsync();
                }

                // Redirect back to the profile page after adding the exhibition
                return RedirectToPage("/Profile");
            }

            // If the user is not an admin, handle accordingly (e.g., show an error)
            return RedirectToPage("/Profile");
        }





        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
