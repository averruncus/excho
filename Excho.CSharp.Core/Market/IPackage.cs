using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excho.Market
{
    public interface IPackage
    {
        IInventory Source { get; }
        IEnumerable<object> Items { get; }
    }

    public interface IPackage<out T> : IPackage
    {
        IInventory Source { get; }
        IEnumerable<T> Items { get; }
    }
}
