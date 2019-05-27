using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServerAsp.Models;
using IdentityServerAsp.Models.RoleViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerAsp.Quickstart.Account
{
    [Authorize]
    public class RoleController : Controller
    {
        public RoleManager<IdentityRole> roleManager { get; set; }
        public UserManager<ApplicationUser> userManager { get; set; }
        public RoleController(RoleManager<IdentityRole> _roleManager, UserManager<ApplicationUser> _userManager)
        {
            roleManager = _roleManager;
            userManager = _userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var roleInDb = await roleManager.FindByNameAsync(model.Name);
            if (roleInDb != null)
                return View(model);

            try
            {
                var res = await roleManager.CreateAsync(new IdentityRole(model.Name));
                if (res.Succeeded)
                    return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
            }

            ViewBag.Error = "Error occured while saving role";
            return View(model);

        }

        public async Task<bool> AddAdminRole()
        {
            await roleManager.CreateAsync(new IdentityRole("admin"));
            var role = roleManager.FindByNameAsync("admin").Result;
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Read", "All"));
            var alice = userManager.FindByNameAsync("alice").Result;
            var res = await userManager.AddToRoleAsync(alice, "admin");
            return res.Succeeded;
        }

        public async Task<List<string>>CheckUser()
        {
            var claims = User.Claims.Select(c=>c.ToString()).ToList();
            claims.Add(userManager.IsInRoleAsync(await userManager.GetUserAsync(User), "admin").Result ? "true" : "false");
            return claims;
        }


        public async Task<bool> AddRoles()
        {
           var user = await userManager.GetUserAsync(User);
           var res =  userManager.AddClaimAsync(user, new System.Security.Claims.Claim(JwtClaimTypes.Role, "admin"));
            return res.Result.Succeeded;
        }
    }
}