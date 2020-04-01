using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     A async query for searching all radial <see cref="LatticeTarget" /> entries around a position in a lattice
    /// </summary>
    public class RadialLatticeTargetQuery<T>
    {
        /// <summary>
        ///     Get or set the selector <see cref="Func{T,TResult}" /> delegate
        /// </summary>
        private Func<IList<LatticePoint<T>>, IComparer<LatticeTarget>, LatticeTarget[]> Selector { get; set; }

        /// <summary>
        ///     Get or set the query task
        /// </summary>
        public Task<LatticeTarget[]> QueryTask { get; private set; }

        /// <summary>
        ///     Get the result. This will await the query
        /// </summary>
        public LatticeTarget[] Result => QueryTask?.Result ?? Run().Result;

        /// <summary>
        ///     Get or set the underlying <see cref="RadialLatticePointQuery{T}" />
        /// </summary>
        public RadialLatticePointQuery<T> RadialLatticePointQuery { get; private set; }

        /// <summary>
        ///     Get or seth the <see cref="IComparer{T}" /> that is used for sorting the results (Null causes no sorting)
        /// </summary>
        public IComparer<LatticeTarget> SortingComparer { get; set; }

        protected RadialLatticeTargetQuery()
        {
        }

        /// <summary>
        ///     Starts the query in the background if not already started
        /// </summary>
        public void Start()
        {
            if (QueryTask != null) return;
            RadialLatticePointQuery.Start();
            QueryTask = RadialLatticePointQuery.QueryTask.ContinueWith(x => Selector(x.Result, SortingComparer));
        }

        /// <summary>
        ///     Runs the query on the tread pool if not already started
        /// </summary>
        /// <returns></returns>
        public Task<LatticeTarget[]> Run()
        {
            Start();
            return QueryTask;
        }

        /// <summary>
        ///     Runs the query on the current thread and returns the result
        /// </summary>
        /// <returns></returns>
        public LatticeTarget[] RunSynchronously()
        {
            if (QueryTask != null) return QueryTask.Result;
            RadialLatticePointQuery.RunSynchronously();
            QueryTask = new Task<LatticeTarget[]>(() => Selector(RadialLatticePointQuery.Result, SortingComparer));
            QueryTask.RunSynchronously();
            return QueryTask.Result;
        }

        /// <summary>
        ///     Creates a new <see cref="RadialLatticeTargetQuery{T}" /> for a start <see cref="Fractional3D" /> and
        ///     <see cref="IUnitCellProvider{T1}" /> that is only restricted by a max distance in angstrom
        /// </summary>
        /// <param name="unitCellProvider"></param>
        /// <param name="originVector"></param>
        /// <param name="maxDistance"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static RadialLatticeTargetQuery<T> CreateRanged(IUnitCellProvider<T> unitCellProvider, in Fractional3D originVector,
            double maxDistance, NumericComparer comparer = null)
        {
            var originPoint = new LatticePoint<T>(originVector, default);
            var radialSearchQuery = new RadialLatticePointQuery<T>
            {
                UnitCellProvider = unitCellProvider,
                RadialConstraint = new NumericConstraint(false, 0, maxDistance, true, comparer ?? NumericComparer.Default()),
                OriginLatticePoint = originPoint
            };
            var selector = CreateSelector(originPoint, unitCellProvider.VectorEncoder);
            return new RadialLatticeTargetQuery<T> {RadialLatticePointQuery = radialSearchQuery, Selector = selector};
        }

        /// <summary>
        ///     Creates the mapping <see cref="Func{T, TResult}" /> that projects the <see cref="RadialLatticePointQuery{T}" />
        ///     results
        ///     with optional sorting <see cref="IComparer{T}" />
        /// </summary>
        /// <param name="originPoint"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        private static Func<IList<LatticePoint<T>>, IComparer<LatticeTarget>, LatticeTarget[]> CreateSelector(LatticePoint<T> originPoint,
            IUnitCellVectorEncoder vectorEncoder)
        {
            LatticeTarget[] ToTargets(IList<LatticePoint<T>> targets, IComparer<LatticeTarget> sortingComparer)
            {
                var data = LatticeTarget.FromLatticePoints(originPoint, targets, vectorEncoder).ToArray();
                if (sortingComparer != null) Array.Sort(data, sortingComparer);
                return data;
            }

            return ToTargets;
        }
    }
}