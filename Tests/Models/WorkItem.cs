using System;
using System.ComponentModel.DataAnnotations;
using GeneticOptimizationFramework.Optimization;

namespace GeneticOptimizationFramework.Tests.Models
{
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
}
