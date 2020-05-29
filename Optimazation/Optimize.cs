using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Optimazation
{
    public static class Optimize<T> where T: IOptimizationItem
    {
        /// <summary>
        /// Returns the best random optimization for the supplied candidate.
        /// </summary>
        /// <param name="candidateData">Optimazation Candidate</param>
        /// <param name="costFunction">Candidate Cost Function</param>
        /// <param name="iterationCount">The number of random iterations to run.</param>
        /// <returns>The best randomly optimized list.</returns>
        public static OptimizationResult<T> GetRandomPermutation(List<T> candidateData, Func<List<T>, int> costFunction, int iterationCount)
        {
            if (candidateData == null || candidateData.Count < 1)
            {
                throw new ArgumentException(nameof(candidateData) + " cannot be null or empty.");
            }

            if (costFunction == null)
            {
                throw new ArgumentException(nameof(costFunction));
            }

            if (iterationCount < 1)
            {
                throw new ArgumentException(nameof(candidateData) + " must be greater then zero.");
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var rnd = new Random();
            var topPass = new Tuple<int, List<T>>(int.MaxValue, null);
            var lastIndex = candidateData.Count - 1;
            
            for (int i = 0; i < iterationCount; i++)
            {
                var currentList = new List<T>(candidateData.Select(x => x.Clone()).Cast<T>().ToList());
                var currentScore = int.MaxValue;

                for (int iRnd = 0; iRnd <= lastIndex; iRnd++)
                {
                    SwapListElements(currentList, rnd.Next(0, lastIndex + 1), rnd.Next(0, lastIndex + 1));
                }

                currentScore = costFunction(currentList);

                if (currentScore < topPass.Item1)
                {
                    topPass = new Tuple<int, List<T>>(currentScore, currentList);
                }
            }

            stopWatch.Stop();

            return new OptimizationResult<T>()
            {
                Result = topPass.Item2,
                ResultCost = topPass.Item1,
                ExecutionTime = stopWatch.Elapsed,
                PermutationCount = Permutations(candidateData.Count, candidateData.Count)
            };
        }

        /// <summary>
        /// Returns the best solution for the supplied candidate data using breeding and mutation algorithms. 
        /// </summary>
        /// <param name="candidateData">Optimazation Candidate</param>
        /// <param name="costFunction">Candidate Cost Function</param>
        /// <returns></returns>
        public static OptimizationResult<T> GetGeneticPermutation(List<T> candidateData, Func<List<T>, int> costFunction)
        {
            if (candidateData == null || candidateData.Count < 1)
            {
                throw new ArgumentException(nameof(candidateData) + " cannot be null or empty.");
            }

            if (costFunction == null)
            {
                throw new ArgumentException(nameof(costFunction));
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var permutationsCount = Permutations(candidateData.Count, candidateData.Count);
            var rnd = new Random();

            var workingPop = new List<Tuple<int, List<T>>>();
            var iPopCount = (permutationsCount > 120) ? 120 : permutationsCount;
            for (int iPop = 0; iPop < iPopCount; iPop++)
            {
                var popListItem = candidateData.Select(x => (T)x.Clone()).ToList();
                var lastIndex = popListItem.Count - 1;

                for (int iRnd = 0; iRnd <= lastIndex; iRnd++)
                {
                    SwapListElements(popListItem, rnd.Next(0, lastIndex + 1), rnd.Next(0, lastIndex + 1));
                }           

                workingPop.Add(new Tuple<int, List<T>>(int.MaxValue, popListItem));
            }                        

            for (int iGen = 0; iGen < 500; iGen++)
            {
                for (int i = 0; i < iPopCount; i++)
                {
                    if (i + 1 < iPopCount)
                    {
                        if (.2D < rnd.NextDouble())
                        {
                            BreedList(workingPop[i].Item2, workingPop[i + 1].Item2);
                        }
                        else
                        {
                            MutateList(workingPop[i + 1].Item2);
                        }
                    }

                    i++;
                }

                workingPop = ScoreSortList(workingPop, costFunction);
            }
                        
            var topResult = ScoreSortList(workingPop, costFunction).FirstOrDefault();

            stopWatch.Stop();

            return new OptimizationResult<T>()
            {
                Result = topResult.Item2,
                ResultCost = topResult.Item1,
                ExecutionTime = stopWatch.Elapsed,
                PermutationCount = permutationsCount
            };
        }

        /// <summary>
        /// Breeds the first inequality of listA into listB.
        /// </summary>
        /// <param name="listA">Parent List</param>
        /// <param name="listB">Child List</param>
        private static void BreedList(List<T> listA, List<T> listB)
        {
            if (listA != null && listA.Count > 1 && listB != null && listB.Count > 1)
            {
                foreach (var pair in listA.Zip(listB, Tuple.Create))
                {
                    if (pair.Item1.UniqueId != pair.Item2.UniqueId)
                    {
                        SwapListElements(listB, listB.FindIndex(x => x.UniqueId == pair.Item2.UniqueId),
                                            listB.FindIndex(x => x.UniqueId == pair.Item1.UniqueId));

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Mutates the positions of two list elements.
        /// </summary>
        /// <param name="sourceList">The source list to mutate.</param>
        private static void MutateList(List<T> sourceList)
        {
            if (sourceList == null)
            {
                throw new ArgumentException(nameof(sourceList));
            }

            if (sourceList.Count > 1)
            {
                var rnd = new Random();
                var sourceIndex = rnd.Next(0, sourceList.Count);
                var destinationIndex = rnd.Next(0, sourceList.Count);

                if (sourceIndex != destinationIndex)
                {
                    T sourceElement = sourceList[sourceIndex];
                    sourceList[sourceIndex] = sourceList[destinationIndex];
                    sourceList[destinationIndex] = sourceElement;
                }
            }
        }

        /// <summary>
        /// Sorts the supplied list according to the cost function score.
        /// </summary>
        /// <param name="sourceList">Source List</param>
        /// <param name="costFunction">Cost Function</param>
        private static List<Tuple<int, List<T>>> ScoreSortList(List<Tuple<int, List<T>>> sourceList, Func<List<T>, int> costFunction)
        {
            var scoredList = new List<Tuple<int, List<T>>>();
            foreach (var workingItem in sourceList)
            {
                scoredList.Add(new Tuple<int, List<T>>(costFunction(workingItem.Item2), workingItem.Item2));
            }

            scoredList.Sort(delegate (Tuple<int, List<T>> sortItemA, Tuple<int, List<T>> sortItemB) {
                if (sortItemA == null && sortItemB == null) return 0;
                else if (sortItemA == null) return -1;
                else if (sortItemB == null) return 1;
                else return sortItemA.Item1.CompareTo(sortItemB.Item1);
            });

            return scoredList;
        }

        /// <summary>
        /// Exchanges List<typeparamref name="T"/> element value postions of the source list.
        /// </summary>
        /// <param name="sourceList">Source List<typeparamref name="T"/></param>
        /// <param name="sourceIndex">Source Index</param>
        /// <param name="destinationIndex">Destination Index</param>
        private static void SwapListElements(List<T> sourceList, int sourceIndex, int destinationIndex)
        {
            if (sourceList == null)
            {
                throw new ArgumentException(nameof(sourceList));
            }

            if (sourceIndex != destinationIndex)
            {
                T sourceElement = sourceList[sourceIndex];
                sourceList[sourceIndex] = sourceList[destinationIndex];
                sourceList[destinationIndex] = sourceElement;
            }
        }

        /// <summary>
        /// Calculates the permutations for the supplied object and sample counts using
        /// the formula p(n,r) = n! / (n - r)!.
        /// </summary>
        /// <returns>Permutations</returns>
        private static double Permutations(int objectCount, int sampleCount)
        {
            if (!(objectCount >= sampleCount) || !(sampleCount >= 0))
            {
                throw new ArgumentException("Ensure objectCount ≥ sampleCount ≥ 0.");
            }

            return Factorial(objectCount) / Factorial(objectCount - sampleCount);
        }

        /// <summary>
        /// Calculates the factorial of the provided integer.
        /// </summary>
        /// <param name="number">Factorial Of</param>
        /// <returns>The calculated factorial.</returns>
        private static double Factorial(int number)
        {
            int i, r = 1;

            for (i = 1; i <= number; i++)
            {
                r *= i;
            }

            return r;
        }
    }
}
