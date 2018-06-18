using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICon.Framework.Extensions;
using ICon.Framework.Operations;
using ICon.Framework.Collections;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;

namespace ICon.Model.Energies.ConflictHandling
{
    /// <summary>
    /// Unstable environment change handler that updates the mismatching information if an unstable environment is changed
    /// </summary>
    public class UnstableEnvironmentChangeHandler : ObjectConflictHandler<UnstableEnvironment, EnergyModelData>
    {
        /// <summary>
        /// Create new unstable environement change handler that uses the provided data access and project service instance
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        public UnstableEnvironmentChangeHandler(IDataAccessor<EnergyModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {

        }

        /// <summary>
        /// Takes a changed unstable environment and corrects the internal data object in terms of interactions and potentially invalid
        /// groupings
        /// </summary>
        /// <param name="envInfo"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(UnstableEnvironment envInfo)
        {
            var report = new ConflictReport();
            UpdatePairInteractions(envInfo, report);
            return report;
        }

        /// <summary>
        /// Updates the asymmetric pair interaction pool with new or removed interactions and relinks them within the unstable environment info
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="dataAccess"></param>
        /// <param name="report"></param>
        /// <param name="projectServices"></param>
        protected void UpdatePairInteractions(UnstableEnvironment envInfo, ConflictReport report)
        {
            var newPairs = GetNewAsymmetricPairs(envInfo, ProjectServices).ToList();
            var comparer = new VectorComparer3D<Fractional3D>(ProjectServices.GeometryNumerics.RangeComparer);

            var warning = ModelMessages.CreateConflictHandlingWarning(this);
            for (int i = 0; i < newPairs.Count;i++)
            {
                switch (envInfo.PairInteractions.FirstOrDefault(value => IsEquivalentInteraction(value, newPairs[i], comparer)))
                {
                    case AsymmetricPairInteraction oldPair:
                        newPairs[i] = oldPair;
                        var detail = $"Reassigned energy dictionary from pair ({oldPair.Index}) to ({newPairs[i].Index})";
                        warning.AddDetails(detail);
                        break;

                    default:
                        break;
                }
            }

            if (warning.Details.Count != 0)
            {
                warning.AddDetails($"Reassigned ({warning.Details.Count}) of ({newPairs.Count}) energy dictionaries");
                report.AddWarning(warning);
            }

            UpdateInteractionIndexing(envInfo, newPairs);
            UpdateEnvironmentLinking(envInfo, newPairs, report);
        }

        /// <summary>
        /// Takes the list of pair interactions (New or reused) and updates the existing model data list with the information. This filters out any interaction
        /// 
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="newPairs"></param>
        protected void UpdateInteractionIndexing(UnstableEnvironment envInfo, IList<AsymmetricPairInteraction> newPairs)
        {
            var dataList = DataAccess.Query(data => data.AsymmetricPairInteractions);
            var uniquePairs = new MultisetList<AsymmetricPairInteraction>(GetInteractionComparer(), 100) { newPairs };

            foreach (var item in dataList.Where(value => value.Position0 != envInfo.UnitCellPosition))
            {
                uniquePairs.Add(item);
            }

            dataList.Clear();
            for (int i = 0; i < uniquePairs.Count; i++)
            {
                uniquePairs[i].Index = i;
                dataList.Add(uniquePairs[i]);
            }
        }

        /// <summary>
        /// Takes all the new and reused pair interactions and replaces the content of the environment pair interaction linking list with the new values
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="usedPairs"></param>
        /// <param name="report"></param>
        protected void UpdateEnvironmentLinking(UnstableEnvironment envInfo, IList<AsymmetricPairInteraction> usedPairs, ConflictReport report)
        {
            if (envInfo.PairInteractions.Count != usedPairs.Count)
            {
                var detail = $"The unstable environment was updated from ({envInfo.PairInteractions.Count}) to ({usedPairs.Count}) asymmetric unique pairs";
                report.AddWarning(ModelMessages.CreateConflictHandlingWarning(this, detail));
            }

            envInfo.PairInteractions.Clear();
            foreach (var item in usedPairs)
            {
                envInfo.PairInteractions.Add(item);
            }
        }

        /// <summary>
        /// Get the new asymmetric pair interactions that result from the passed unstable environment
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="projectServices"></param>
        /// <returns></returns>
        protected IEnumerable<AsymmetricPairInteraction> GetNewAsymmetricPairs(IUnstableEnvironment envInfo, IProjectServices projectServices)
        {
            var unitCellProvider = projectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            var finder = new PairInteractionFinder(unitCellProvider, projectServices.SpaceGroupService);
            return finder.CreateUniqueAsymmetricPairs(envInfo.AsSingleton(), projectServices.GeometryNumerics.RangeComparer);
        }

        /// <summary>
        /// Checks if two pair interactions are identical by abusing the fact that the pair interaction finder results in
        /// exactly the same refernce pair interaction as long as the structure definition does not change
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected bool IsEquivalentInteraction(IAsymmetricPairInteraction lhs, IAsymmetricPairInteraction rhs, VectorComparer3D<Fractional3D> comparer)
        {
            return lhs.Position0 == rhs.Position0
                && lhs.Position1 == rhs.Position1
                && comparer.Equals(lhs.GetSecondPositionVector(), rhs.GetSecondPositionVector());
        }

        /// <summary>
        /// Makes anasymmetric pair interaction comparer that sorts by unit cell position index and target vector
        /// </summary>
        /// <returns></returns>
        protected IComparer<AsymmetricPairInteraction> GetInteractionComparer()
        {
            int Compare(AsymmetricPairInteraction lhs, AsymmetricPairInteraction rhs)
            {
                int indexCompare = lhs.Position0.Index.CompareTo(rhs.Position0.Index);
                if (indexCompare == 0)
                {
                    return lhs.Distance.CompareTo(rhs.Distance);
                }
                return indexCompare;
            }
            return Comparer<AsymmetricPairInteraction>.Create(Compare);
        }
    }
}
