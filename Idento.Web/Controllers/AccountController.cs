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
                User user = await _userManager.FindByNameAsync(model.UserName);

                    if (user == null)
                    {
                        var identityResult =
                            await _userManager.CreateAsync(new User
                            {
                                UserName = model.UserName,
                                FirstName =  model.FirstName,
                                LastName =  model.LastName
                            }, model.Password);

                        if (identityResult.Succeeded)
                        {
                            return RedirectToAction(nameof(List));
                        }
                    }
                ModelState.AddModelError("Username", "Username already in use");
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
                UserName = account.UserName,
                FirstName = account.FirstName,
                LastName =  account.LastName
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
                var userWithSameName = await _userManager.FindByNameAsync(model.UserName);
                if (userWithSameName == null || userWithSameName.Id == id)
                {
                    User user = await _userManager.FindByIdAsync(id.ToString());
                    user.UserName = model.UserName;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    var identityResult = await _userManager.UpdateAsync(user);

                    if (identityResult.Succeeded)
                    {
                        return RedirectToAction(nameof(List));
                    }
                }
                ModelState.AddModelError("Username", "Username already in use");
            }
            return View("CreateOrUpdate", model);
        }

        public IActionResult ChangePassword(Guid? id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("ConfirmDelete/{id}")]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null) return NotFound();

            return View(new ConfirmDeleteAccountViewModel {Id = id, UserName = user.UserName});
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

    public class ConfirmDeleteAccountViewModel
    {
        public Guid Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string UserName { get; set; }
    }

    public class CreateOrUpdateAccountViewModel
    {
        public string Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string UserName { get; set; }
    }

    public class RegisterModel
    {
        public Guid? Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string UserName { get; set; }

        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}