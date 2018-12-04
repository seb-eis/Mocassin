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
        public IList<CrystalVector4D> PositionSequence4D => Mapping.EncodedPath;

        /// <inheritdoc />
        public IList<Fractional3D> PositionSequence3D => Mapping.FractionalPath;

        /// <inheritdoc />
        public IList<CrystalVector4D> TransitionSequence4D { get; set; }

        /// <inheritdoc />
        public IList<Fractional3D> TransitionSequence3D { get; set; }

        /// <inheritdoc />
        public Matrix2D PositionMovementMatrix { get; set; }

        /// <inheritdoc />
        /// <remarks> This value cannot be set for a kinetic transition </remarks>
        public CrystalVector4D StartVector4D
        {
            get => PositionSequence4D[0];
            set => throw new NotSupportedException("Cannot manipulate start vector on kinetic mapping");
        }

        /// <inheritdoc />
        public bool LinkIfInverseMatch(IKineticMappingModel inverseModel)
        {
            if (!PositionMovementMatrix.GetRowReversed().Equals(inverseModel.PositionMovementMatrix))
                return false;

            InverseMapping = inverseModel;
            inverseModel.InverseMapping = this;
            return true;
        }

        /// <inheritdoc />
        public IKineticMappingModel CreateInverse()
        {
            return new KineticMappingModel
            {
                TransitionModel = TransitionModel,
                Mapping = Mapping.CreateInverse(),
                InverseMapping = this,
                TransitionSequence3D = GetInverseTransitionSequence3D(),
                TransitionSequence4D = GetInverseTransitionSequence4D(),
                PositionMovementMatrix = GetInverseMovementMatrix()
            };
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

        /// <summary>
        ///     Gets an inverted version of the fractional transition sequence
        /// </summary>
        /// <returns></returns>
        protected IList<Fractional3D> GetInverseTransitionSequence3D()
        {
            return TransitionSequence3D.Reverse().Select(a => a * -1).ToList();
        }

        /// <summary>
        ///     Gets an inverted version of the encoded transition sequence
        /// </summary>
        /// <returns></returns>
        protected IList<CrystalVector4D> GetInverseTransitionSequence4D()
        {
            return TransitionSequence4D.Reverse().Select(a => a * -1).ToList();
        }

        /// <summary>
        ///     Get an inverted version of the movement matrix
        /// </summary>
        /// <returns></returns>
        protected Matrix2D GetInverseMovementMatrix()
        {
            var reversed = PositionMovementMatrix.GetRowReversed();
            reversed.ScalarMultiply(-1);
            return reversed;
        }
    }
}