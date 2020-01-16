using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.Xml.Helper.Migration
{
    /// <summary>
    ///     Migration tool that partially recycles content of <see cref="ProjectCustomizationGraph" /> instances
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
        ///     Overwrites all data of the target <see cref="ProjectCustomizationGraph" /> by values that also exist on the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public MigrationReport Migrate(ProjectCustomizationGraph source, ProjectCustomizationGraph target)
        {
            Migrate(source.EnergyModelCustomization, target.EnergyModelCustomization);
            Migrate(source.TransitionModelCustomization, target.TransitionModelCustomization);
            return GenerateReport(source, target);
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="EnergyModelCustomizationGraph" /> by values that also exist on the
        ///     source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(EnergyModelCustomizationGraph source, EnergyModelCustomizationGraph target)
        {
            Migrate(source.StablePairEnergyParameterSets, target.StablePairEnergyParameterSets);
            Migrate(source.UnstablePairEnergyParameterSets, target.UnstablePairEnergyParameterSets);
            Migrate(source.GroupEnergyParameterSets, target.GroupEnergyParameterSets);
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="EnergyModelCustomizationGraph" /> by values that also exist on the
        ///     source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(TransitionModelCustomizationGraph source, TransitionModelCustomizationGraph target)
        {
            Migrate(source.KineticTransitionParameterSets, target.KineticTransitionParameterSets);
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="ICollection{T}" /> by values that also exist on the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(ICollection<KineticRuleSetGraph> source, ICollection<KineticRuleSetGraph> target)
        {
            source.Select(x => (SourceItem: x, TargetItem: target.FirstOrDefault(y => AreModelCompatible(x, y))))
                .Where(pair => pair.TargetItem != null)
                .Action(pair => Migrate(pair.SourceItem, pair.TargetItem))
                .Load();
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="KineticRuleSetGraph" /> by values that also exist on the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(KineticRuleSetGraph source, KineticRuleSetGraph target)
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
        private void Migrate(ICollection<PairEnergySetGraph> source, ICollection<PairEnergySetGraph> target)
        {
            source.Select(x => (SourceItem: x, TargetItem: target.FirstOrDefault(y => AreModelCompatible(x, y))))
                .Where(pair => pair.TargetItem != null)
                .Action(pair => Migrate(pair.SourceItem, pair.TargetItem))
                .Load();
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="PairEnergySetGraph" /> by values that also exist on the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(PairEnergySetGraph source, PairEnergySetGraph target)
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
        private void Migrate(ICollection<GroupEnergySetGraph> source, ICollection<GroupEnergySetGraph> target)
        {
           source.Select(x => (SourceItem: x, TargetItem: target.FirstOrDefault(y => AreModelCompatible(x, y))))
                .Where(pair => pair.TargetItem != null)
                .Action(pair => Migrate(pair.SourceItem, pair.TargetItem))
                .Load();
        }

        
        /// <summary>
        ///     Overwrites all data of the target <see cref="GroupEnergySetGraph" /> by values that also exist on the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Migrate(GroupEnergySetGraph source, GroupEnergySetGraph target)
        {
            var migrationReporter = GetMigrationReporter(source, target);
            MigrateName(source, target, migrationReporter);
            source.EnergyEntries.Select(x => (SourceItem: x, TargetItem: FindMigrationTarget(x, target.EnergyEntries)))
                .Where(pair => pair.TargetItem != null)
                .Action(pair => Migrate(pair.SourceItem, pair.TargetItem, migrationReporter))
                .Load();
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="PairEnergyGraph" /> by values that also exist on the source and reports the action with the provided reporter delegate
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="reporter"></param>
        private void Migrate(PairEnergyGraph source, PairEnergyGraph target, DataMigrationReporter reporter)
        {
            if (!IsRedundantReportEnabled && source.Name == target.Name && VectorComparer.ValueComparer.Compare(source.Energy, target.Energy) == 0) return;
            
            var comment = $"Energy: \"{source.Energy}\", Name: \"{source.Name}\"";
            target.Energy = source.Energy;
            target.Name = source.Name;
            reporter.Invoke(source, target, comment);
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="GroupEnergyGraph" /> by values that also exist on the source and reports the action with the provided reporter delegate
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="reporter"></param>
        private void Migrate(GroupEnergyGraph source, GroupEnergyGraph target, DataMigrationReporter reporter)
        {
            if (!IsRedundantReportEnabled && source.Name == target.Name && VectorComparer.ValueComparer.Compare(source.Energy, target.Energy) == 0) return;
            
            var comment = $"Energy: \"{source.Energy}\", Name: \"{source.Name}\"";
            target.Energy = source.Energy;
            target.Name = source.Name;
            reporter.Invoke(source, target, comment);
        }

        /// <summary>
        ///     Overwrites all data of the target <see cref="KineticRuleGraph" /> by values that also exist on the source and reports the action with the provided reporter delegate
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="reporter"></param>
        private void Migrate(KineticRuleGraph source, KineticRuleGraph target, DataMigrationReporter reporter)
        {
            if (!IsRedundantReportEnabled && source.Name == target.Name && VectorComparer.ValueComparer.Compare(source.AttemptFrequency, target.AttemptFrequency) == 0) return;
            
            var comment = $"Frequency: \"{source.AttemptFrequency}\", Name: \"{source.Name}\"";
            target.AttemptFrequency = source.AttemptFrequency;
            target.Name = source.Name;
            reporter.Invoke(source, target, comment);
        }

        /// <summary>
        ///     Overwrites the name of the source <see cref="ProjectObjectGraph"/> to the target and reports the action with the provided delegate
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="reporter"></param>
        private void MigrateName(ProjectObjectGraph source, ProjectObjectGraph target, DataMigrationReporter reporter)
        {
            if (!IsRedundantReportEnabled && source.Name == target.Name) return;
            var comment = $"Name: \"{source.Name}\"";
            target.Name = source.Name;
            reporter.Invoke(source, target, comment);
        }

        /// <summary>
        ///     Searches for a matching <see cref="PairEnergyGraph" /> in the provided target <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private PairEnergyGraph FindMigrationTarget(PairEnergyGraph source, ICollection<PairEnergyGraph> targets)
        {
            return targets.FirstOrDefault(x => x.CenterParticle.Equals(source.CenterParticle) && x.PartnerParticle.Equals(source.PartnerParticle));
        }

        /// <summary>
        ///     Searches for a matching <see cref="GroupEnergyGraph" /> in the provided target <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private GroupEnergyGraph FindMigrationTarget(GroupEnergyGraph source, ICollection<GroupEnergyGraph> targets)
        {
            return targets.FirstOrDefault(x => x.CenterParticle.Equals(source.CenterParticle) && x.OccupationState.HasEqualState(source.OccupationState));
        }

        /// <summary>
        ///     Searches for a matching <see cref="KineticRuleGraph" /> in the provided target <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private KineticRuleGraph FindMigrationTarget(KineticRuleGraph source, ICollection<KineticRuleGraph> targets)
        {
            return targets.FirstOrDefault(x => x.HasEqualStates(source));
        }

        /// <summary>
        ///     Basic check if two sets of <see cref="PairEnergySetGraph" /> contain compatible model data that can be migrated
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private bool AreModelCompatible(PairEnergySetGraph first, PairEnergySetGraph second)
        {
            if (VectorComparer.ValueComparer.Compare(first.Distance, second.Distance) != 0) return false;
            if (first.CenterPosition.CompareTo(second.CenterPosition) != 0) return false;
            if (first.PartnerPosition.CompareTo(second.PartnerPosition) != 0) return false;
            return first.AsVectorPath().Zip(second.AsVectorPath(), VectorComparer.Equals).All(x => x);
        }

        /// <summary>
        ///     Basic check if two sets of <see cref="GroupEnergySetGraph" /> contain compatible model data that can be migrated
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private bool AreModelCompatible(GroupEnergySetGraph first, GroupEnergySetGraph second)
        {
            return first.AsVectorPath(true).Zip(second.AsVectorPath(true), VectorComparer.Equals).All(x => x);
        }

        /// <summary>
        ///     Basic check if two sets of <see cref="KineticRuleSetGraph" /> contain compatible model data that can be migrated
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private bool AreModelCompatible(KineticRuleSetGraph first, KineticRuleSetGraph second)
        {
            return first.Transition.Equals(second.Transition);
        }
    }
}