using AvaliaFatec.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace AvaliaFatec.Controllers
{
    public class AccountController : Controller
    {
        //criando um tipo de verificação
        private UserManager<ApplicationUser> _UserManager;
        //criando a verificação para conseguir fazer o login
        private SignInManager<ApplicationUser> _signInManager;

        // criando o metodo construtor dessa classe
        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            this._UserManager = userManager;
            this._signInManager = signInManager;
        }
        //mudando o index para login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Coordenador model)
        {
            if (ModelState.IsValid)
            {
                var user = await _UserManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Senha, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "Credenciais inválidas");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário não encontrado");
                }
            }

            return View(model);
        }
        //criando o método logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
    }
}
