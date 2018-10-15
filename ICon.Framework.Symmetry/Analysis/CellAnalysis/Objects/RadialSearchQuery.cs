using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using ICon.Mathematics.Constraints;

namespace ICon.Symmetry.Analysis
{
    /// <summary>
    /// Generic radial search query that defines a start position, radial constraint, acceptance predicate and sorting comparer for radial position sampling
    /// </summary>
    public class RadialSearchQuery<T1>
    {
        /// <summary>
        /// The task completion source for the search query
        /// </summary>
        public TaskCompletionSource<IList<CellEntry<T1>>> TaskCompletionSource { get; protected set; }

        /// <summary>
        /// The unit cell provider used for the search
        /// </summary>
        public IUnitCellProvider<T1> UnitCellProvider { get; set; }

        /// <summary>
        /// Defines the cell entry at the start position
        /// </summary>
        public CellEntry<T1> StartCellEntry { get; set; }

        /// <summary>
        /// The radial search constraint
        /// </summary>
        public NumericConstraint RadialConstraint { get; set; }

        /// <summary>
        /// The acceptance prediacte for found entry values
        /// </summary>
        public Predicate<T1> AcceptancePredicate { get; set; }

        /// <summary>
        /// The sorting comparer used for sorted samplings
        /// </summary>
        public IComparer<CellEntry<T1>> SortingComparer { get; set; }

        /// <summary>
        /// Get the value from the task completion source when the calculation is finished
        /// </summary>
        public IList<CellEntry<T1>> Result => TaskCompletionSource.Task.Result;

        /// <summary>
        /// Create empty search query with an always true acceptance predicate
        /// </summary>
        public RadialSearchQuery()
        {
            AcceptancePredicate = a => true;
        }

        /// <summary>
        /// Starts the query on the thread pool if not already done
        /// </summary>
        public void Start()
        {
            if (TaskCompletionSource != null)
            {
                return;
            }
            InvokeResultGeneration();
        }

        /// <summary>
        /// Invokes the result generation as a new task
        /// </summary>
        protected void InvokeResultGeneration()
        {
            TaskCompletionSource = new TaskCompletionSource<IList<CellEntry<T1>>>();
            Task.Run(() => TaskCompletionSource.SetResult(GetSamplerDelegate().Invoke()));
        }

        /// <summary>
        /// Get the correct sampling delegate depending on the set query properties
        /// </summary>
        /// <returns></returns>
        protected Func<IList<CellEntry<T1>>> GetSamplerDelegate()
        {
            var sampler = new RadialPositionSampler();
            if (SortingComparer == null)
            {
                return () => sampler.Search(UnitCellProvider, StartCellEntry.AbsoluteVector, RadialConstraint, AcceptancePredicate);
            }
            return () => sampler.Search(UnitCellProvider, StartCellEntry.AbsoluteVector, RadialConstraint, AcceptancePredicate, SortingComparer);
        }
    }
}
