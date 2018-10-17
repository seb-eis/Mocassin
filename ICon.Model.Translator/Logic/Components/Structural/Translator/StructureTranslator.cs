using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;
using Mocassin.Model.Energies;
using Mocassin.Model.Particles;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Translates the .NET level structure model data into the simulation data context format
    /// </summary>
    public class StructureTranslator : TranslatorBase, IStructureTranslator
    {
        protected List<StructureModel> StructureModels { get; set; }

        public IList<StructureModel> CreateStructureModels(ITranslationContext translationContext)
        {
            PrepareTranslatorDataContext(translationContext);

            foreach (var item in TranslationContext.BaseSimulations)
            {
                StructureModels.Add(CreateModelForSimulationBasis(item));
            }

            return StructureModels;
        }

        protected void PrepareTranslatorDataContext(ITranslationContext translationContext)
        {
            CheckAndPrepareModelContext(translationContext);
            StructureModels = new List<StructureModel>(TranslationContext.BaseSimulations.Count);
        }

        protected StructureModel CreateModelForSimulationBasis(ISimulation simulationBase)
        {
            var structureModel = new StructureModel
            {
                InteractionRange = CreateStablePositionInteractionRange(),
                EnvironmentDefinitions = CreateEnvironmentDefinitionList()
            };

            if (simulationBase is IKineticSimulation kineticSimulation)
            {
                structureModel.NumOfTrackersPerCell = GetNumberOfStaticTrackersPerCell(kineticSimulation);
                structureModel.NumOfGlobalTrackers = GetNumberOfGlobalTrackers(kineticSimulation);
                structureModel.PositionCellTrackerIndexing = CreateStaticTrackerIndexing(kineticSimulation);
            }

            return structureModel;
        }

        InteractionRange CreateStablePositionInteractionRange()
        {
            var interactionRange = new InteractionRange();
            var cellParameters = StructureManager.QueryPort.Query(data => data.GetCellParameters());
            var environmentInfo = EnergyManager.QueryPort.Query(data => data.GetStableEnvironmentInfo());

            interactionRange.Structure.A = (int) Math.Ceiling(environmentInfo.MaxInteractionRange / cellParameters.ParamA);
            interactionRange.Structure.B = (int) Math.Ceiling(environmentInfo.MaxInteractionRange / cellParameters.ParamB);
            interactionRange.Structure.C = (int) Math.Ceiling(environmentInfo.MaxInteractionRange / cellParameters.ParamC);

            return interactionRange;
        }

        protected List<EnvironmentDefinitionEntity> CreateEnvironmentDefinitionList()
        {
            return null;
        }

        protected int GetNumberOfStaticTrackersPerCell(IKineticSimulation kineticSimulation)
        {
            return 0;
        }

        /// <summary>
        /// Calculates the number of global trackers based upon all possible kinetic transitions
        /// </summary>
        /// <returns></returns>
        protected int GetNumberOfGlobalTrackers(IKineticSimulation kineticSimulation)
        {
            int count = ParticleManager.QueryPort.Query(data => data.GetValidParticleCount());
            count *= TransitionManager.QueryPort.Query(data => data.GetKineticTransitionCount());
            return count;
        }

        IndexRedirectionListEntity CreateStaticTrackerIndexing(IKineticSimulation kineticSimulation)
        {
            return null;
        }
    }
}
