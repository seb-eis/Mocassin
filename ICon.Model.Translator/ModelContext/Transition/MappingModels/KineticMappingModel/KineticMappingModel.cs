using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class KineticMappingModel : IKineticMappingModel
    {
        /// <inheritdoc />
        public IKineticTransitionModel TransitionModel { get; set; }

        /// <inheritdoc />
        public bool InverseIsSet => InverseMapping != null;

        /// <inheritdoc />
        public KineticMapping Mapping { get; set; }

        /// <inheritdoc />
        public IKineticMappingModel InverseMapping { get; set; }

        /// <inheritdoc />
        public IList<Vector4I> PositionSequence4D => Mapping.EncodedPath;

        /// <inheritdoc />
        public IList<Fractional3D> PositionSequence3D => Mapping.FractionalPath;

        /// <inheritdoc />
        public IList<Vector4I> TransitionSequence4D { get; set; }

        /// <inheritdoc />
        public IList<Fractional3D> TransitionSequence3D { get; set; }

        /// <inheritdoc />
        public Matrix2D PositionMovementMatrix { get; set; }

        /// <inheritdoc />
        public int PathLength => Mapping.PathLength;

        /// <inheritdoc />
        public ITransitionMappingModel InverseMappingBase => InverseMapping;

        /// <inheritdoc />
        /// <remarks> This value cannot be set for a kinetic transition </remarks>
        public Vector4I StartVector4D
        {
            get => PositionSequence4D[0];
            set => throw new NotSupportedException("Cannot manipulate start vector on kinetic mapping");
        }

        /// <inheritdoc />
        public bool LinkIfGeometricInversion(IKineticMappingModel inverseModel)
        {
            if (PositionSequence4D.Select(x => x.P).Zip(inverseModel.PositionSequence4D.Reverse().Select(x => x.P), (x, y) => x - y).Any(x => x != 0))
                return false;
            if (!GetGeometricInversionMovementMatrix().Equals(inverseModel.PositionMovementMatrix))
                return false;

            InverseMapping = inverseModel;
            inverseModel.InverseMapping = this;
            return true;
        }

        /// <inheritdoc />
        public IKineticMappingModel CreateGeometricInversion()
        {
            var result = new KineticMappingModel
            {
                TransitionModel = TransitionModel.InverseTransitionModel ?? throw new InvalidOperationException("Inverse transition model is unknown!"),
                Mapping = Mapping.CreateGeometricInversion(),
                InverseMapping = this,
                TransitionSequence3D = GetGeometricInversionTransitionSequence3D(),
                TransitionSequence4D = GetGeometricInversionTransitionSequence4D(),
                PositionMovementMatrix = GetGeometricInversionMovementMatrix()
            };
            return result;
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> GetMovementSequence()
        {
            if (PositionMovementMatrix == null)
                yield break;

            for (var col = 0; col < PositionMovementMatrix.Cols; col++)
            {
                yield return new Fractional3D(PositionMovementMatrix[0, col], PositionMovementMatrix[1, col],
                    PositionMovementMatrix[2, col]);
            }
        }

        /// <inheritdoc />
        public IEnumerable<Vector4I> GetTransitionSequence() => TransitionSequence4D.AsEnumerable();

        /// <inheritdoc />
        public ITransitionModel GetTransitionModel() => TransitionModel;

        /// <summary>
        ///     Gets an inverted version of the fractional transition sequence
        /// </summary>
        /// <returns></returns>
        protected IList<Fractional3D> GetGeometricInversionTransitionSequence3D()
        {
            var result = new List<Fractional3D>(TransitionSequence3D.Count);
            var lastId = PositionSequence3D.Count - 1;

            for (var i = lastId - 1; i >= 0; i--)
                result.Add(PositionSequence3D[i] - PositionSequence3D[lastId]);

            return result;
        }

        /// <summary>
        ///     Gets an inverted version of the encoded transition sequence
        /// </summary>
        /// <returns></returns>
        protected IList<Vector4I> GetGeometricInversionTransitionSequence4D()
        {
            var result = new List<Vector4I>(TransitionSequence4D.Count);
            var lastId = PositionSequence4D.Count - 1;

            for (var i = lastId - 1; i >= 0; i--)
                result.Add(PositionSequence4D[i] - PositionSequence4D[lastId]);

            return result;
        }

        /// <summary>
        ///     Get an inverted version of the movement matrix
        /// </summary>
        /// <returns></returns>
        protected Matrix2D GetGeometricInversionMovementMatrix()
        {
            var result = PositionMovementMatrix.GetRowReversed();
            return result;
        }
    }
}