using System;
namespace Optimazation
{
    public interface IOptimizationItem: ICloneable
    {
        int UniqueId { get; set; }
    }
}
