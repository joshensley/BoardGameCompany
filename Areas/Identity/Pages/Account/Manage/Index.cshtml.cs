using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BoardGameCompany.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCompany.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment environment)
        {
            _environment = environment;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Username { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }
            public string FilePath { get; set; }
            public IFormFile Upload { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var resUserManager = await _userManager.GetUserAsync(User);

            Input = new InputModel
            {
                Username = resUserManager.UserName,
                PhoneNumber = resUserManager.PhoneNumber,
                Name = resUserManager.Name,
                Address = resUserManager.Address,
                City = resUserManager.City,
                PostalCode = resUserManager.PostalCode,
                FilePath = resUserManager.UserAvatarFilePath
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


            if (Input.Upload != null)
            {
                // Loads new profile avatar
                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/images/profileAvatar/", Input.Upload.FileName);
                Input.FilePath = $"/images/profileAvatar/{Input.Upload.FileName}";

                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await Input.Upload.CopyToAsync(fileStream);
                }

                var deleteFile = Path.Combine(_environment.ContentRootPath, $"wwwroot{user.UserAvatarFilePath}");
                Debug.WriteLine(deleteFile);
                if (System.IO.File.Exists(deleteFile))
                {
                    System.IO.File.Delete(deleteFile);
                }
            }
           

            if (await TryUpdateModelAsync<ApplicationUser>(
                user,
                "input",
                a => a.PhoneNumber,
                a => a.Name,
                a => a.Address,
                a => a.City,
                a => a.PostalCode,
                a => a.UserAvatarFilePath))
            {

                if(Input.Upload != null)
                {
                    user.UserAvatarFilePath = Input.FilePath;
                }
                
                await _userManager.UpdateAsync(user);
            }
           
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
