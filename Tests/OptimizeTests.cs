using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ConsoleTables;
using GeneticOptimizationFramework.Optimization;
using GeneticOptimizationFramework.Tests.Helpers;
using GeneticOptimizationFramework.Tests.Models;
using Xunit;

namespace GeneticOptimizationFramework.Tests
{
    public class OptimizeTests
    {
        #region Test Methods
        [Theory]
        [JsonFileData("Data/WorkItemsTestData.json", "WorkItemsTestDataCollection", typeof(List<WorkItem>))]
        public void GetGeneticPermutationTest(List<WorkItem> data)
        {
            ValidateData(data);

            var randomBenchmarkResult = Optimize<WorkItem>.GetRandomPermutation(data, WorkItemsCost, 25);
            PrintResults("Random Benchmark Test Result", randomBenchmarkResult);

            var geneticResult = Optimize<WorkItem>.GetGeneticPermutation(data, WorkItemsCost);
            PrintResults("Genetic Test Result", geneticResult);

            Assert.True(geneticResult.ResultCost < randomBenchmarkResult.ResultCost, "Random benchmark permutations should not produce a scored better then a genetic permutation.");
        }

        [Theory]
        [JsonFileData("Data/WorkItemsTestData.json", "WorkItemsTestDataCollection", typeof(List<WorkItem>))]
        public void GetRandomPermutationTest(List<WorkItem> data)
        {
            ValidateData(data);

            var randomMulticycleData = Optimize<WorkItem>.GetRandomPermutation(data, WorkItemsCost, 100);            
            PrintResults("Random Multicycle Test Result", randomMulticycleData);

            var randomSingleCycleData = Optimize<WorkItem>.GetRandomPermutation(data, WorkItemsCost, 1);
            PrintResults("Random Single Cycle Test Result", randomMulticycleData);

            Assert.True(randomMulticycleData.ResultCost < randomSingleCycleData.ResultCost, "Random permutations should not produce a scored better then a genetic permutation.");
        }
        #endregion

        #region Test Support Methods
        private int WorkItemsCost(List<WorkItem> workItemsCandidate)
        {
            var score = 0;
            var resultCount = workItemsCandidate.Count;

            for (int i = 0; i < resultCount; i++)
            {
                var itemScore = workItemsCandidate[i].TimeEstHours;
                score += Convert.ToInt32(((Convert.ToDecimal(i) + 1M / Convert.ToDecimal(resultCount)) * 100) * itemScore);
            }

            return score;
        }

        private void PrintResults(string name, OptimizationResult<WorkItem> optimazationResult)
        {
            if (optimazationResult == null)
            {
                Console.WriteLine("A null optimzation result was provided.");
            }
            else
            {
                Console.WriteLine($"Name: {name ?? "No solution name was provided."}");
                Console.WriteLine($"Result Cost: {optimazationResult.ResultCost}");
                Console.WriteLine($"Permutation Count: {optimazationResult.PermutationCount}");
                Console.WriteLine($"Execution Time (Milliseconds): {optimazationResult.ExecutionTime.Milliseconds}");

                if (optimazationResult.Result == null || optimazationResult.Result.Count < 1)
                {
                    Console.WriteLine("No schedule was provided.");
                }
                else
                {
                    ConsoleTable.From<WorkItem>(optimazationResult.Result)
                        .Configure(o => o.NumberAlignment = Alignment.Right)
                        .Write(Format.Alternative);
                }

                Console.WriteLine();
            }
        }

        private void ValidateData(List<WorkItem> data)
        {
            if (data == null || data.Count < 1)
            {
                throw new ArgumentException(nameof(data));
            }

            data.ForEach(delegate (WorkItem workItem) {
                var modelValidationResults = new List<ValidationResult>();
                if (!TryModelValidate(workItem, out modelValidationResults))
                {
                    var msg = "At least one data row failed validation. ";
                    msg += modelValidationResults.SingleOrDefault().ErrorMessage;
                    throw new ValidationException(msg);
                };
            });
        }

        private bool TryModelValidate(object modelObject, out List<ValidationResult> validationResults)
        {
            var context = new ValidationContext(modelObject, serviceProvider: null, items: null);
            validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(modelObject, context, validationResults, validateAllProperties: true);
        }
        #endregion
    }
}
