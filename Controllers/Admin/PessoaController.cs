using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebCarteiraMvc.Models;
using WebCarteiraMvc.ObjetosDeValor;
using WebCarteiraMvc.Repositorio;

namespace WebCarteiraMvc.Controllers.Admin
{
    public class PessoaController : Controller
    {

        private readonly RepositorioPessoa pesRep;
        private readonly INotyfService _toastNotification;
        public PessoaController(NHibernate.ISession session, INotyfService toastNotification)
        {
            pesRep = new RepositorioPessoa(session);
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {

            if (HttpContext.Session.GetString("User") == null)
            {
                _toastNotification.Error("Necessário fazer login para visualizar está página");
                return RedirectToAction("Index", "Home");
            }

            var usuarioLogado = JsonSerializer.Deserialize<Pessoa>(HttpContext.Session.GetString("User"));
            if(usuarioLogado.Id == 1)
            {
                ModelParaViewPessoa pessoas = new ModelParaViewPessoa(pesRep.FindAll().ToList());
                return View("Index", pessoas);
            }
                _toastNotification.Error("Acesso negado!");
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
        }
        public ActionResult PesquisaFiltro(string nome)
        {
            var pessoa = pesRep.FindByName(nome).ToList();
            ModelParaViewPessoa pessoasFiltradas = new ModelParaViewPessoa(pessoa);
            return View("Index", pessoasFiltradas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind("Id, Nome, Email, Senha, Salario, Limite, Minimo")]
            Pessoa pessoa)

        {
            if (ModelState.IsValid)
            {
                await pesRep.Add(pessoa);
                return RedirectToAction("Index");
            }
            return View(pessoa);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            Pessoa pessoa = await pesRep.FindById(id.Value);
            if (pessoa == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return View(pessoa);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind("Id, Nome, Email, Senha, Salario, Limite, Minimo")]
            Pessoa pessoa)

        {
            if (ModelState.IsValid)
            {
                await pesRep.Update(pessoa);
                return RedirectToAction("Index");
            }
            return View(pessoa);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            Pessoa pessoa = await pesRep.FindById(id.Value);

            if (pessoa == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return View(pessoa);
        }

        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await pesRep.Remove(id);
            return RedirectToAction("Index");
        }

    

    }
}
