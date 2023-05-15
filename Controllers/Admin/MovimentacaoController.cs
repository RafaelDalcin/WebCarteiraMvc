
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebCarteiraMvc.Models;
using WebCarteiraMvc.ObjetosDeValor;
using WebCarteiraMvc.Repositorio;
using System.Windows;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using IronPdf;

namespace WebCarteiraMvc.Controllers.Admin
{
    public class MovimentacaoController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly RepositorioEntrada entRep;
        private readonly RepositorioPessoa pesRep;
        private readonly RepositorioSaida saidRep;
        private readonly INotyfService _toastNotification;
        public MovimentacaoController(NHibernate.ISession session, INotyfService toastNotification, IEmailSender emailSender)
        {
            entRep = new RepositorioEntrada(session);
            pesRep = new RepositorioPessoa(session);
            saidRep = new RepositorioSaida(session);
            _toastNotification = toastNotification;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            List<Entrada> entradas = entRep.FindAll().ToList();
            List<Saida> saidas = saidRep.FindAll().ToList();

            List<ModeloParaMov> movimentos = new List<ModeloParaMov>();

            ForEachUniaoEntradaSaida(entradas, saidas, movimentos);
            movimentos = movimentos.OrderBy(i => i.Data).ToList();
            ModeloParaViewMov EntradaPessoa2 = new ModeloParaViewMov(movimentos, pesRep.FindAll().ToList());
            return View(EntradaPessoa2);
        }

        private void ForEachUniaoEntradaSaida(List<Entrada> entradas, List<Saida> saidas, List<ModeloParaMov> movimentos)
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Create(double valor, string descricao, int tipoMovimentacao, int idPessoa)
        {
            if (ModelState.IsValid)
            {
                double valorFormatado = Convert.ToDouble(valor.ToString().Replace('.', ','));
                Saida saida = new Saida();
                Entrada entrada = new Entrada();
                string dataFormatada = DateTime.Now.ToString("dd/MM/yyyy");

                var pessoa = await pesRep.FindById(idPessoa);

                if (tipoMovimentacao == 1)
                {
                    EntradaCreateBase(descricao, valorFormatado, entrada, dataFormatada, pessoa);
                    await entRep.Add(entrada);
                    await pesRep.Update(entrada.Pessoa);
                }
                else
                {
                    SaidaCreateBase(descricao, valorFormatado, saida, dataFormatada, pessoa);
                    if (saida.Pessoa.Saldo < 0)
                    {
                        _toastNotification.Error("Saldo insuficiente para realizar esta transação!");
                        return RedirectToAction("Index");
                    }
                    else if (saida.Pessoa.Saldo < saida.Pessoa.Minimo)
                    {
                        _toastNotification.Warning("Transação feita com sucesso, porém saldo abaixo do mínimo!");
                        await saidRep.Add(saida);
                        await pesRep.Update(saida.Pessoa);
                        return RedirectToAction("Index");
                    }
                    await saidRep.Add(saida);
                    await pesRep.Update(saida.Pessoa);

                }

                _toastNotification.Success("Transação feita com sucesso!");
            }


            return RedirectToAction("Index");



        }

        private static void EntradaCreateBase(string descricao, double valorFormatado, Entrada entrada, string dataFormatada, Pessoa pessoa)
        {
            entrada.Pessoa = pessoa;
            entrada.Valor = valorFormatado;
            entrada.Descricao = descricao;
            entrada.Data = DateTime.Parse(dataFormatada);
            entrada.Pessoa.Saldo += valorFormatado;
        }

        private static void SaidaCreateBase(string descricao, double valorFormatado, Saida saida, string dataFormatada, Pessoa pessoa)
        {
            saida.Pessoa = pessoa;
            saida.Valor = valorFormatado;
            saida.Descricao = descricao;
            saida.Data = DateTime.Parse(dataFormatada);
            saida.Pessoa.Saldo -= valorFormatado;
        }

        [HttpGet]
        public ActionResult PesquisaFiltro(string nomeMovimentacao, bool checkBoxEntrada, bool checkBoxSaida, DateTime from, DateTime to)
        {
            List<ModeloParaMov> movimentos = new List<ModeloParaMov>();

            if (checkBoxEntrada == true && checkBoxSaida == false)
            {
                EntradasCheckBox(nomeMovimentacao, movimentos);

                movimentos = movimentos.OrderBy(i => i.Data).ToList();
            }
            else if (checkBoxSaida == true && checkBoxEntrada == false)
            {

                var saidas = saidRep.FindByName(nomeMovimentacao).ToList();
                SaidasCheckBox(movimentos, saidas);

                movimentos = movimentos.OrderBy(i => i.Data).ToList();
            }
            else
            {
                var entradas = entRep.FindByName(nomeMovimentacao).ToList();
                var saidas = saidRep.FindByName(nomeMovimentacao).ToList();
                EntradasCheckBox(nomeMovimentacao, movimentos);
                SaidasCheckBox(movimentos, saidas);

                movimentos = movimentos.OrderBy(i => i.Data).ToList();
            }
            _toastNotification.Information("Busca realizada com sucesso");
            movimentos = movimentos.Where(i => i.Data >= from && i.Data <= to).ToList();
            ModeloParaViewMov EntradaPessoa = new ModeloParaViewMov(movimentos, pesRep.FindAll().ToList(), nomeMovimentacao, checkBoxEntrada, checkBoxSaida);
            return View(viewName: "Index", EntradaPessoa);

        }

        private static void SaidasCheckBox(List<ModeloParaMov> movimentos, List<Saida> saidas)
        {
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

        private void EntradasCheckBox(string nomeMovimentacao, List<ModeloParaMov> movimentos)
        {
            var entradas = entRep.FindByName(nomeMovimentacao).ToList();
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
        }

        public ActionResult GerarPdf()
        {
            PdfRender();
            _toastNotification.Success("PDF Gerado com sucesso!");
            return RedirectToAction("Index");
        }

        private static void PdfRender()
        {
            var renderer = new IronPdf.ChromePdfRenderer
            {
                RenderingOptions =
                {
                    MarginTop = 10, //millimeters
                    MarginBottom = 20,
                    MarginLeft = 20,
                    MarginRight = 20,
                    CssMediaType = IronPdf.Rendering.PdfCssMediaType.Print,
                    TextHeader = new TextHeaderFooter
                    {
                        CenterText = "{pdf-title}",
                        DrawDividerLine = true,
                        FontSize = 20
                    },
                    TextFooter = new TextHeaderFooter
                    {
                        LeftText = "{date} {time}",
                        RightText = "Page {page} of {total-pages}",
                        DrawDividerLine = true,
                        FontSize = 15
                    }
                }
            };
            var pdf = renderer.RenderUrlAsPdf("https://localhost:5001/Movimentacao");
            pdf.SaveAs("relatorio.pdf");
        }
    }
}
