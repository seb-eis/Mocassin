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
                                         .Manager<IStructureManager>().DataAccess
                                         .Query(port => port.GetCellReferencePositions());

            return cellReferencePositions
                   .Select(a => CreatePositionTransitionModel(a, modelContext))
                   .ToList();
        }

        /// <summary>
        ///     Creates the position transition model for the passed transition model
        /// </summary>
        /// <param name="cellSite"></param>
        /// <param name="modelContext"></param>
        /// <returns></returns>
        protected IPositionTransitionModel CreatePositionTransitionModel(ICellSite cellSite,
            ITransitionModelContext modelContext)
        {
            var model = new PositionTransitionModel
            {
                CellSite = cellSite,
                KineticTransitionModels = GetPossibleModels(cellSite, modelContext.KineticTransitionModels),
                MetropolisTransitionModels = GetPossibleModels(cellSite, modelContext.MetropolisTransitionModels)
            };

            return model;
        }

        /// <summary>
        ///     Searches a list of kinetic transition models for entries that are possible on the passed unit cell position
        /// </summary>
        /// <param name="cellSite"></param>
        /// <param name="transitionModels"></param>
        /// <returns></returns>
        protected IList<IKineticTransitionModel> GetPossibleModels(ICellSite cellSite,
            IList<IKineticTransitionModel> transitionModels)
        {
            return transitionModels
                   .Where(a => a.GetStartCellReferencePosition() == cellSite)
                   .ToList();
        }

        /// <summary>
        ///     Searches a list of metropolis transition models for entries that are possible on the passed unit cell position
        /// </summary>
        /// <param name="cellSite"></param>
        /// <param name="transitionModels"></param>
        /// <returns></returns>
        protected IList<IMetropolisTransitionModel> GetPossibleModels(ICellSite cellSite,
            IList<IMetropolisTransitionModel> transitionModels)
        {
            return transitionModels
                   .Where(a => a.Transition.FirstCellSite == cellSite)
                   .ToList();
        }
    }
}