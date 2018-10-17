using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Provider;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    public class SymmetryAnalysisService : ISymmetryAnalysisService
    {
        /// <summary>
        ///     The comparer for the approx symmetry indicator comparison
        /// </summary>
        public IComparer<SymmetryIndicator> IndicatorComparer { get; set; }

        /// <summary>
        ///     Provider for the vector transformer to change coordinate systems
        /// </summary>
        public IObjectProvider<IVectorTransformer> TransformerProvider { get; set; }

        /// <summary>
        ///     Create new symmetry analysis service from indicator comparer and vector transformer provider
        /// </summary>
        /// <param name="indicatorComparer"></param>
        /// <param name="transformerProvider"></param>
        public SymmetryAnalysisService(IComparer<SymmetryIndicator> indicatorComparer,
            IObjectProvider<IVectorTransformer> transformerProvider)
        {
            IndicatorComparer = indicatorComparer ?? throw new ArgumentNullException(nameof(indicatorComparer));
            TransformerProvider = transformerProvider ?? throw new ArgumentNullException(nameof(transformerProvider));
        }


        /// <inheritdoc />
        public SymmetryIndicator GetSymmetryIndicator<T1>(IEnumerable<T1> massPoints) where T1 : struct, ICartesianMassPoint3D<T1>
        {
            return new GeometricSymmetryAnalyzer().GetSymmetryIndicator(massPoints, TransformerProvider.Get().FractionalSystem.Comparer);
        }

        /// <inheritdoc />
        public SymmetryIndicator GetSymmetryIndicatorAny<T1>(IEnumerable<T1> massPoints) where T1 : struct, IMassPoint3D<T1>
        {
            return GetSymmetryIndicator(GetCartesianSequence(massPoints));
        }

        /// <inheritdoc />
        public SymmetryCompareIndicator CompareSymmetryApprox<T1>(IEnumerable<T1> firstSet, IEnumerable<T1> secondSet)
            where T1 : struct, ICartesianMassPoint3D<T1>
        {
            return new SymmetryCompareIndicator(GetSymmetryIndicator(firstSet), GetSymmetryIndicator(secondSet), IndicatorComparer);
        }

        /// <inheritdoc />
        public SymmetryCompareIndicator CompareSymmetryApproxAny<T1, T2>(IEnumerable<T1> firstSet, IEnumerable<T2> secondSet)
            where T1 : struct, IMassPoint3D<T1> where T2 : struct, IMassPoint3D<T2>
        {
            return CompareSymmetryApprox(GetCartesianSequence(firstSet), GetCartesianSequence(secondSet));
        }

        /// <inheritdoc />
        public bool CompareSymmetryExact<T1>(IEnumerable<T1> firstSet, IEnumerable<T1> secondSet, IComparer<T1> comparer)
            where T1 : struct, ICartesian3D<T1>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool TryGetTransformationSequence<T1>(IEnumerable<T1> source, IEnumerable<T1> target, IComparer<T1> comparer,
            out IEnumerable<TransformMatrix2D> matrix) where T1 : struct, IVector3D<T1>
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Creates a sequence with cartesian coordinates from any kind of mass point sequence
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<CartesianMassPoint3D<double>> GetCartesianSequence<T1>(IEnumerable<T1> source)
            where T1 : struct, IMassPoint3D<T1>
        {
            var sourceCollection = source.ToCollection();
            return sourceCollection.Zip(TransformerProvider.Get().ToCartesian((IEnumerable<IVector3D>) sourceCollection),
                (org, vector) => new CartesianMassPoint3D<double>(org.GetMass(), vector));
        }
    }
}