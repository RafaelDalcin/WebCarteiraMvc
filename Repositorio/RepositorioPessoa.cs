using Microsoft.AspNetCore.Http;
using NHibernate;
using sun.security.provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCarteiraMvc.Models;
using ISession = NHibernate.ISession;

namespace WebCarteiraMvc.Repositorio
{
    public class RepositorioPessoa : IRepositorio<Pessoa>
    {
        private ISession _session;

        public RepositorioPessoa(ISession session) => _session = session;

        public async Task Add(Pessoa item)
        {
            ITransaction transaction = null;
            try
            {
                transaction = _session.BeginTransaction();
                await _session.SaveAsync(item);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await transaction?.RollbackAsync();

            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public IEnumerable<Pessoa> FindAll() =>
            _session.Query<Pessoa>();

        public async Task<Pessoa> FindById(int id) => 
			await _session.GetAsync<Pessoa>(id);

        public IEnumerable<Pessoa> FindByName(string name)
        {
            var pessoas = from p in _session.Query<Pessoa>()
                          where p.Nome.Contains(name?? string.Empty)
                          select p;

            return pessoas;
        }


        public Pessoa FindByEmail(string email)
        {
            var pessoas = from p in _session.Query<Pessoa>()
                          where p.Email == email
                          select p;

            return pessoas.FirstOrDefault();
        }


        public Pessoa FindPessoaByLogin(string email, string senha)
        {
            var pessoas = from p in _session.Query<Pessoa>()
                          where p.Email == email && p.Senha == senha
                          select p;

            return pessoas.FirstOrDefault();
        }

        public IEnumerable<Pessoa> FindUsuario(int idUsuario)
        {
            var query = _session.Query<Pessoa>().Where(x => x.Id == idUsuario);
            return query;
        }


        public async Task Remove(int id)
        {
            ITransaction transaction = null;
            try
            {
                transaction = _session.BeginTransaction();
                var item = await _session.GetAsync<Pessoa>(id);
                await _session.DeleteAsync(item);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await transaction?.RollbackAsync();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task Update(Pessoa item)
        {
            ITransaction transaction = null;
            try
            {
                transaction = _session.BeginTransaction();
                _session.SaveOrUpdate(item);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await transaction?.RollbackAsync();
            }
            finally
            {
                transaction?.Dispose();
            }
        }
    }
}
