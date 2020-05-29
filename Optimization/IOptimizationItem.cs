using System;

namespace Optimization
{
    public interface IOptimizationItem: ICloneable
    {
        int UniqueId { get; set; }
    }
}
