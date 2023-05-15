using NHibernate;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using WebCarteiraMvc.Models;
using NHibernate.Mapping;

namespace WebCarteiraMvc.Repositorio
{
    public class RepositorioEntrada : IRepositorioMov<Entrada>
    {
        private ISession _session;

        public RepositorioEntrada(ISession session) => _session = session;

        public async Task Add(Entrada item)
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

        public IEnumerable<Entrada> FindByName(string nomeMovimentacao)
        {
            var query = _session.Query<Entrada>().Where(x => x.Pessoa.Nome.Contains(nomeMovimentacao ?? string.Empty));


            return query;
        }


        public IEnumerable<Entrada> FindUsuario(int idUsuario)
        {
            var query = _session.Query<Entrada>().Where(x => x.Pessoa.Id == idUsuario);
            return query;
        }

        public IEnumerable<Entrada> FindAll() =>
            _session.Query<Entrada>();

        public async Task<Entrada> FindById(int id) =>
            await _session.GetAsync<Entrada>(id);



        public async Task Remove(int id)
        {
            ITransaction transaction = null;
            try
            {
                transaction = _session.BeginTransaction();
                var item = await _session.GetAsync<Entrada>(id);
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

        public async Task Update(Entrada item)
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
