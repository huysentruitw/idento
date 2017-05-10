using Idento.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Idento.Web.Controllers
{
    //todo: implement Change password functionality
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("")]
        // GET: /<controller>/
        public async Task<IActionResult> List()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("CreateOrUpdate", new RegisterModel());
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.Password.Equals(model.ConfirmedPassword))
                {
                    ModelState.AddModelError("Password", "Password and Confirm password must be the same");
                    return View("CreateOrUpdate", model);
                }

                User user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    };
                    var identityResult =
                        await _userManager.CreateAsync(user, model.Password);
                    
                    if (identityResult.Succeeded)
                    {
                        return RedirectToAction(nameof(List));
                    }

                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Email already in use");
                }
            }

            return View("CreateOrUpdate", model);
        }

        [HttpGet]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(Guid id)
        {
            var account = await _userManager.FindByIdAsync(id.ToString());
            if (account == null) return NotFound();
            return View("CreateOrUpdate", new RegisterModel
            {
                Id = id,
                Email = account.UserName,
                FirstName = account.FirstName,
                LastName = account.LastName
            });
        }

        [HttpPost]
        [Route("Update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id, RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.Id.HasValue || model.Id.Value != id) throw new ArgumentException("Invalid Id in model");
                var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
                if (userWithSameEmail == null || userWithSameEmail.Id == id)
                {
                    User user = await _userManager.FindByIdAsync(id.ToString());
                    user.UserName = model.Email;
                    user.Email = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    var identityResult = await _userManager.UpdateAsync(user);

                    if (identityResult.Succeeded)
                    {
                        return RedirectToAction(nameof(List));
                    }
                }
                ModelState.AddModelError("Email", "Email already in use");
            }
            return View("CreateOrUpdate", model);
        }

        [HttpGet]
        [Route("ChangePassword/{id}")]
        public async Task<IActionResult> ChangePassword(Guid id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null) return NotFound();

            return View(new ChangePasswordAccountViewModel {Id = id, Email = user.UserName});
        }

        [HttpPost]
        [Route("ChangePassword/{id}")]
        public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordAccountViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                if(Guid.Empty.Equals(id) || !model.Id.Equals(id)) throw new ArgumentException("Invalid Id in model");

                User user;
                user = await _userManager.FindByIdAsync(id.ToString());

                if (user != null)
                {
                    var identityResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                    if (identityResult.Succeeded)
                    {
                        return RedirectToAction("Update", "Account", model.Id);
                    }

                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
            }

            return View("ChangePassword", model);
        }

        [HttpGet]
        [Route("ConfirmDelete/{id}")]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null) return NotFound();

            return View(new ConfirmDeleteAccountViewModel {Id = id, Email = user.UserName});
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(List));
        }

        
    }

    public class ChangePasswordAccountViewModel
    {
        [Required]
        public Guid Id { get; set; }
        public string Email { get; set; }
        [Required, MaxLength(256)]
        public string CurrentPassword { get; set; }
        [Required, MaxLength(256)]
        public string NewPassword { get; set; }
    }

    public class ConfirmDeleteAccountViewModel
    {
        public Guid Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }

    public class CreateOrUpdateAccountViewModel
    {
        public string Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }

    public class RegisterModel
    {
        public Guid? Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm password is required")]
        public string ConfirmedPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}