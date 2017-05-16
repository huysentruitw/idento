using Idento.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Idento.Domain.Stores;
using Idento.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Idento.Web.Controllers
{
    //todo: implement Change password functionality
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IApplicationStore _applicationStore;
        private readonly IUserApplicationStore _userApplicationStore;

        public AccountController(UserManager<User> userManager, IApplicationStore applicationStore,
            IUserApplicationStore userApplicationStore)
        {
            _userManager = userManager;
            _applicationStore = applicationStore;
            _userApplicationStore = userApplicationStore;
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
            var model = new RegisterViewModel();
            model.AvailableApplications = _applicationStore.GetAll().Result;
            return View("CreateOrUpdate", model);
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password == null || model.ConfirmedPassword == null)
                {
                    ModelState.AddModelError("Password", "Password and Confirm password are required");
                    return View("CreateOrUpdate", model);
                }

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
                        var storedUser = await _userManager.FindByEmailAsync(user.Email);
                        if (AddApplicationsForUser(storedUser.Id, model).Result)
                        {
                            return RedirectToAction(nameof(List));
                        }
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
            var availableApplications = await _applicationStore.GetAll();
            var userApplications = _userApplicationStore.FindUserApplicationsByUserId(id);
            var selectedApplications = new List<Guid>();

            if (userApplications.Result != null)
            {
                userApplications?.Result.ToAsyncEnumerable().ForEach(x => selectedApplications.Add(x.ApplicationId));
            }

            return View("CreateOrUpdate", new RegisterViewModel
            {
                Id = id,
                Email = account.UserName,
                FirstName = account.FirstName,
                LastName = account.LastName,
                AvailableApplications = availableApplications,
                SelectedApplications = selectedApplications
            });
        }

        [HttpPost]
        [Route("Update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id, RegisterViewModel model)
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
                    var availableApplications = _applicationStore.GetAll().Result;
                    model.AvailableApplications = availableApplications;
                    // update userApplications with new selected applications for user
                    var userApplicationsResult =
                        UpdateUserApplications(user, model.AvailableApplications, model.SelectedApplications);

                    if (identityResult.Succeeded && userApplicationsResult.Result)
                    {
                        return RedirectToAction(nameof(List));
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
                if (Guid.Empty.Equals(id) || !model.Id.Equals(id)) throw new ArgumentException("Invalid Id in model");

                User user;
                user = await _userManager.FindByIdAsync(id.ToString());

                if (user != null)
                {
                    var identityResult =
                        await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

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

        public async Task<bool> AddApplicationsForUser(Guid id,
            RegisterViewModel createOrUpdateUserApplicationModel)
        {
            var user = _userManager.FindByIdAsync(id.ToString());

            var userApplications = new List<UserApplications>();

            if (createOrUpdateUserApplicationModel.SelectedApplications.Count == 0) return true;

            try
            {
                foreach (var selectedApplicationId in createOrUpdateUserApplicationModel.SelectedApplications)
                {
                    var userApplication = new UserApplications();
                    userApplication.User = user.Result;
                    userApplication.UserId = id;
                    userApplication.Application = _applicationStore.FindById(selectedApplicationId).Result;
                    userApplication.ApplicationId = selectedApplicationId;

                    userApplications.Add(userApplication);

                    await _userApplicationStore.Create(user.Result,
                        _applicationStore.FindById(selectedApplicationId).Result);
                }
                return true;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UserApplications",
                    $"There was a problem when processing the applications for user {user.Id}");
                return false;
            }
        }

        public async Task<bool> UpdateUserApplications(User user,
            IEnumerable<Application> availableApplications,
            IEnumerable<Guid> selectedApplications)
        {
            var storedSelectedApplications = _userApplicationStore.FindApplicationsByUserId(user.Id);

            if (availableApplications == null)
            {
                return true;
            }

            try
            {
                foreach (var application in availableApplications)
                {
                    var isApplicationStoredForUser =
                        storedSelectedApplications.Result?.Any(x => x.Id == application.Id) ?? false;
                    var isApplicationSelected = selectedApplications?.Any(x => x == application.Id) ?? false;

                    if (isApplicationStoredForUser && !isApplicationSelected)
                    {
                        var userApplication =
                            _userApplicationStore.FindUserApplicationsByUserIdAndApplicationId(user.Id, application.Id);
                        await _userApplicationStore.Delete(userApplication.Result.Id);
                    }

                    if (!isApplicationStoredForUser && isApplicationSelected)
                    {
                        await _userApplicationStore.Create(user, application);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UserApplications",
                    $"There was a problem when processing the applications for user {user.Id}");

                return false;
            }

            return true;
        }
    }
}