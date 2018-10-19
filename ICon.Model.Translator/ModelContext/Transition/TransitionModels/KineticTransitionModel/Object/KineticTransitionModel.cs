using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IKineticTransitionModel" />
    public class KineticTransitionModel : ModelComponentBase, IKineticTransitionModel
    {
        /// <inheritdoc />
        public bool HasInversion { get; set; }

        /// <inheritdoc />
        public IKineticTransition Transition { get; set; }

        /// <inheritdoc />
        public IKineticTransitionModel InverseTransitionModel { get; set; }

        /// <inheritdoc />
        public IList<IKineticMappingModel> MappingModels { get; set; }

        /// <inheritdoc />
        public IList<IKineticRuleModel> RuleModels { get; set; }

        /// <inheritdoc />
        public IParticleSet MobileParticles { get; set; }

        /// <inheritdoc />
        public IParticle EffectiveParticle { get; set; }

        /// <inheritdoc />
        public IList<int> AbstractMovement { get; set; }

        /// <inheritdoc />
        public IKineticTransitionModel CreateInverse()
        {
            return new KineticTransitionModel
            {
                Transition = Transition,
                InverseTransitionModel = this,
                AbstractMovement = GetInverseAbstractMovement(),
                MobileParticles = MobileParticles,
                EffectiveParticle = EffectiveParticle
            };
        }

        /// <inheritdoc />
        public bool MappingsContainInversion()
        {
            if (MappingModels.Count == 0) return false;
            var checkMapping = MappingModels.First();
            return checkMapping.Mapping.StartUnitCellPosition == checkMapping.Mapping.EndUnitCellPosition;
        }

        /// <inheritdoc />
        public IUnitCellPosition GetStartUnitCellPosition()
        {
            return MappingModels.First().Mapping.StartUnitCellPosition;
        }

        /// <summary>
        ///     Gets the inverted abstract movement description
        /// </summary>
        /// <returns></returns>
        protected IList<int> GetInverseAbstractMovement()
        {
            return AbstractMovement.Select(a => -a).Reverse().ToList();
        }
    }
}