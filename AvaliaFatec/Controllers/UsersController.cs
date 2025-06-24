﻿using AvaliaFatec.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AvaliaFatec.Controllers
{
    public class UsersController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private readonly ContextMongodb _context = new ContextMongodb();

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var todosUsuarios = _userManager.Users.ToList();
            var coordenadores = new List<ApplicationUser>();

            foreach (var usuario in todosUsuarios)
            {
                if (await _userManager.IsInRoleAsync(usuario, "Coordenador"))
                {
                    coordenadores.Add(usuario);
                }
            }

            return View(coordenadores);
        }


        public IActionResult Create(string role)
        {
            ViewBag.Role = role;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user, string role)
        {
            if (user.Senha != user.ConfirmeSenha)
            {
                ModelState.AddModelError("ConfirmeSenha", "As senhas não coincidem.");
            }

            if (ModelState.IsValid)
            {
                string userName = user.NomeCompleto.Replace(" ", "");
                var normalizedString = userName.Normalize(NormalizationForm.FormD);

                StringBuilder sb = new StringBuilder();
                foreach (char c in normalizedString)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    {
                        sb.Append(c);
                    }
                }

                userName = sb.ToString().Normalize(NormalizationForm.FormC);
                userName = Regex.Replace(userName, @"[^a-zA-Z0-9]", "");
                if (string.IsNullOrWhiteSpace(userName))
                {
                    ModelState.AddModelError("NomeCompleto", "Não foi possível gerar um nome de usuário válido com base no nome informado.");
                    return View(user);
                }

                var appuser = new ApplicationUser
                {
                    UserName = userName,
                    Email = user.Email,
                    NomeCompleto = user.NomeCompleto
                };

                IdentityResult result = await _userManager.CreateAsync(appuser, user.Senha);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appuser, role);

                    TempData["SuccessMessage"] = "Usuário cadastrado com sucesso!";
                    return RedirectToAction("Create");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(user);
        }

        //[Authorize(Roles = "Administrador")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateRole(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = userRole.RoleName });
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Role cadastrada com sucesso!";
                    return RedirectToAction("ListarRoles");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }

        public IActionResult ListarRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteRoleConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(role);
                }
            }

            TempData["SuccessMessage"] = "Role excluída com sucesso!";
            return RedirectToAction("ListarRoles");
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return View(user); 
        }


        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,NomeCompleto,Email")] ApplicationUser user)
        {
            if (id != user.Id) return NotFound();

            var identityUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (identityUser == null) return NotFound();

            identityUser.NomeCompleto = user.NomeCompleto;
            identityUser.Email = user.Email;

            // Atualiza o UserName baseado no NomeCompleto
            string userName = user.NomeCompleto.Replace(" ", "");
            var normalizedString = userName.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            userName = sb.ToString().Normalize(NormalizationForm.FormC);
            userName = Regex.Replace(userName, @"[^a-zA-Z0-9]", "");
            identityUser.UserName = userName;
            identityUser.NormalizedUserName = userName.ToUpperInvariant();

            var result = await _userManager.UpdateAsync(identityUser);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Usuário atualizado com sucesso!";
                return RedirectToAction(nameof(MeuPerfil));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(identityUser);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var perguntasDoUsuario = await _context.Pergunta.Find(p => p.IdCoordenador == id).ToListAsync();
            foreach (var pergunta in perguntasDoUsuario)
            {
                await _context.Avaliacao.DeleteManyAsync(a => a.PerguntaId == pergunta.Id);
            }

            await _context.Pergunta.DeleteManyAsync(p => p.IdCoordenador == id);

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(user);
            }

            TempData["SuccessMessage"] = "Usuário excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> MeuPerfil()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return View(user); 
        }

        private bool UserExists(Guid id)
        {
            return _context.User.Find(e => e.Id == id).Any();
        }
    }
}