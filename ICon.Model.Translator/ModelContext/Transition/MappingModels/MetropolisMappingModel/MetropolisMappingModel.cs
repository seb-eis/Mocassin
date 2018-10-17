using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class MetropolisMappingModel : IMetropolisMappingModel
    {
        /// <inheritdoc />
        public bool IsSourceInversion { get; set; }

        /// <inheritdoc />
        public MetropolisMapping Mapping { get; set; }

        /// <inheritdoc />
        public IMetropolisMappingModel InverseMapping { get; set; }

        /// <inheritdoc />
        public Fractional3D StartVector3D { get; set; }

        /// <inheritdoc />
        public Fractional3D EndVector3D { get; set; }

        /// <inheritdoc />
        public CrystalVector4D StartVector4D { get; set; }

        /// <inheritdoc />
        public CrystalVector4D EndVector4D { get; set; }

        /// <inheritdoc />
        public bool InverseIsSet => InverseMapping != null;

        /// <inheritdoc />
        public IMetropolisMappingModel CreateInverse()
        {
            return new MetropolisMappingModel
            {
                IsSourceInversion = true,
                Mapping = Mapping,
                InverseMapping = this,
                StartVector3D = EndVector3D,
                EndVector3D = StartVector3D,
                StartVector4D = EndVector4D,
                EndVector4D = StartVector4D
            };
        }

        /// <inheritdoc />
        public bool LinkIfInverseMatch(IMetropolisMappingModel mappingModel)
        {
            if (Mapping.Transition != mappingModel.Mapping.Transition || !StartVector4D.Equals(mappingModel.EndVector4D))
                return false;

            InverseMapping = mappingModel;
            mappingModel.InverseMapping = this;
            return true;
        }
    }
}
