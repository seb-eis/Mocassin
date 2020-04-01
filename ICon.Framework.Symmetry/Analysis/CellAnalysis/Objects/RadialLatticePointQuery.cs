using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mocassin.Mathematics.Constraints;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Generic radial search query that defines a start position, radial constraint, acceptance predicate and sorting
    ///     comparer for radial position sampling
    /// </summary>
    public class RadialLatticePointQuery<T>
    {
        /// <summary>
        ///     The active query <see cref="Task"/>
        /// </summary>
        public Task<IList<LatticePoint<T>>> QueryTask { get; protected set; }

        /// <summary>
        ///     The unit cell provider used for the search
        /// </summary>
        public IUnitCellProvider<T> UnitCellProvider { get; set; }

        /// <summary>
        ///     Defines the cell entry at the start position
        /// </summary>
        public LatticePoint<T> OriginLatticePoint { get; set; }

        /// <summary>
        ///     The radial search constraint
        /// </summary>
        public NumericConstraint RadialConstraint { get; set; }

        /// <summary>
        ///     The acceptance predicate for found entry values
        /// </summary>
        public Func<T, bool> AcceptancePredicate { get; set; }

        /// <summary>
        ///     The sorting comparer used for sorted samplings
        /// </summary>
        public IComparer<LatticePoint<T>> SortingComparer { get; set; }

        /// <summary>
        ///     Get the value from the task completion source when the calculation is finished
        /// </summary>
        public IList<LatticePoint<T>> Result => QueryTask?.Result;

        /// <summary>
        ///     Create empty search query with an always true acceptance predicate
        /// </summary>
        public RadialLatticePointQuery()
        {
            AcceptancePredicate = a => true;
        }

        /// <summary>
        ///     Starts the query on the thread pool if not already done
        /// </summary>
        public void Start()
        {
            if (QueryTask != null) return;
            QueryTask = PrepareTask();
            QueryTask.Start();
        }

        /// <summary>
        ///     Runs the search query
        /// </summary>
        /// <returns></returns>
        public Task<IList<LatticePoint<T>>> Run()
        {
            Start();
            return QueryTask;
        }

        /// <summary>
        ///     Runs the query synchronously
        /// </summary>
        /// <returns></returns>
        public IList<LatticePoint<T>> RunSynchronously()
        {
            if (QueryTask != null) return QueryTask.Result;
            QueryTask = PrepareTask();
            QueryTask.RunSynchronously();
            return QueryTask.Result;
        }

        /// <summary>
        ///     Prepares the work <see cref="Task{TResult}"/>
        /// </summary>
        /// <returns></returns>
        private Task<IList<LatticePoint<T>>> PrepareTask()
        {
            return new Task<IList<LatticePoint<T>>>(GetSamplerDelegate());
        }

        /// <summary>
        ///     Get the correct sampling delegate depending on the set query properties
        /// </summary>
        /// <returns></returns>
        protected Func<IList<LatticePoint<T>>> GetSamplerDelegate()
        {
            var sampler = new RadialPositionSampler();
            if (SortingComparer == null)
                return () => sampler.Search(UnitCellProvider, OriginLatticePoint.Fractional, RadialConstraint, AcceptancePredicate);

            return () => sampler.Search(UnitCellProvider, OriginLatticePoint.Fractional, RadialConstraint, AcceptancePredicate,
                SortingComparer);
        }
    }
}