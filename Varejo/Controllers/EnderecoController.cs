using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class EnderecoController : Controller
    {

        private readonly IEnderecoRepository _endereco;
        private readonly IPessoaRepository _pessoa; 

        public EnderecoController(IEnderecoRepository enderecoRepository, IPessoaRepository pessoa)
        {
            _endereco = enderecoRepository;
            _pessoa = pessoa;
        }

       
        public async Task<EnderecoViewModel> CriarEndereco(EnderecoViewModel? model = null)
        {
            var pessoas = await _pessoa.GetAllAsync();
            var endereco = await _endereco.GetAllAsync();

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




       public IActionResult Create() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Endereco endereco)
        {
            if (ModelState.IsValid)
            {
                await _endereco.AddAsync(endereco);
                return RedirectToAction(nameof(Index));
            }
            return View(endereco);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var endereco = await _endereco.GetByIdAsync(id);
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
                await _endereco.UpdateAsync(endereco);
                return RedirectToAction(nameof(Index));
            }
            return View(endereco);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var endereco = await _endereco.GetByIdAsync(id);
            if (endereco == null) return NotFound();
            return View(endereco);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _endereco.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
