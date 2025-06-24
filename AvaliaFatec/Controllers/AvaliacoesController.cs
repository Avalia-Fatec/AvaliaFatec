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
using MongoDB.Bson;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace AvaliaFatec.Controllers
{
    public class AvaliacoesController : Controller
    {
        ContextMongodb _context = new ContextMongodb();
        private UserManager<ApplicationUser> _userManager;

        public AvaliacoesController(ContextMongodb context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var perguntasDisponiveis = await _context.Pergunta
                .Find(p => p.Status == "Disponível")
                .ToListAsync();

            return View(perguntasDisponiveis);
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
            var userId = _userManager.GetUserId(User);

            if (!Guid.TryParse(userId, out Guid usuarioGuid))
            {
                TempData["Mensagem"] = "Usuário inválido.";
                return RedirectToAction(nameof(Index));
            }

            var filtro = Builders<Avaliacao>.Filter.And(
                Builders<Avaliacao>.Filter.Eq(a => a.PerguntaId, avaliacao.PerguntaId),
                Builders<Avaliacao>.Filter.Eq(a => a.UsuarioId, usuarioGuid),
                Builders<Avaliacao>.Filter.Gt(a => a.Data, DateTime.Now.AddDays(-7))
            );

            var avaliacaoExistente = await _context.Avaliacao.Find(filtro).FirstOrDefaultAsync();

            if (avaliacaoExistente != null)
            {
                TempData["Mensagem"] = "Você já avaliou essa pergunta nos últimos 7 dias.";
                return RedirectToAction(nameof(Index));
            }

            avaliacao.Id = Guid.NewGuid();
            avaliacao.Data = DateTime.Now;
            avaliacao.UsuarioId = usuarioGuid;

            await _context.Avaliacao.InsertOneAsync(avaliacao);

            TempData["Mensagem"] = "Avaliação registrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Grafico()
        {
            var userId = _userManager.GetUserId(User);

            // Busca as perguntas do coordenador logado
            var perguntas = await _context.Pergunta
                .Find(p => p.IdCoordenador == userId)
                .ToListAsync();

            var graficos = new List<object>();

            foreach (var pergunta in perguntas)
            {
                var pipeline = new[]
                {
                    new BsonDocument("$match", new BsonDocument("PerguntaId", pergunta.Id)),
                    new BsonDocument("$group", new BsonDocument
                    {
                        { "_id", BsonNull.Value },
                        { "satisfeito", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                            {
                                new BsonDocument("$eq", new BsonArray { "$Emoji", "Satisfeito" }),
                                1,
                                0
                            }))
                        },
                        { "neutro", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                            {
                                new BsonDocument("$eq", new BsonArray { "$Emoji", "Neutro" }),
                                1,
                                0
                            }))
                        },
                        { "insatisfeito", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                            {
                                new BsonDocument("$eq", new BsonArray { "$Emoji", "Insatisfeito" }),
                                1,
                                0
                            }))
                        }
                    })
                };

                var result = await _context.Avaliacao.AggregateAsync<BsonDocument>(pipeline);
                var data = await result.FirstOrDefaultAsync();

                var categorias = new[] { "Satisfeito", "Neutro", "Insatisfeito" };
                var valores = new[]
                {
                    data?["satisfeito"].AsInt32 ?? 0,
                    data?["neutro"].AsInt32 ?? 0,
                    data?["insatisfeito"].AsInt32 ?? 0
                };

                graficos.Add(new
                {
                    Pergunta = pergunta.Conteudo,
                    Categorias = categorias,
                    Valores = valores
                });
            }

            ViewBag.Graficos = graficos;
            return View();
        }
    }
}
