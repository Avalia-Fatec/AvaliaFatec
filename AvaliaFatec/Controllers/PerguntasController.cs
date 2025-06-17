﻿using System;
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

namespace AvaliaFatec.Controllers
{
    public class PerguntasController : Controller
    {
        ContextMongodb _context = new ContextMongodb();
        private UserManager<ApplicationUser> _userManager;

        public PerguntasController(UserManager<ApplicationUser> userManager, ContextMongodb context)
        {
            _context = context;
            this._userManager = userManager;
        }

        // GET: Perguntas
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            return View(await _context.Pergunta.Find(p => p.CoordenadorId == userId).ToListAsync());
        }

        // GET: Perguntas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pergunta = await _context.Pergunta
              .Find(m => m.Id == id).FirstOrDefaultAsync();
            if (pergunta == null)
            {
                return NotFound();
            }

            return View(pergunta);
        }

        // GET: Perguntas/Create
        public IActionResult Create(string status)
        {
            Pergunta pergunta = new Pergunta();
            pergunta.Status = status;
            return View(pergunta);
        }

        // POST: Perguntas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Conteudo,DataPostagem,Status,CoordenadorId")] Pergunta pergunta)
        {
            if (ModelState.IsValid)
            {
                pergunta.Id = Guid.NewGuid();
                pergunta.CoordenadorId = _userManager.GetUserId(User);
                await _context.Pergunta.InsertOneAsync(pergunta);
                return RedirectToAction(nameof(Index));
            }
            return View(pergunta);
        }

        // GET: Perguntas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pergunta = await _context.Pergunta
               .Find(m => m.Id == id).FirstOrDefaultAsync();
            if (pergunta == null)
            {
                return NotFound();
            }
            return View(pergunta);
        }

        // POST: Perguntas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Conteudo,DataPostagem,Status,CoordenadorId")] Pergunta pergunta)
        {
            if (id != pergunta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Pergunta.ReplaceOneAsync(m => m.Id == pergunta.Id, pergunta);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PerguntaExists(pergunta.Id))
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
            return View(pergunta);
        }

        // GET: Perguntas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pergunta = await _context.Pergunta.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (pergunta == null)
            {
                return NotFound();
            }

            return View(pergunta);
        }

        // POST: Perguntas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _context.Pergunta.DeleteOneAsync(u => u.Id == id);
            return RedirectToAction(nameof(Index));
        }

        private bool PerguntaExists(Guid id)
        {
            return _context.Pergunta.Find(e => e.Id == id).Any();
        }
    }
}
