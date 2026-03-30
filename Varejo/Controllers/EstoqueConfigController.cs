namespace Varejo.Controllers
{
    using global::Varejo.Data;
    using global::Varejo.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace Varejo.Controllers
    {
        public class EstoqueConfigController : Controller
        {
            private readonly VarejoDbContext _context;

            public EstoqueConfigController(VarejoDbContext context)
            {
                _context = context;
            }

            // =========================
            // EDITAR CONFIG DO PRODUTO
            // =========================
            public async Task<IActionResult> Edit(int produtoId)
            {
                var config = await _context.EstoquesConfig
                    .FirstOrDefaultAsync(e => e.ProdutoId == produtoId);

                if (config == null)
                {
                    config = new EstoqueConfig
                    {
                        ProdutoId = produtoId,
                        EstoqueMinimo = 0,
                        EstoqueMaximo = 0
                    };
                }

                return View(config);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(EstoqueConfig model)
            {
                if (!ModelState.IsValid)
                    return View(model);

                var existente = await _context.EstoquesConfig
                    .FirstOrDefaultAsync(e => e.ProdutoId == model.ProdutoId);

                if (existente == null)
                {
                    _context.EstoquesConfig.Add(model);
                }
                else
                {
                    existente.EstoqueMinimo = model.EstoqueMinimo;
                    existente.EstoqueMaximo = model.EstoqueMaximo;

                    _context.EstoquesConfig.Update(existente);
                }

                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Configuração atualizada.";

                return RedirectToAction("Index", "Estoque");
            }
        }
    }
}
