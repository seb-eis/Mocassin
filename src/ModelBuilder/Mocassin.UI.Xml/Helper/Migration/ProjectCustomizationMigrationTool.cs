using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.UI.Data.Base;
using Mocassin.UI.Data.Customization;

namespace Mocassin.UI.Data.Helper.Migration
{
    /// <summary>
    ///     Migration tool that partially recycles content of <see cref="ProjectCustomizationTemplate" /> instances
    /// </summary>
    public class ProjectCustomizationMigrationTool : ProjectDataMigrationTool
    {
        /// <summary>
        ///     Get or set the used <see cref="VectorComparer3D{T}" /> for <see cref="Fractional3D" /> information
        /// </summary>
        public VectorComparer3D<Fractional3D> VectorComparer { get; set; }

        /// <summary>
        ///     Creates a new <see cref="ProjectCustomizationMigrationTool" />
        /// </summary>
        /// <param name="vectorComparer"></param>
        public ProjectCustomizationMigrationTool(VectorComparer3D<Fractional3D> vectorComparer = null)
        {
            VectorComparer = vectorComparer ?? new VectorComparer3D<Fractional3D>(NumericComparer.CreateRanged(1e-6));
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="ProjectCustomizationTemplate" /> by values that also exist on the
        ///     source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public MigrationReport Migrate(ProjectCustomizationTemplate source, ProjectCustomizationTemplate target)
        {
            Migrate(source.EnergyModelCustomization, target.EnergyModelCustomization);
            Migrate(source.TransitionModelCustomization, target.TransitionModelCustomization);
            return GenerateReport(source, target);
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="EnergyModelCustomizationData" /> by values that also exist on the
        ///     source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(EnergyModelCustomizationData source, EnergyModelCustomizationData target)
        {
            Migrate(source.StablePairEnergyParameterSets, target.StablePairEnergyParameterSets);
            Migrate(source.UnstablePairEnergyParameterSets, target.UnstablePairEnergyParameterSets);
            Migrate(source.GroupEnergyParameterSets, target.GroupEnergyParameterSets);
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="EnergyModelCustomizationData" /> by values that also exist on the
        ///     source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(TransitionModelCustomizationData source, TransitionModelCustomizationData target)
        {
            Migrate(source.KineticTransitionParameterSets, target.KineticTransitionParameterSets);
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="ICollection{T}" /> by values that also exist on the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(ICollection<KineticRuleSetData> source, ICollection<KineticRuleSetData> target)
        {
            source.Select(x => (SourceItem: x, TargetItem: target.FirstOrDefault(y => AreModelCompatible(x, y))))
                  .Where(pair => pair.TargetItem != null)
                  .Action(pair => Migrate(pair.SourceItem, pair.TargetItem))
                  .Load();
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="KineticRuleSetData" /> by values that also exist on the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(KineticRuleSetData source, KineticRuleSetData target)
        {
            var migrationReporter = GetMigrationReporter(source, target);
            MigrateName(source, target, migrationReporter);
            source.KineticRules.Select(x => (SourceItem: x, TargetItem: FindMigrationTarget(x, target.KineticRules)))
                  .Where(pair => pair.TargetItem != null)
                  .Action(pair => Migrate(pair.SourceItem, pair.TargetItem, migrationReporter))
                  .Load();
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="ICollection{T}" /> by values that also exist on the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(ICollection<PairEnergySetData> source, ICollection<PairEnergySetData> target)
        {
            source.Select(x => (SourceItem: x, TargetItem: target.FirstOrDefault(y => AreModelCompatible(x, y))))
                  .Where(pair => pair.TargetItem != null)
                  .Action(pair => Migrate(pair.SourceItem, pair.TargetItem))
                  .Load();
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="PairEnergySetData" /> by values that also exist on the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(PairEnergySetData source, PairEnergySetData target)
        {
            var migrationReporter = GetMigrationReporter(source, target);
            MigrateName(source, target, migrationReporter);
            source.PairEnergyEntries.Select(x => (SourceItem: x, TargetItem: FindMigrationTarget(x, target.PairEnergyEntries)))
                  .Where(pair => pair.TargetItem != null)
                  .Action(pair => Migrate(pair.SourceItem, pair.TargetItem, migrationReporter))
                  .Load();
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="ICollection{T}" /> by values that also exist on the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(ICollection<GroupEnergySetData> source, ICollection<GroupEnergySetData> target)
        {
            source.Select(x => (SourceItem: x, TargetItem: target.FirstOrDefault(y => AreModelCompatible(x, y))))
                  .Where(pair => pair.TargetItem != null)
                  .Action(pair => Migrate(pair.SourceItem, pair.TargetItem))
                  .Load();
        }


        /// <summary>
        ///     Overwrites all data of the target <see cref="GroupEnergySetData" /> by values that also exist on the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(GroupEnergySetData source, GroupEnergySetData target)
        {
            var migrationReporter = GetMigrationReporter(source, target);
            MigrateName(source, target, migrationReporter);
            source.EnergyEntries.Select(x => (SourceItem: x, TargetItem: FindMigrationTarget(x, target.EnergyEntries)))
                  .Where(pair => pair.TargetItem != null)
                  .Action(pair => Migrate(pair.SourceItem, pair.TargetItem, migrationReporter))
                  .Load();
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="PairEnergyData" /> by values that also exist on the source and reports
        ///     the action with the provided reporter delegate
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="reporter"></param>
        private void Migrate(PairEnergyData source, PairEnergyData target, DataMigrationReporter reporter)
        {
            if (!IsRedundantReportEnabled && VectorComparer.ValueComparer.Compare(source.Energy, target.Energy) == 0) return;

            var comment = $"Energy: \"{source.Energy}\", Name: \"{source.Name}\"";
            target.Energy = source.Energy;
            reporter.Invoke(source, target, comment);
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="GroupEnergyData" /> by values that also exist on the source and
        ///     reports the action with the provided reporter delegate
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="reporter"></param>
        private void Migrate(GroupEnergyData source, GroupEnergyData target, DataMigrationReporter reporter)
        {
            if (!IsRedundantReportEnabled && VectorComparer.ValueComparer.Compare(source.Energy, target.Energy) == 0) return;

            var comment = $"Energy: \"{source.Energy}\", Name: \"{source.Name}\"";
            target.Energy = source.Energy;
            reporter.Invoke(source, target, comment);
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="KineticRuleData" /> by values that also exist on the source and
        ///     reports the action with the provided reporter delegate
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="reporter"></param>
        private void Migrate(KineticRuleData source, KineticRuleData target, DataMigrationReporter reporter)
        {
            if (!IsRedundantReportEnabled && source.Name == target.Name &&
                VectorComparer.ValueComparer.Compare(source.AttemptFrequency, target.AttemptFrequency) == 0) return;

            var comment = $"Frequency: \"{source.AttemptFrequency}\", Name: \"{source.Name}\"";
            target.AttemptFrequency = source.AttemptFrequency;
            target.Name = source.Name;
            reporter.Invoke(source, target, comment);
        }

        /// <summary>
        ///     Overwrites the name of the source <see cref="ProjectDataObject" /> to the target and reports the action with the
        ///     provided delegate
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="reporter"></param>
        private void MigrateName(ProjectDataObject source, ProjectDataObject target, DataMigrationReporter reporter)
        {
            if (!IsRedundantReportEnabled && source.Name == target.Name) return;
            var comment = $"Name: \"{source.Name}\"";
            target.Name = source.Name;
            reporter.Invoke(source, target, comment);
        }

        /// <summary>
        ///     Searches for a matching <see cref="PairEnergyData" /> in the provided target <see cref="ICollection{T}" />
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private PairEnergyData FindMigrationTarget(PairEnergyData source, ICollection<PairEnergyData> targets)
        {
            return targets.FirstOrDefault(x => x.CenterParticle.Equals(source.CenterParticle) && x.PartnerParticle.Equals(source.PartnerParticle));
        }

        /// <summary>
        ///     Searches for a matching <see cref="GroupEnergyData" /> in the provided target <see cref="ICollection{T}" />
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private GroupEnergyData FindMigrationTarget(GroupEnergyData source, ICollection<GroupEnergyData> targets)
        {
            return targets.FirstOrDefault(x => x.CenterParticle.Equals(source.CenterParticle) && x.OccupationState.HasEqualState(source.OccupationState));
        }

        /// <summary>
        ///     Searches for a matching <see cref="KineticRuleData" /> in the provided target <see cref="ICollection{T}" />
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private KineticRuleData FindMigrationTarget(KineticRuleData source, ICollection<KineticRuleData> targets)
        {
            return targets.FirstOrDefault(x => x.HasEqualStates(source));
        }

        /// <summary>
        ///     Basic check if two sets of <see cref="PairEnergySetData" /> contain compatible model data that can be migrated
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private bool AreModelCompatible(PairEnergySetData first, PairEnergySetData second)
        {
            if (VectorComparer.ValueComparer.Compare(first.Distance, second.Distance) != 0) return false;
            if (first.CenterPosition.CompareTo(second.CenterPosition) != 0) return false;
            if (first.PartnerPosition.CompareTo(second.PartnerPosition) != 0) return false;
            return first.AsVectorPath().Zip(second.AsVectorPath(), VectorComparer.Equals).All(x => x);
        }

        /// <summary>
        ///     Basic check if two sets of <see cref="GroupEnergySetData" /> contain compatible model data that can be migrated
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private bool AreModelCompatible(GroupEnergySetData first, GroupEnergySetData second)
        {
            return first.AsVectorPath(true).Zip(second.AsVectorPath(true), VectorComparer.Equals).All(x => x);
        }

        /// <summary>
        ///     Basic check if two sets of <see cref="KineticRuleSetData" /> contain compatible model data that can be migrated
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private bool AreModelCompatible(KineticRuleSetData first, KineticRuleSetData second) => first.Transition.Equals(second.Transition);
    }
}