using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IPositionTransitionModelBuilder" />
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
            var cellReferencePositions = ModelProject
                .GetManager<IStructureManager>().QueryPort
                .Query(port => port.GetCellReferencePositions());

            return cellReferencePositions
                .Select(a => CreatePositionTransitionModel(a, modelContext))
                .ToList();
        }

        /// <summary>
        ///     Creates the position transition model for the passed transition model
        /// </summary>
        /// <param name="cellReferencePosition"></param>
        /// <param name="modelContext"></param>
        /// <returns></returns>
        protected IPositionTransitionModel CreatePositionTransitionModel(ICellReferencePosition cellReferencePosition,
            ITransitionModelContext modelContext)
        {
            var model = new PositionTransitionModel
            {
                CellReferencePosition = cellReferencePosition,
                KineticTransitionModels = GetPossibleModels(cellReferencePosition, modelContext.KineticTransitionModels),
                MetropolisTransitionModels = GetPossibleModels(cellReferencePosition, modelContext.MetropolisTransitionModels)
            };

            return model;
        }

        /// <summary>
        ///     Searches a list of kinetic transition models for entries that are possible on the passed unit cell position
        /// </summary>
        /// <param name="cellReferencePosition"></param>
        /// <param name="transitionModels"></param>
        /// <returns></returns>
        protected IList<IKineticTransitionModel> GetPossibleModels(ICellReferencePosition cellReferencePosition,
            IList<IKineticTransitionModel> transitionModels)
        {
            return transitionModels
                .Where(a => a.GetStartCellReferencePosition() == cellReferencePosition)
                .ToList();
        }

        /// <summary>
        ///     Searches a list of metropolis transition models for entries that are possible on the passed unit cell position
        /// </summary>
        /// <param name="cellReferencePosition"></param>
        /// <param name="transitionModels"></param>
        /// <returns></returns>
        protected IList<IMetropolisTransitionModel> GetPossibleModels(ICellReferencePosition cellReferencePosition,
            IList<IMetropolisTransitionModel> transitionModels)
        {
            return transitionModels
                .Where(a => a.Transition.FirstCellReferencePosition == cellReferencePosition)
                .ToList();
        }
    }
}