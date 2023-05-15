using NHibernate;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebCarteiraMvc.Models;
using System.Linq;

namespace WebCarteiraMvc.Repositorio
{
    public class RepositorioSaida : IRepositorioMov<Saida>
    {
        private ISession _session;

        public RepositorioSaida(ISession session) => _session = session;

        public async Task Add(Saida item)
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


        public IEnumerable<Saida> FindByName(string nomeMovimentacao)
        {
            var query = _session.Query<Saida>().Where(x => x.Pessoa.Nome.Contains(nomeMovimentacao ?? string.Empty));


            return query;
        }

        public IEnumerable<Saida> FindUsuario(int idUsuario)
        {
            var query = _session.Query<Saida>().Where(x => x.Pessoa.Id == idUsuario);
            return query;
        }
        public IEnumerable<Saida> FindAll() =>
                _session.Query<Saida>();

        public async Task<Saida> FindById(int id) =>
            await _session.GetAsync<Saida>(id);



        public async Task Remove(int id)
        {
            ITransaction transaction = null;
            try
            {
                transaction = _session.BeginTransaction();
                var item = await _session.GetAsync<Saida>(id);
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

        public async Task Update(Saida item)
        {
            ITransaction transaction = null;
            try
            {
                transaction = _session.BeginTransaction();
                await _session.UpdateAsync(item);
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
