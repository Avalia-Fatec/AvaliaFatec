using AvaliaFatec.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace AvaliaFatec.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser>? _userManager;
        private SignInManager<ApplicationUser>? _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            if (ModelState.IsValid)
            {
                var appUser = await _userManager.FindByEmailAsync(user.Email);
                if (appUser != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(appUser, user.Senha, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Credenciais inválidas.");
            }

            return View(user);
        }

        //criando o método logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
    }
}
