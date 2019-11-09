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
        public SymmetryIndicator GetSymmetryIndicator(IEnumerable<CartesianMassPoint3D> massPoints)
        {
            return new GeometricSymmetryAnalyzer().GetSymmetryIndicator(massPoints, TransformerProvider.Get().FractionalSystem.Comparer);
        }

        /// <inheritdoc />
        public SymmetryIndicator GetSymmetryIndicatorAny(IEnumerable<IMassPoint3D> massPoints)
        {
            return GetSymmetryIndicator(GetCartesianSequence(massPoints));
        }

        /// <inheritdoc />
        public SymmetryCompareIndicator CompareSymmetryApprox(IEnumerable<CartesianMassPoint3D> firstSet,
            IEnumerable<CartesianMassPoint3D> secondSet)
        {
            return new SymmetryCompareIndicator(GetSymmetryIndicator(firstSet), GetSymmetryIndicator(secondSet), IndicatorComparer);
        }

        /// <inheritdoc />
        public SymmetryCompareIndicator CompareSymmetryApproxAny(IEnumerable<IMassPoint3D> firstSet, IEnumerable<IMassPoint3D> secondSet)
        {
            return CompareSymmetryApprox(GetCartesianSequence(firstSet), GetCartesianSequence(secondSet));
        }


        /// <summary>
        ///     Creates a sequence with cartesian coordinates from any kind of mass point sequence
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<CartesianMassPoint3D> GetCartesianSequence(IEnumerable<IMassPoint3D> source)
        {
            var sourceCollection = source.ToCollection();
            return sourceCollection.Zip(TransformerProvider.Get().ToCartesian(sourceCollection),
                (org, vector) => new CartesianMassPoint3D(org.Mass, vector));
        }
    }
}