using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IPositionTransitionModelBuilder"/>
    public class PositionTransitionModelBuilder : ModelBuilderBase, IPositionTransitionModelBuilder
    {
        /// <inheritdoc />
        public PositionTransitionModelBuilder(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IPositionTransitionModel> BuildModels(ITransitionModelContext modelContext, Task transitionBuildTask)
        {
            transitionBuildTask.Wait();
            var unitCellPositions = ModelProject
                .GetManager<IStructureManager>().QueryPort
                .Query(port => port.GetUnitCellPositions());

            return unitCellPositions
                .Select(a => CreatePositionTransitionModel(a, modelContext))
                .ToList();
        }

        /// <summary>
        /// Creates the position transition model for the passed transition model
        /// </summary>
        /// <param name="unitCellPosition"></param>
        /// <param name="modelContext"></param>
        /// <returns></returns>
        protected IPositionTransitionModel CreatePositionTransitionModel(IUnitCellPosition unitCellPosition,
            ITransitionModelContext modelContext)
        {
            var model = new PositionTransitionModel
            {
                UnitCellPosition = unitCellPosition,
                KineticTransitionModels = GetPossibleModels(unitCellPosition, modelContext.KineticTransitionModels),
                MetropolisTransitionModels = GetPossibleModels(unitCellPosition, modelContext.MetropolisTransitionModels)
            };

            return model;
        }

        /// <summary>
        /// Searches a list of kinetic transition models for entries that are possible on the passed unit cell position
        /// </summary>
        /// <param name="unitCellPosition"></param>
        /// <param name="transitionModels"></param>
        /// <returns></returns>
        protected IList<IKineticTransitionModel> GetPossibleModels(IUnitCellPosition unitCellPosition,
            IList<IKineticTransitionModel> transitionModels)
        {
            return transitionModels
                .Where(a => a.GetStartUnitCellPosition() == unitCellPosition)
                .ToList();
        }

        /// <summary>
        /// Searches a list of metropolis transition models for entries that are possible on the passed unit cell position
        /// </summary>
        /// <param name="unitCellPosition"></param>
        /// <param name="transitionModels"></param>
        /// <returns></returns>
        protected IList<IMetropolisTransitionModel> GetPossibleModels(IUnitCellPosition unitCellPosition,
            IList<IMetropolisTransitionModel> transitionModels)
        {
            return transitionModels
                .Where(a => a.Transition.FirstUnitCellPosition == unitCellPosition)
                .ToList();
        }
    }
}