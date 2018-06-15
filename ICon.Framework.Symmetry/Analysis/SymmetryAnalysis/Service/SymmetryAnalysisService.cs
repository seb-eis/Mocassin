using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Provider;
using ICon.Mathematics.Coordinates;
using ICon.Mathematics.ValueTypes;

namespace ICon.Symmetry.Analysis
{
    public class SymmetryAnalysisService : ISymmetryAnalysisService
    {
        /// <summary>
        /// The comparer for the approx symmetry indicator comparsion
        /// </summary>
        public IComparer<SymmetryIndicator> IndicatorComparer { get; set; }

        /// <summary>
        /// Provider for the vector transformer to change coordinate systems
        /// </summary>
        public IObjectProvider<VectorTransformer> TransformerProvider { get; set; }

        /// <summary>
        /// Create new symmetry analysis service from indicator comparer and vector transformer provider
        /// </summary>
        /// <param name="indicatorComparer"></param>
        /// <param name="transformerProvider"></param>

        public SymmetryAnalysisService(IComparer<SymmetryIndicator> indicatorComparer, IObjectProvider<VectorTransformer> transformerProvider)
        {
            IndicatorComparer = indicatorComparer ?? throw new ArgumentNullException(nameof(indicatorComparer));
            TransformerProvider = transformerProvider ?? throw new ArgumentNullException(nameof(transformerProvider));
        }


        /// <summary>
        /// Calcultes the symmetry indicator for a sequence of mass points using the double comparer of the vector transformer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="massPoints"></param>
        /// <returns></returns>
        public SymmetryIndicator GetSymmetryIndicator<T1>(IEnumerable<T1> massPoints) where T1 : struct, ICartesianMassPoint3D<T1>
        {
            return new GeometricSymmetryAnalyzer().GetSymmetryIndicator(massPoints, TransformerProvider.Get().FractionalSystem.Comparer);
        }

        /// <summary>
        /// Get an indicator object that can be used for approximated symmetry comparisons for mass point sets
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="massPoints"></param>
        /// <returns></returns>
        public SymmetryIndicator GetSymmetryIndicatorAny<T1>(IEnumerable<T1> massPoints) where T1 : struct, IMassPoint3D<T1>
        {
            return GetSymmetryIndicator(GetCartesianSequence(massPoints));
        }

        /// <summary>
        /// Compares two sequences of mass points for approximate symmetric equality by symmetry indicator comparison
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="firstSet"></param>
        /// <param name="secondSet"></param>
        /// <returns></returns>
        public SymmetryCompareIndicator CompareSymmetryApprox<T1>(IEnumerable<T1> firstSet, IEnumerable<T1> secondSet) where T1 : struct, ICartesianMassPoint3D<T1>
        {
            return new SymmetryCompareIndicator(GetSymmetryIndicator(firstSet), GetSymmetryIndicator(secondSet), IndicatorComparer);
        }

        /// <summary>
        /// Compares two sequences of mass points in arbitrary coordinate systems for approximate symmetry equivalency
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="firstSet"></param>
        /// <param name="secondSet"></param>
        /// <returns></returns>
        public SymmetryCompareIndicator CompareSymmetryApproxAny<T1, T2>(IEnumerable<T1> firstSet, IEnumerable<T2> secondSet) where T1 : struct, IMassPoint3D<T1> where T2 : struct, IMassPoint3D<T2>
        {
            return CompareSymmetryApprox(GetCartesianSequence(firstSet), GetCartesianSequence(secondSet));
        }

        /// <summary>
        /// Compares two sets of vectors for exact symmetry equivalency using the provided comparer for the individual vectors
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="firstSet"></param>
        /// <param name="secondSet"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool CompareSymmetryExact<T1>(IEnumerable<T1> firstSet, IEnumerable<T1> secondSet, IComparer<T1> comparer) where T1 : struct, ICartesian3D<T1>
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to transform a sequence of vector objects onto another by transformation operations and provides the transfromation sequence if successful
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="comparer"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool TryGetTransformationSequence<T1>(IEnumerable<T1> source, IEnumerable<T1> target, IComparer<T1> comparer, out IEnumerable<TransformMatrix2D> matrix) where T1 : struct, IVector3D<T1>
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a sequence with cartesian coordinates from any kind of mass point sequence
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<CartesianMassPoint3D<double>> GetCartesianSequence<T1>(IEnumerable<T1> source) where T1 : struct, IMassPoint3D<T1>
        {
            return source.Zip(TransformerProvider.Get().ToCartesian(source), (org, vector) => new CartesianMassPoint3D<double>(org.GetMass(), vector));
        }
    }
}
