using System.Linq;
using System.Collections.Generic;

using ICon.Mathematics.ValueTypes;
using ICon.Mathematics.Comparers;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;

namespace ICon.Model.Energies.ConflictHandling
{
    /// <summary>
    /// Conflict resolver for a change in the stable environment info parameter within the energy manager
    /// </summary>
    public class StableEnvironmentInfoChangeHandler : ObjectConflictHandler<StableEnvironmentInfo, EnergyModelData>
    {
        /// <summary>
        /// Main resolver method that handles the changes induced due to the new stable environment information
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        /// <returns></returns>
        public override ConflictReport Resolve(StableEnvironmentInfo obj, IDataAccessor<EnergyModelData> dataAccess, IProjectServices projectServices)
        {
            var report = new ConflictReport();
            UpdatePairInteractionModel(obj, dataAccess, report, projectServices);
            return report;
        }

        /// <summary>
        /// Updates the pair interaction model to the new set of pair interactions and inputs them into the model data object
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="report"></param>
        /// <param name="projectServices"></param>
        protected void UpdatePairInteractionModel(IStableEnvironmentInfo info, IDataAccessor<EnergyModelData> dataAccess, ConflictReport report, IProjectServices projectServices)
        {
            var newPairs = GetNewPairInteractions(info, projectServices);
            var oldPairs = dataAccess.Query(data => data.SymmetricPairInteractions);

            PullEnergyInfoFromOldModel(oldPairs, newPairs, report, projectServices.GeometryNumerics.RangeComparer);
            MoveNewPairsToModelList(oldPairs, newPairs, report);
        }

        /// <summary>
        /// Finds all interactions that are equivalent in the new model and corrects the energy dictionaries to the ones of the old model list
        /// </summary>
        /// <param name="oldPairs"></param>
        /// <param name="newPairs"></param>
        /// <param name="report"></param>
        /// <param name="comparer"></param>
        protected void PullEnergyInfoFromOldModel(IList<SymmetricPairInteraction> oldPairs, IList<SymmetricPairInteraction> newPairs, ConflictReport report, DoubleComparer comparer)
        {
            var vectorComparer = new VectorComparer3D<Fractional3D>(comparer);
            var warning = ModelMessages.CreateConflictHandlingWarning(this);
            foreach (var oldPair in oldPairs)
            {
                switch (newPairs.FirstOrDefault(value => IsEquivalentInteraction(value, oldPair, vectorComparer)))
                {
                    case SymmetricPairInteraction newPair when newPair != null:
                        newPair.EnergyDictionary = oldPair.EnergyDictionary;

                        var detail = $"Reused energy definition from pair ({oldPair.Index}) in new pair ({newPair.Index})";
                        warning.AddDetails(detail);
                        break;

                    default:
                        break;
                }
            }
            if (warning.Details.Count != 0)
            {
                warning.AddDetails($"Recycled ({warning.Details.Count}) of ({newPairs.Count}) energy definitions");
                report.AddWarning(warning);
            }
        }

        /// <summary>
        /// Deltes the old model list and inputs the new pairs into the old model list withou changing the actual list in the model object
        /// </summary>
        /// <param name="oldPairs"></param>
        /// <param name="newPairs"></param>
        protected void MoveNewPairsToModelList(IList<SymmetricPairInteraction> oldPairs, IList<SymmetricPairInteraction> newPairs, ConflictReport report)
        {
            if (oldPairs.Count != newPairs.Count)
            {
                var detail = $"The pair interaction set was updated ({oldPairs.Count}) to ({newPairs.Count}) interactions";
                report.AddWarning(ModelMessages.CreateConflictHandlingWarning(this, detail));
            }

            oldPairs.Clear();
            foreach (var item in newPairs)
            {
                oldPairs.Add(item);
            }
        }

        /// <summary>
        /// Checks if two pair interactions are equivalent in terms of symmetry. This function uses the fact that the radial search routine always results
        /// in the same refernce pair interaction as long as the structure is not changed and only works as long as this statement is true
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected bool IsEquivalentInteraction(SymmetricPairInteraction lhs, SymmetricPairInteraction rhs, VectorComparer3D<Fractional3D> comparer)
        {
            return lhs.Position0 == rhs.Position0
                && lhs.Position1 == rhs.Position1
                && comparer.Equals(lhs.GetSecondPositionVector(), rhs.GetSecondPositionVector());
        }

        /// <summary>
        /// Calculates the new set of pair interactions with the provided project services and new stable environment info
        /// </summary>
        /// <param name="info"></param>
        /// <param name="projectServices"></param>
        /// <returns></returns>
        protected IList<SymmetricPairInteraction> GetNewPairInteractions(IStableEnvironmentInfo info, IProjectServices projectServices)
        {
            var energyQueries = projectServices.GetManager<IEnergyManager>().QueryPort;
            var structureQueries = projectServices.GetManager<IStructureManager>().QueryPort;

            var unitCellProvider = structureQueries.Query(port => port.GetFullUnitCellProvider());
            var positions = structureQueries.Query(port => port.GetUnitCellPositions().Where(value => value.IsValidAndStable()));
            var comparer = projectServices.GeometryNumerics.RangeComparer;

            var interactionFinder = new PairInteractionFinder(unitCellProvider, projectServices.SpaceGroupService);
            return interactionFinder.CreateUniqueSymmetricPairs(positions, info, comparer).ToList();
        }
    }
}
