using System.Collections.Generic;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <inheritdoc cref="Mocassin.Model.Translator.DbBuilder.ITransitionDbModelBuilder"/>
    public class TransitionDbModelBuilder : DbModelBuilder, ITransitionDbModelBuilder
    {
        /// <inheritdoc />
        public TransitionDbModelBuilder(IProjectModelContext modelContext)
            : base(modelContext)
        {
        }

        /// <inheritdoc />
        public TransitionModel BuildModel(IKineticSimulationModel simulationModel)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public TransitionModel BuildModel(IMetropolisSimulationModel simulationModel)
        {
            throw new System.NotImplementedException();
        }

        public List<JumpCollectionEntity> GetJumpCollectionEntities(IKineticSimulationModel simulationModel)
        {
            return null;
        }

        public JumpCollectionEntity GetJumpCollectionEntity(IKineticTransitionModel transitionModel)
        {
            return null;
        }

        public JumpDirectionEntity GetJumpDirectionEntity(IKineticMappingModel mappingModel)
        {
            var entity = new JumpDirectionEntity
            {
                JumpLength = mappingModel.TransitionSequence4D.Count
            };
            return null;
        }
    }
}