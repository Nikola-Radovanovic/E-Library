using Library.Areas.Identity.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users.ToList();

            return View("GetUsers", users);
        }

        [HttpGet]
        public async Task<IActionResult> DisplayUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            ViewBag.UserName = user.UserName;

            var userRoles = await _userManager.GetRolesAsync(user);

            return View("DisplayUser", userRoles);
        }

        [HttpGet]
        public IActionResult CreateRoleForm()
        {
            return RedirectToAction("DisplayRoles", "Accounts");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string role)
        {
            await _roleManager.CreateAsync(new IdentityRole(role));

            return RedirectToAction("DisplayRoles", "Accounts");
        }

        [HttpGet]
        public IActionResult DisplayRoles()
        {
            var roles = _roleManager.Roles.ToList();

            return View("DisplayRoles", roles);
        }

        [HttpGet]
        public IActionResult CreateUserRoleForm()
        {
            UserRoleModel userRole = new();

            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();

            users.ForEach(u =>
            {
                userRole.UserList.Add(new SelectListItem { Value = u.Id, Text = u.UserName });
            });

            roles.ForEach(r =>
            {
                userRole.RoleList.Add(new SelectListItem { Value = r.Name, Text = r.Name });
            });

            return View("CreateUserRoleForm", userRole);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserRole(UserRoleModel userRole)
        {
            var user = await _userManager.FindByIdAsync(userRole.UserId);

            await _userManager.AddToRoleAsync(user, userRole.RoleName);

            return RedirectToAction("GetUsers", "Accounts");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUserRole(string role, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            await _userManager.RemoveFromRoleAsync(user, role);

            return RedirectToAction("DisplayUser", "Accounts", new { user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string role)
        {
            var roleForDelete = await _roleManager.FindByNameAsync(role);

            await _roleManager.DeleteAsync(roleForDelete);

            return RedirectToAction("DisplayRoles", "Accounts");
        }
    }
}