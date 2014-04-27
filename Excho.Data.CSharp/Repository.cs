using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excho.Data.CSharp
{
    public interface IRepository
    {
        IEnumerable All { get; }
        void Commit(object data);
        void Delete(object data);
    }
    public interface IRepository<out T> : IRepository
    {
        IEnumerable<T> All { get; }
        void Commit(T data);
        void Delete(T data);
    }
}
