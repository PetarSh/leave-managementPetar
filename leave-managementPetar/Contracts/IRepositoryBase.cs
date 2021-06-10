using leave_managementPetar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managementPetar.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
       Task<ICollection<T>> FindAll();

        Task<bool> isExists(int id);
        Task<T> FindById(int id);
        Task<bool> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Save();
    }
   

   

   
}
