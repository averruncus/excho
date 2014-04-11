using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excho.Market
{
    public interface IInventory
    {
        IPackage Unload(IEnumerable<object> keys);
        void Load(IPackage package);
    }

    public interface IInventoryLoader<out T>
    {
        IPackage<T> Unload(IEnumerable<object> keys);
    }

    public interface IInventoryUnloader<in T>
    {
        void Load(IPackage<T> package);
    }

    public interface IInventory<T> : IInventory, IInventoryLoader<T>, IInventoryUnloader<T>
    {
    }
}
