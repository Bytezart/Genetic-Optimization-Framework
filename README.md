# Genetic Optimization Framework for .Net Core

This framework implements a set of optimization techniques known as “Stochastic Optimization” which can be leveraged to solve simple to complex optimization problems. The optimization techniques implemented within this library are typically used to generate an optimal solution for problems that have many different possible outcomes. These techniques have a wide verity of applications such as finding the best arbitrage opportunities in Forex Markets to deciding how best to pack pallets with boxes of differentiating sizes and weights.

## List Optimization in .Net Core

The process of optimizing generic list data involves finding the best solution to your problem by trying many different solutions using breeding and mutation algorithms. These optimization processes are typically used in cases where there are too many possible solutions to calculate in a reasonable time period. These capabilities have been designed to conveniently optimize your custom data models of varying size and complexity.

The example in this section implements a very simple and relatable data model resembling someone’s task list to demonstrate functionality and integration. Each task is represented by self-evident fields for identification and classification. These fields in conjunction with a Cost Function, explained later, will enable the optimization engine to optimize the list order based on your particular requirements.

## List \<T> Setup and Data Representation

Optimizing a generic List<T> of objects is a simple process that involves the implementation the IOptimazationItem interface for the list object type of T. The interface has only two requirements which allow the optimization framework to sort, copy and track each item as it mutates and breeds the data internally. The first requirement involves creating a UniqueId property and ensuring each object has a unique identifier. Finally, you’ll need to make sure your object implements ICloneable.

    public interface IOptimazationItem: ICloneable
    {
    	int UniqueId { get; set; }
    }
   
    public class WorkItem: IOptimizationItem
    {
        [Required(ErrorMessage = "Each task must be identified uniquely.")]
        public int UniqueId { get; set; }

        [Required(ErrorMessage = "Each task must contain a task name.")]
        public string TaskName { get; set; }

        [Range(1, 8, ErrorMessage = "A task's time estimate must be between one and eight hours.")]
        public int TimeEstHours { get; set; }

        [Range(1, 10, ErrorMessage = "A task's value must be between one and 10 points.")]
        public int Value { get; set; }

        [Range(0, 10, ErrorMessage = "A task's effort must be between one and 10 points.")]
        public int Effort { get; set; }

        public object Clone()
        {
            return new WorkItem()
            {
                UniqueId = UniqueId,
                TaskName = TaskName,
                TimeEstHours = TimeEstHours,
                Value = Value,
                Effort = Effort
            };
        }
    }
    
 ## Cost Function

The cost function is the principal mechanism used by the framework to effectively solve an optimization problem. The framework could potentially call this function millions of times to gage the effectiveness of any particular breeding, mutation or randomization routine. Considering this and other aspects it’s important to create a cost function that is both performant and accurate to your use case.

The role of the cost function is to determine how good or bad a particular arrangement of items within the List\<T> is. There is no predefined range of values used to represent how worse or better a particular arrangement is. To convey an optimization scale, the cost function should return lower values for optimizations that are better than others.

The simple example below will iterate through a candidate solution and produce a score that is a multiple of the item position in the list and the estimated time the task will take.  The effect of this cost function calculation causes solutions to skew towards a time descending candidate result.

> Note: These lists could be sorted any number of ways using
> built-in functions. However, these examples and their solutions have
> been chosen for their illustrative purposes. In practice, cost
> functions are many more times complex and represent the core logic of
> your particular use case.

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

## List\<T> Optimization Execution and Results Evaluation

Optimization execution can begin after the prerequisite data structures and source list have been created. The current version of the framework supports two types of optimization routines. There are more in development and will be made available in upcoming releases.

> Note: The solution test project contains functional tests that demonstrate end-to-end
> functionality in a very easy to understand format.

 ##### GetRandomPermutation(List<T> candidateData, Func<List<T>, int> costFunction, int iterationCount)

The GetRandomPermutation function will randomly mutate the provide list data the specified number of times and return the best randomly generated optimization. This method is good for benching marking and testing. In practice, randomly generated optimizations should never produce a better result than other methods of optimization.

	var randomBenchmarkResult = Optimize<WorkItem>.GetRandomPermutation(data, WorkItemsCost, 25);

##### GetGeneticPermutation(List<T> candidateData, Func<List<T>, int> costFunction)

The GetGeneticPermutation function leverages breeding and mutation processes to generate an optimization result.

	var geneticResult = Optimize<WorkItem>.GetGeneticPermutation(data, WorkItemsCost);

##### OptimizationResult\<T>

Each optimization function returns an OptimizationResult\<T> which provides you access to a optimized solution and statistical information.
	

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
