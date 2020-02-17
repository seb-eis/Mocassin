using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies.ConflictHandling
{
    /// <summary>
    ///     Unstable environment change handler that updates the mismatching information if an unstable environment is changed
    /// </summary>
    public class UnstableEnvironmentChangeHandler : ObjectConflictHandler<UnstableEnvironment, EnergyModelData>
    {
        /// <inheritdoc />
        public UnstableEnvironmentChangeHandler(IDataAccessor<EnergyModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }

        /// <inheritdoc />
        public override ConflictReport HandleConflicts(UnstableEnvironment envInfo)
        {
            var report = new ConflictReport();
            UpdatePairInteractions(envInfo, report);
            UpdateGroupInteractions(envInfo, report);
            return report;
        }

        /// <summary>
        ///     Updates the asymmetric pair interaction pool with new or removed interactions and re-links them within the unstable
        ///     environment info
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="report"></param>
        protected void UpdatePairInteractions(UnstableEnvironment envInfo, ConflictReport report)
        {
            var newPairs = GetNewAsymmetricPairs(envInfo, ModelProject).ToList();
            var comparer = new VectorComparer3D<Fractional3D>(ModelProject.GeometryNumeric.RangeComparer);

            var warning = ModelMessageSource.CreateConflictHandlingWarning(this);
            for (var i = 0; i < newPairs.Count; i++)
            {
                switch (envInfo.PairInteractions.FirstOrDefault(value => IsEquivalentInteraction(value, newPairs[i], comparer)))
                {
                    case AsymmetricPairInteraction oldPair:
                        newPairs[i] = oldPair;
                        var detail = $"Reassigned energy dictionary from pair ({oldPair.Index}) to ({newPairs[i].Index})";
                        warning.AddDetails(detail);
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
        ///     Takes the list of pair interactions (New or reused) and updates the existing model data list with the information.
        ///     This filters out any interaction
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="newPairs"></param>
        protected void UpdateInteractionIndexing(UnstableEnvironment envInfo, IList<AsymmetricPairInteraction> newPairs)
        {
            var dataList = DataAccess.Query(data => data.UnstablePairInteractions);
            var uniquePairs = new MultisetList<AsymmetricPairInteraction>(GetInteractionComparer(), 100) {newPairs};

            foreach (var item in dataList.Where(value => value.Position0 != envInfo.CellReferencePosition))
                uniquePairs.Add(item);

            dataList.Clear();
            for (var i = 0; i < uniquePairs.Count; i++)
            {
                uniquePairs[i].Index = i;
                dataList.Add(uniquePairs[i]);
            }
        }

        /// <summary>
        ///     Takes all the new and reused pair interactions and replaces the content of the environment pair interaction linking
        ///     list with the new values
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="usedPairs"></param>
        /// <param name="report"></param>
        protected void UpdateEnvironmentLinking(UnstableEnvironment envInfo, IList<AsymmetricPairInteraction> usedPairs,
            ConflictReport report)
        {
            if (envInfo.PairInteractions.Count != usedPairs.Count)
            {
                var detail =
                    $"The unstable environment was updated from ({envInfo.PairInteractions.Count}) to ({usedPairs.Count}) asymmetric unique pairs";
                report.AddWarning(ModelMessageSource.CreateConflictHandlingWarning(this, detail));
            }

            envInfo.PairInteractions.Clear();
            foreach (var item in usedPairs)
                envInfo.PairInteractions.Add(item);
        }

        /// <summary>
        ///     Get the new asymmetric pair interactions that result from the passed unstable environment
        /// </summary>
        /// <param name="envInfo"></param>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        protected IEnumerable<AsymmetricPairInteraction> GetNewAsymmetricPairs(IUnstableEnvironment envInfo,
            IModelProject modelProject)
        {
            var unitCellProvider = modelProject.GetManager<IStructureManager>().QueryPort.Query(port => port.GetFullUnitCellProvider());
            var finder = new PairInteractionFinder(unitCellProvider, modelProject.SpaceGroupService);
            return finder.CreateUniqueAsymmetricPairs(envInfo.AsSingleton(), modelProject.GeometryNumeric.RangeComparer);
        }

        /// <summary>
        ///     Checks if two pair interactions are identical by abusing the fact that the pair interaction finder results in
        ///     exactly the same reference pair interaction as long as the structure definition does not change
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected bool IsEquivalentInteraction(IAsymmetricPairInteraction lhs, IAsymmetricPairInteraction rhs,
            VectorComparer3D<Fractional3D> comparer)
        {
            return lhs.Position0 == rhs.Position0
                   && lhs.Position1 == rhs.Position1
                   && comparer.Equals(lhs.SecondPositionVector, rhs.SecondPositionVector);
        }

        /// <summary>
        ///     Makes an asymmetric pair interaction comparer that sorts by unit cell position index and target vector
        /// </summary>
        /// <returns></returns>
        protected IComparer<AsymmetricPairInteraction> GetInteractionComparer()
        {
            int Compare(AsymmetricPairInteraction lhs, AsymmetricPairInteraction rhs)
            {
                var indexCompare = lhs.Position0.Index.CompareTo(rhs.Position0.Index);
                return indexCompare == 0 ? lhs.Distance.CompareTo(rhs.Distance) : indexCompare;
            }

            return Comparer<AsymmetricPairInteraction>.Create(Compare);
        }

        /// <summary>
        ///     Updates all group interactions affiliated with this unstable environment
        ///     (Currently just deprecates all affiliated group interactions and clears environment group interaction list)
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="report"></param>
        protected void UpdateGroupInteractions(UnstableEnvironment environment, ConflictReport report)
        {
            var counter = 0;
            foreach (var groupInteraction in DataAccess.Query(data => data.GroupInteractions))
            {
                if (!environment.GroupInteractions.Contains(groupInteraction))
                    continue;

                counter++;
                groupInteraction.Deprecate();
            }

            environment.GroupInteractions.Clear();

            if (counter == 0)
                return;

            const string detail0 = "Recovery of group interaction definitions on unstable environment changes is currently not supported";
            const string detail1 = "Deprecated all group interactions affiliated with the changed unstable environment to avoid conflicts";
            report.AddWarning(ModelMessageSource.CreateContentResetWarning(this, detail0, detail1));
        }
    }
}