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

namespace AvaliaFatec.Controllers
{
    public class AvaliacoesController : Controller
    {
        ContextMongodb _context = new ContextMongodb();

        public AvaliacoesController(ContextMongodb context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var perguntas = await _context.Pergunta.Find(_ => true).ToListAsync();
            return View(perguntas);
        }

        // GET: Avaliacoes/Create
        public async Task<IActionResult> Create(Guid perguntaId)
        {
            var pergunta = await _context.Pergunta.Find(p => p.Id == perguntaId).FirstOrDefaultAsync();

            if (pergunta == null)
                return NotFound();

            ViewBag.Pergunta = pergunta;
            return View(new Avaliacao { PerguntaId = perguntaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Emoji,PerguntaId")] Avaliacao avaliacao)
        {
            if (ModelState.IsValid)
            {
                avaliacao.Id = Guid.NewGuid();
                avaliacao.Data = DateTime.Now;
                await _context.Avaliacao.InsertOneAsync(avaliacao);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
