using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _102techBot.Domain.Repositories
{
    internal interface IRepository<T> where T : class
    {
        public T GetById(int id);
        IEnumerable<T> GetAll();
        public T Add(T obj);
        public T Update(T obj);
        public T Delete(int id);
    }
}
