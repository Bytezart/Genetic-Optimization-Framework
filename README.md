# Genetic Optimization Framework for .Net Core

This library implements a set of optimization techniques known as “Stochastic Optimization” which can be leveraged to solve simple to complex optimization problems. The optimization techniques implemented within this library are typically used to generate an optimal solution for problems that have many different possible outcomes. These techniques have a wide verity of applications such as finding the best arbitrage opportunities in Forex Markets to deciding how best to pack pallets with boxes of differentiating sizes and weights.

## List Optimization in .Net Core

The process of optimizing generic list data involves finding the best solution to your problem by trying many different solutions using breeding and mutation algorithms. These optimization processes are typically used in cases where there are too many possible solutions to calculate in a reasonable time period. These capabilities have been designed to conveniently optimize your custom data models of varying size and complexity.

The example in this section implements a very simple and relatable data model resembling someone’s task list to demonstrate functionality and integration. Each task is represented by self-evident fields for identification and classification. These fields in conjunction with a Cost Function, explained later, will enable the optimization engine to optimize the list order based on your particular requirements.

## List <T> Setup and Data Representation

Optimizing a generic List<T> of objects is a simple process that involves the implementation the IOptimazationItem interface for the list object type of T. The interface has only two requirements which allow the optimization framework to sort, copy and track each item as it mutates and breeds the data internally. The first requirement involves creating a UniqueId property and ensuring each object has a unique identifier. Finally, you’ll need to make sure your object implements ICloneable.

    public interface IOptimazationItem: ICloneable
    {
    	int UniqueId { get; set; }
    }

