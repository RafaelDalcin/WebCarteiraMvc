using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebCarteiraMvc.Models;
using WebCarteiraMvc.ObjetosDeValor;
using WebCarteiraMvc.Repositorio;
using System.Collections.Generic;
using AspNetCoreHero.ToastNotification.Abstractions;
using IronPdf;

namespace WebCarteiraMvc.Controllers.Usuario
{
    public class UsuarioController : Controller
    {
        private readonly INotyfService _toastNotification;
        private readonly RepositorioEntrada _entradas;
        private readonly RepositorioPessoa _usuario;
        private readonly RepositorioSaida _saidas;
        public UsuarioController(NHibernate.ISession session, INotyfService toastNotification)
        {
            _usuario = new RepositorioPessoa(session);
            _entradas = new RepositorioEntrada(session);
            _saidas = new RepositorioSaida(session);
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
            return View(usuarioLogado);
      
        }

        [HttpPost]
        public async Task<ActionResult> CriarTransacao(int id, double valor, string descricao, int tipoMovimentacao)
        {

            double valorFormatado = Convert.ToDouble(valor.ToString().Replace('.', ','));
            string dataFormatada = DateTime.Now.ToString("dd/MM/yyyy");
            Pessoa pessoa = await _usuario.FindById(id);

            if (tipoMovimentacao != 1)
            {
                Saida saida = CriaTransacaoSaida(descricao, valorFormatado, dataFormatada, pessoa);
                if (saida.Pessoa.Saldo < 0)
                {
                    _toastNotification.Error("Saldo insuficiente para realizar esta transação!");
                    return RedirectToAction("Index");
                }
                else if (saida.Pessoa.Saldo < saida.Pessoa.Minimo)
                {
                    _toastNotification.Warning("Transação feita com sucesso, porém saldo abaixo do mínimo!");
                    AddAndUpdateSaida(saida);
                    return RedirectToAction("Index");
                }
                await AddAndUpdateSaida(saida);
                _toastNotification.Success("Saque feito com sucesso!");
            }
            else
            {
                Entrada entrada = CriaTransacaoEntrada(descricao, valorFormatado, dataFormatada, pessoa);
                await AddAndUpdateEntrada(entrada);
                _toastNotification.Success("Depósito feito com sucesso!");
            }
            return RedirectToAction("Index");
        }

        private static Saida CriaTransacaoSaida(string descricao, double valorFormatado, string dataFormatada, Pessoa pessoa)
        {
            Saida saida = new Saida
            {
                Pessoa = pessoa,
                Descricao = descricao,
                Data = DateTime.Parse(dataFormatada),
                Valor = valorFormatado
            };
            saida.Pessoa.Saldo -= valorFormatado;
            return saida;
        }

        private static Entrada CriaTransacaoEntrada(string descricao, double valorFormatado, string dataFormatada, Pessoa pessoa)
        {
            Entrada entrada = new Entrada
            {
                Pessoa = pessoa,
                Descricao = descricao,
                Data = DateTime.Parse(dataFormatada),
                Valor = valorFormatado
            };
            entrada.Pessoa.Saldo += valorFormatado;
            return entrada;
        }

        private async Task AddAndUpdateEntrada(Entrada entrada)
        {
            await _entradas.Add(entrada);
            await _usuario.Update(entrada.Pessoa);
            string jsonString = JsonSerializer.Serialize(entrada.Pessoa);
            HttpContext.Session.SetString("User", jsonString);
        }

        private async Task AddAndUpdateSaida(Saida saida)
        {
            await _saidas.Add(saida);
            await _usuario.Update(saida.Pessoa);
            string jsonString = JsonSerializer.Serialize(saida.Pessoa);
            HttpContext.Session.SetString("User", jsonString);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, string nome, string email, string senha)
        {
            Pessoa pessoa = await _usuario.FindById(id);
            pessoa.Nome = nome;
            pessoa.Email = email;
            pessoa.Id = id;
            pessoa.Senha = senha;
            await _usuario.Update(pessoa);
            string jsonString = JsonSerializer.Serialize(pessoa);
            HttpContext.Session.SetString("User", jsonString);
            return View(pessoa);
        }

        public ActionResult MinhasMovimentacoes()
        {
            Pessoa usuarioLogado;
            List<Entrada> entradas;
            List<Saida> saidas;
            List<ModeloParaMov> movimentos;
            FindEntradasESaidasByUsuario(out usuarioLogado, out entradas, out saidas, out movimentos);
            ForEachParaUniaoDeEntradaESaida(entradas, saidas, movimentos);

            movimentos = movimentos.OrderBy(i => i.Data).ToList();
            var usuario = _usuario.FindUsuario(usuarioLogado.Id).ToList();

            ModeloParaViewMov EntradaPessoa2 = new ModeloParaViewMov(movimentos, usuario);
            return View(EntradaPessoa2);


        }

        private void FindEntradasESaidasByUsuario(out Pessoa usuarioLogado, out List<Entrada> entradas, out List<Saida> saidas, out List<ModeloParaMov> movimentos)
        {
            usuarioLogado = JsonSerializer.Deserialize<Pessoa>(HttpContext.Session.GetString("User"));
            entradas = _entradas.FindUsuario(usuarioLogado.Id).ToList();
            saidas = _saidas.FindUsuario(usuarioLogado.Id).ToList();
            movimentos = new List<ModeloParaMov>();
        }

        private void ForEachParaUniaoDeEntradaESaida(List<Entrada> entradas, List<Saida> saidas, List<ModeloParaMov> movimentos)
        {
            foreach (var entrada in entradas)
            {
                ModeloParaMov movimento = new ModeloParaMov();
                movimento.Id = entrada.Id;
                movimento.TipoMovimentacao = "Entrada";
                movimento.Descricao = entrada.Descricao;
                movimento.Data = entrada.Data;
                movimento.Pessoa = entrada.Pessoa;
                movimento.Valor = entrada.Valor;

                movimentos.Add(movimento);
            }

            foreach (var saida in saidas)
            {
                ModeloParaMov movimento = new ModeloParaMov();
                movimento.Id = saida.Id;
                movimento.TipoMovimentacao = "Saída";
                movimento.Descricao = saida.Descricao;
                movimento.Pessoa = saida.Pessoa;
                movimento.Data = saida.Data;
                movimento.Valor = saida.Valor;

                movimentos.Add(movimento);
            }
        }

    }
}
