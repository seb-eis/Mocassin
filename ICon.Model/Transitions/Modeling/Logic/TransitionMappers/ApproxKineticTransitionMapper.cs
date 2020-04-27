using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Symmetry.Analysis;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Approximated mapper for kinetic transitions that searches and creates all possible 4D encoded paths for reference
    ///     transitions (Uses radial chain search)
    /// </summary>
    public class ApproxKineticTransitionMapper
    {
        /// <summary>
        ///     The symmetry service used for geometry comparisons
        /// </summary>
        public ISymmetryAnalysisService SymmetryService { get; protected set; }

        /// <summary>
        ///     The unit cell provider used for the mapping search
        /// </summary>
        public IUnitCellProvider<int> UnitCellProvider { get; protected set; }

        /// <summary>
        ///     Create new transition mapper that uses the specified symmetry service and unit cell provider
        /// </summary>
        /// <param name="symmetryService"></param>
        /// <param name="unitCellProvider"></param>
        public ApproxKineticTransitionMapper(ISymmetryAnalysisService symmetryService, IUnitCellProvider<int> unitCellProvider)
        {
            SymmetryService = symmetryService ?? throw new ArgumentNullException(nameof(symmetryService));
            UnitCellProvider = unitCellProvider ?? throw new ArgumentNullException(nameof(unitCellProvider));
        }

        /// <summary>
        ///     Performs a mapping operation for a kinetic transition using all provided start points as possible points of origin.
        ///     Symmetry comparison is approximated
        ///     and intermediate positions are not checked
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="startPoints"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IEnumerable<KineticMapping> QuickMapping(IKineticTransition transition, IEnumerable<Fractional3D> startPoints,
            NumericComparer comparer)
        {
            var mainGeometry = GetGeometricPath(transition.GetGeometrySequence()).ToCollection();
            var chainSearcher = new PositionChainSampler<int>(UnitCellProvider, comparer);
            var candidates = chainSearcher.MultiPointSearch(startPoints, mainGeometry, EqualityComparer<int>.Default);

            foreach (var geometry in QuickFilterPaths(mainGeometry, candidates, comparer))
                yield return new KineticMapping(transition, EncodeGeometry(geometry).ToArray());
        }

        /// <summary>
        ///     Filters a sequence of kinetic mappings by returning only mappings that have intermediate positions allowed by the
        ///     passed intermediate map
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="positionMap"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public IEnumerable<KineticMapping> FilterByIntermediatePositions(IEnumerable<KineticMapping> mappings,
            IList<SetList<Fractional3D>> positionMap, double tolerance)
        {
            return mappings.Where(mapping => IntermediatePositionsValid(mapping, positionMap, tolerance));
        }

        /// <summary>
        ///     Checks if the intermediate position of each step of a mapping can be found in the passed position map
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="positionMap"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool IntermediatePositionsValid(KineticMapping mapping, IList<SetList<Fractional3D>> positionMap, double tolerance)
        {
            if (!UnitCellProvider.VectorEncoder.TryDecode(mapping.EncodedPath, out var regularPositions))
                throw new ArgumentException("Passed mapping contains invalid 4D position information", nameof(mapping));

            for (var i = 0; i < regularPositions.Count - 1; i++)
            {
                var interPosition = Fractional3D.CalculateMiddle(regularPositions[i + 1], regularPositions[i]).TrimToUnitCell(tolerance);
                if (!positionMap[i].Contains(interPosition))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Get a symmetry filtered sequence by comparing all sequences to the reference through the internal symmetry analysis
        ///     service (Approximated filtering)
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="geometries"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IEnumerable<IList<LatticePoint<int>>> QuickFilterPaths(IEnumerable<LatticePoint<int>> reference,
            IEnumerable<IList<LatticePoint<int>>> geometries, NumericComparer comparer)
        {
            var refIdentifier = SymmetryService.GetSymmetryIndicator(GetMassPointPath(reference));
            foreach (var geometry in GetUniquePaths(geometries, comparer))
            {
                var identifier = SymmetryService.GetSymmetryIndicator(GetMassPointPath(geometry));
                if (SymmetryService.IndicatorComparer.Compare(refIdentifier, identifier) == 0)
                    yield return geometry;
            }
        }

        /// <summary>
        ///     Creates a filtered version of a sequence of geometries by removing all duplicate sequences
        /// </summary>
        /// <param name="geometries"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected IEnumerable<IList<LatticePoint<int>>> GetUniquePaths(IEnumerable<IList<LatticePoint<int>>> geometries, NumericComparer comparer)
        {
            var sequenceComparer = MakeCellEntrySequenceComparer(new VectorComparer3D<Fractional3D>(comparer), Comparer<int>.Default);
            foreach (var item in ContainerFactory.CreateSetList(sequenceComparer, geometries))
                yield return item.ToList();
        }

        /// <summary>
        ///     Creates the reference geometry sequence from the 4D path information of a kinetic transition
        /// </summary>
        /// <param name="pathGeometry"></param>
        /// <returns></returns>
        protected IEnumerable<LatticePoint<int>> GetGeometricPath(IEnumerable<Vector4I> pathGeometry)
        {
            foreach (var encoded in pathGeometry)
            {
                if (!UnitCellProvider.VectorEncoder.TryDecode(encoded, out Fractional3D decoded))
                {
                    throw new ArgumentException("Passed 4D geometry sequence cannot be decoded with the internal vector encoded",
                        nameof(pathGeometry));
                }

                yield return new LatticePoint<int>(decoded, UnitCellProvider.GetCellEntry(encoded).Content);
            }
        }

        /// <summary>
        ///     Creates the reference geometry sequence from the 3DD path information of a kinetic transition
        /// </summary>
        /// <param name="pathGeometry"></param>
        /// <returns></returns>
        protected IEnumerable<LatticePoint<int>> GetGeometricPath(IEnumerable<Fractional3D> pathGeometry)
        {
            foreach (var decoded in pathGeometry)
            {
                if (!UnitCellProvider.VectorEncoder.TryEncode(decoded, out var encoded))
                {
                    throw new ArgumentException("Passed 3DD geometry sequence cannot be encoded with the internal vector encoded",
                        nameof(pathGeometry));
                }

                yield return new LatticePoint<int>(decoded, UnitCellProvider.GetCellEntry(encoded).Content);
            }
        }

        /// <summary>
        ///     Takes a sequence of cell entries and reinterprets them as a sequence of cartesian mass points
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected IEnumerable<CartesianMassPoint3D> GetMassPointPath<T1>(IEnumerable<LatticePoint<T1>> sequence)
            where T1 : struct, IConvertible
        {
            return sequence.Select(value =>
                new CartesianMassPoint3D(value.Content.ToDouble(CultureInfo.InvariantCulture),
                    UnitCellProvider.VectorEncoder.Transformer.ToCartesian(value.Fractional)));
        }

        /// <summary>
        ///     Takes a sequence of cell entries and decodes them as a sequence of 4D crystal vectors
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected IEnumerable<Vector4I> EncodeGeometry<T1>(IEnumerable<LatticePoint<T1>> sequence)
        {
            foreach (var item in sequence)
            {
                if (!UnitCellProvider.VectorEncoder.TryEncode(item.Fractional, out var encoded))
                {
                    throw new ArgumentException(
                        "The provided sequence contains fractional vectors that cannot be converted to 4D equivalents", nameof(sequence));
                }

                yield return encoded;
            }
        }

        /// <summary>
        ///     Creates a sequence comparer for sequences of cell entries that compares lexicographic by vector then entry value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vectorComparer"></param>
        /// <param name="entryComparer"></param>
        /// <returns></returns>
        protected IComparer<IEnumerable<LatticePoint<T1>>> MakeCellEntrySequenceComparer<T1>(IComparer<Fractional3D> vectorComparer,
            IComparer<T1> entryComparer)
        {
            var comparer = LatticePoint<T1>.MakeComparer(vectorComparer, entryComparer);
            return Comparer<IEnumerable<LatticePoint<T1>>>.Create((lhs, rhs) => lhs.LexicographicCompare(rhs, comparer));
        }
    }
}