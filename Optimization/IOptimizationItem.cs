using System;

namespace GeneticOptimizationFramework.Optimization
{
    public interface IOptimizationItem: ICloneable
    {
        int UniqueId { get; set; }
    }
}
