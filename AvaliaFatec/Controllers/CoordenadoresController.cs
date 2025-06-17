    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using AvaliaFatec.Data;
    using AvaliaFatec.Models;
    using MongoDB.Driver;
    using Microsoft.AspNetCore.Identity;
    using System.Text;
    using System.Globalization;
    using System.Text.RegularExpressions;

    namespace AvaliaFatec.Controllers
    {
        public class CoordenadoresController : Controller
        {
            ContextMongodb _context = new ContextMongodb();
            private UserManager<ApplicationUser> _userManager;
            public CoordenadoresController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
            {
                this._userManager = userManager;
            }


            [HttpGet]
            public IActionResult Create()
            {
                return View();
            }
            [HttpPost]
            public async Task<IActionResult> Create(Coordenador coordenador)
            {
                if (coordenador.Senha != coordenador.ConfirmeSenha)
                {
                    ModelState.AddModelError("ConfirmeSenha", "As senhas não coincidem.");
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser appuser = new ApplicationUser();
                    string userName = coordenador.NomeCompleto.Replace(" ", "");
                    var normalizedString = userName.Normalize(NormalizationForm.FormD);

                    StringBuilder sb = new StringBuilder();
                    foreach (char c in normalizedString)
                    {
                        // Apenas mantém os caracteres que não são diacríticos (acentos, til, etc.)
                        if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                        {
                            sb.Append(c);
                        }
                    }

                    userName = sb.ToString().Normalize(NormalizationForm.FormC);

                    userName = Regex.Replace(userName, @"[^a-zA-Z0-9\s]", "");

                    Console.WriteLine(userName);

                    appuser.UserName = userName;
                    appuser.Email = coordenador.Email;
                    appuser.NomeCompleto = coordenador.NomeCompleto;
                    IdentityResult result = await _userManager.CreateAsync(appuser, coordenador.Senha);
                    if (result.Succeeded)
                    {
                        ViewBag.Message = "Usuário Cadastrado com sucesso";
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                    //inserindo uma mensagem de que o coordenador foi cadastrado com sucesso
                    TempData["SuccessMessage"] = "Coordenador cadastrado com sucesso!";
                    return RedirectToAction("Create");
                }
                return View();
            }

            // GET: Coordenadores/Edit/5
            public async Task<IActionResult> Edit(Guid? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var coordenador = await _context.Coordenador
                    .Find(m => m.Id == id).FirstOrDefaultAsync();
                if (coordenador == null)
                {
                    return NotFound();
                }
                return View(coordenador);
            }

            // POST: Coordenadores/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(Guid id, [Bind("Id,NomeCompleto,Email,Senha")] Coordenador coordenador)
            {
                if (id != coordenador.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        await _context.Coordenador.ReplaceOneAsync
                            (m => m.Id == coordenador.Id, coordenador);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CoordenadorExists(coordenador.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(coordenador);
            }

            // GET: Coordenadores/Delete/5
            public async Task<IActionResult> Delete(Guid? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var coordenador = await _context.Coordenador.Find
                    (m => m.Id == id).FirstOrDefaultAsync();
                if (coordenador == null)
                {
                    return NotFound();
                }

                return View(coordenador);
            }

            // POST: Coordenadores/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(Guid id)
            {
                await _context.Coordenador.DeleteOneAsync
                    (u => u.Id == id);
                return RedirectToAction(nameof(Index));
            }

            private bool CoordenadorExists(Guid id)
            {
                return _context.Coordenador.Find(e => e.Id == id).Any();
            }
        }
    }
