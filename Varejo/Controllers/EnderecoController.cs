using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class EnderecoController : Controller
    {

        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IPessoaRepository _pessoaRepository; 

        public EnderecoController(IEnderecoRepository enderecoRepository, IPessoaRepository pessoa)
        {
            _enderecoRepository = enderecoRepository;
            _pessoaRepository = pessoa;
        }

       
        public async Task<EnderecoViewModel> CriarEnderecoViewModel(EnderecoViewModel? model = null)
        {
            var pessoas = await _pessoaRepository.GetAllAsync();
            var endereco = await _enderecoRepository.GetAllAsync();

            return new EnderecoViewModel { 
            
            IdEndereco = model?.IdEndereco ?? 0,
            Logradouro = model?.Logradouro,
            Cep = model.Cep,
            Bairro = model.Bairro,
            Cidade = model.Cidade,
            Uf = model.Uf,
            Complemento = model.Complemento,
            Numero = model.Numero,
            PessoaId = model.PessoaId,
            };
        }



        public async Task<IActionResult> Index(int? enderecoId, string? search)
        {
            var endereco = await _enderecoRepository.GetAllAsync();

            //filtro
            if (enderecoId.HasValue && enderecoId.Value > 0)
                endereco = endereco.Where(f => f.IdEndereco == enderecoId).ToList();

            //search
            if (!string.IsNullOrEmpty(search))
                endereco = endereco.Where(f => f.Cidade.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            //ordenar decrescernte
            endereco = endereco.OrderByDescending(f => f.IdEndereco).ToList();

            //componentes
            ViewBag.Generos = new SelectList(await _enderecoRepository.GetAllAsync(), "IdEndereco", "Cidade");
            ViewBag.FiltroGeneroId = enderecoId;
            ViewBag.Search = search;

            return View(endereco);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = await CriarEnderecoViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnderecoViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var filme = new Endereco
                {

                    IdEndereco = model?.IdEndereco ?? 0,
                    Logradouro = model?.Logradouro,
                    Cep = model.Cep,
                    Bairro = model.Bairro,
                    Cidade = model.Cidade,
                    Uf = model.Uf,
                    Complemento = model.Complemento,
                    Numero = model.Numero,
                    PessoaId = model.PessoaId,

                };

                await _enderecoRepository.AddAsync(filme);
                return RedirectToAction(nameof(Index));
            }
            model = await CriarEnderecoViewModel(model);
            return View(model);

        }

        public async Task<IActionResult> Edit(int id)
        {
            var endereco = await _enderecoRepository.GetByIdAsync(id);
            if (endereco == null) return NotFound();
            return View(endereco);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Endereco endereco)
        {
            if (id != endereco.IdEndereco) return NotFound();
            if (ModelState.IsValid)
            {
                await _enderecoRepository.UpdateAsync(endereco);
                return RedirectToAction(nameof(Index));
            }
            return View(endereco);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var endereco = await _enderecoRepository.GetByIdAsync(id);
            if (endereco == null) return NotFound();
            return View(endereco);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _enderecoRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
