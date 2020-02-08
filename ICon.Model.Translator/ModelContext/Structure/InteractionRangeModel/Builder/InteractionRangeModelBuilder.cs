using System;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class InteractionRangeModelBuilder : IInteractionRangeModelBuilder
    {
        /// <inheritdoc />
        public IInteractionRangeModel BuildModel(IModelProject modelProject)
        {
            var rangeModel = new InteractionRangeModel
            {
                InteractionRange = GetInteractionDistance(modelProject)
            };

            AddInteractionCubeInformation(rangeModel, modelProject);
            return rangeModel;
        }

        /// <summary>
        ///     Get the interaction range defined in the model project
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public double GetInteractionDistance(IModelProject modelProject)
        {
            var manager = modelProject.GetManager<IEnergyManager>();
            var environmentInfo = manager.QueryPort.Query(port => port.GetStableEnvironmentInfo());
            return environmentInfo.MaxInteractionRange;
        }

        /// <summary>
        ///     Adds the interaction cube information to the passed range model
        /// </summary>
        /// <param name="rangeModel"></param>
        /// <param name="modelProject"></param>
        public void AddInteractionCubeInformation(IInteractionRangeModel rangeModel, IModelProject modelProject)
        {
            var manager = modelProject.GetManager<IStructureManager>();
            var cellParameter = manager.QueryPort.Query(port => port.GetCellParameters());
            rangeModel.CellsInDirectionA = (int) Math.Ceiling(rangeModel.InteractionRange / cellParameter.ParamA);
            rangeModel.CellsInDirectionB = (int) Math.Ceiling(rangeModel.InteractionRange / cellParameter.ParamB);
            rangeModel.CellsInDirectionC = (int) Math.Ceiling(rangeModel.InteractionRange / cellParameter.ParamC);
        }
    }
}