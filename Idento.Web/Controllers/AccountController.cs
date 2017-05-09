using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Idento.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Idento.Web.Controllers
{
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
                if (await _userManager.FindByNameAsync(model.Username) == null)
                {
                    var identityResult =
                        await _userManager.CreateAsync(new User {UserName = model.Username}, model.Password);

                    if (identityResult.Succeeded)
                    {
                        return RedirectToAction(nameof(List));
                    }
                }
                ModelState.AddModelError("Username", "Username already in use");
            }
            //ModelState.AddModelError("", identityResult.Errors.FirstOrDefault().ToString());

            return View("CreateOrUpdate", model);
        }

       /* [HttpGet]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(Guid id)
        {
            var account = await _userManager..FindByIdAsync(id);
            if (account == null) return NotFound();
            return View("CreateOrUpdate", new RegisterModel() { Id = id, Username = account.UserName});
        }*/
    }

    public class CreateOrUpdateAccountViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
    }

    public class RegisterModel
    {
        public Guid? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}