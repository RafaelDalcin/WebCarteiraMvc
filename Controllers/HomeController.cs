using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebCarteiraMvc.Models;
using WebCarteiraMvc.Repositorio;

namespace WebCarteiraMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<HomeController> _logger;
        private readonly RepositorioPessoa _pessoa;
        private readonly IHttpContextAccessor _contxt;
        private readonly INotyfService _toastNotification;
        public HomeController(ILogger<HomeController> logger,
            NHibernate.ISession session,
            IHttpContextAccessor httpContextAccessor,
            INotyfService toastNotification,
            IEmailSender emailSender)
        {
            _logger = logger;
            _pessoa = new RepositorioPessoa(session);
            _contxt = httpContextAccessor;
            _toastNotification = toastNotification;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logar(string email, string senha)
        {
            var dadosPessoa = _pessoa.FindPessoaByLogin(email, senha);

            if (dadosPessoa == null || email == null || senha == null)
            {
                _toastNotification.Error("Login inexistente!");
                return View("Index");
            }

            bool admin = dadosPessoa.Id == 1;
            string jsonString = JsonSerializer.Serialize(dadosPessoa);
            _contxt.HttpContext.Session.SetString("User", jsonString);
            _toastNotification.Success("Seja bem-vindo ao sistema de controle financeiro! " + dadosPessoa.Nome.ToUpper());
            return admin ? RedirectToAction("Index", "Pessoa") : RedirectToAction("Index", "Usuario");

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ResetPassword(string email)
        {
            await VerificaEmailEGeraSenha(email);

            return RedirectToAction("Index", "Home");

        }

        private async Task VerificaEmailEGeraSenha(string email)
        {
            if (email != null)
            {
                string chars = "abcdefghjkmnpqrstuvwxyz023456789";
                string pass = "";
                Random random = new Random();
                for (int f = 0; f < 6; f++)
                {
                    pass = pass + chars.Substring(random.Next(0, chars.Length - 1), 1);
                }

                await EnviaEmailESalvaSenha(email, pass);

            }
        }

        private async Task EnviaEmailESalvaSenha(string email, string pass)
        {
            var pessoa = _pessoa.FindByEmail(email);
            if (pessoa != null)
            {
                pessoa.Senha = pass;
                await _pessoa.Update(pessoa);
                var receiver = email;
                var subject = "RESET PASSWORD";
                string message = HmtlDoEmail(pessoa);
                await _emailSender.SendEmailAsync(receiver, subject, message);
            }
        }

        private static string HmtlDoEmail(Pessoa pessoa)
        {
            return $@"<div style=""text-align: center;"">
        <h1>
            ---------------- Password Reset ----------------
        </h1>
    </div>
    <hr>
    <div>
        <h2>
            <p>
    Olá {pessoa.Nome},
                Você recentemnte fez o pedido de renovação de senha em sua conta na nossa plataforma de Fluxo Financeiro.
            </p>
            <p>
                Se você não fez o pedido de renovação de senha, porfavor apenas ignore esse e-mail ou nos responda para sabermos.
                Essa senha é válida pelos próximos 30 minutos.
            </p>                
            <p>
                Atenciosamente, Fluxo Financeiro.
            </p>
        </h2>
    </div>
    <div>
        <h3>
            <p>Login: {pessoa.Email}</p>
            <p>Nova senha: {pessoa.Senha}</p>
        </h3>
    </div>";
        }
    }
}
