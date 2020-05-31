using System;
using System.Collections.Generic;

namespace GeneticOptimizationFramework.Optimization
{
    public class OptimizationResult<T>
    {
        /// <summary>
        /// The optimized list result for the operation.
        /// </summary>
        public ICollection<T> Result { get; set; }
        /// <summary>
        /// The total number of possible permutations.
        /// </summary>
        public double PermutationCount { get; set; }
        /// <summary>
        /// The minimum cost found for the operation.
        /// </summary>
        public int ResultCost { get; set; }
        /// <summary>
        /// The operation execution time.
        /// </summary>
        public TimeSpan ExecutionTime { get; set; }
    }
}
