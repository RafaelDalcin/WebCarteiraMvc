using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebCarteiraMvc.Repositorio
{
    public interface IRepositorio<T>
    {
        Task Add (T item);
        Task Remove(int id);
        Task Update(T Item);
        Task<T> FindById (int id);
        IEnumerable<T> FindAll ();



    }
}
