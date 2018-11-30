using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Operations;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Mml
{
    /// <inheritdoc />
    [ExportAdapter]
    public class EnergyContextAdapter : ReflectiveDataAdapter<MmlEnergySpecification, IEnergyModelContext>
    {
        /// <summary>
        ///     Get or set the pair data adapter that the context adapter should use
        /// </summary>
        [ImportAdapter(typeof(PairEnergyAdapter))]
        public IDataAdapter<MmlInteraction<MmlPairEnergy>, IPairEnergyModel> PairDataAdapter { get; set; }

        /// <summary>
        ///     Get or set the group data adapter that the context adapter should use
        /// </summary>
        [ImportAdapter(typeof(GroupEnergyAdapter))]
        public IDataAdapter<MmlInteraction<MmlGroupEnergy>, IGroupEnergyModel> GroupDataAdapter { get; set; }

        /// <summary>
        ///     Extract the misc information from the energy model context and stores it in the model specification
        /// </summary>
        /// <param name="energySpecification"></param>
        /// <param name="modelContext"></param>
        [ExtractionMethod]
        public void ExtractMiscInformation(MmlEnergySpecification energySpecification, IEnergyModelContext modelContext)
        {
            energySpecification.TargetContextHash = modelContext.GetHashCode();
        }

        /// <summary>
        ///     Extract the pair energy data from the energy model and stores it in the model specification
        /// </summary>
        /// <param name="energySpecification"></param>
        /// <param name="modelContext"></param>
        [ExtractionMethod]
        public void ExtractPairEnergyData(MmlEnergySpecification energySpecification, IEnergyModelContext modelContext)
        {
            var specifications = modelContext.PairEnergyModels
                .Select(model => PairDataAdapter.ExtractData(model));

            energySpecification.PairInteractionSpecifications = new HashSet<MmlInteraction<MmlPairEnergy>>(specifications);
        }

        /// <summary>
        ///     Extract the group energy data from the energy model and stores it in the model specification
        /// </summary>
        /// <param name="energySpecification"></param>
        /// <param name="modelContext"></param>
        [ExtractionMethod]
        public void ExtractGroupEnergyData(MmlEnergySpecification energySpecification, IEnergyModelContext modelContext)
        {
            var specifications = modelContext.GroupEnergyModels
                .Select(model => GroupDataAdapter.ExtractData(model));

            energySpecification.GroupInteractionSpecifications = new HashSet<MmlInteraction<MmlGroupEnergy>>(specifications);
        }

        /// <summary>
        ///     Injects the model specification pair energy information into the energy model context and writes the operation info
        ///     to the operation report
        /// </summary>
        /// <param name="energySpecification"></param>
        /// <param name="modelContext"></param>
        /// <param name="report"></param>
        [InjectionMethod]
        public void InjectPairEnergyData(MmlEnergySpecification energySpecification, IEnergyModelContext modelContext,
            IOperationReport report)
        {
            if (energySpecification.TargetContextHash != modelContext.GetHashCode())
            {
                report.AddException(new InvalidOperationException("Model specification cannot target passed energy context"));
                return;
            }

            try
            {
                foreach (var specification in energySpecification.PairInteractionSpecifications)
                {
                    var pairModel = modelContext.PairEnergyModels[specification.ModelContextId];
                    if (pairModel.ModelId != specification.ModelContextId)
                        pairModel = modelContext.PairEnergyModels.First(a => a.ModelId == specification.ModelContextId);

                    var localReport = PairDataAdapter.InjectData(specification, pairModel);
                    report.Merge(localReport);
                }
            }
            catch (Exception e)
            {
                report.AddException(e);
            }
        }

        /// <summary>
        ///     Injects the model specification pair energy information into the energy model context and writes the operation info
        ///     to the operation report
        /// </summary>
        /// <param name="energySpecification"></param>
        /// <param name="modelContext"></param>
        /// <param name="report"></param>
        [InjectionMethod]
        public void InjectGroupEnergyData(MmlEnergySpecification energySpecification, IEnergyModelContext modelContext,
            IOperationReport report)
        {
            if (energySpecification.TargetContextHash != modelContext.GetHashCode())
            {
                report.AddException(new InvalidOperationException("Model specification cannot target passed energy context"));
                return;
            }

            try
            {
                foreach (var specification in energySpecification.GroupInteractionSpecifications)
                {
                    var groupModel = modelContext.GroupEnergyModels[specification.ModelContextId];
                    if (groupModel.ModelId != specification.ModelContextId)
                        groupModel = modelContext.GroupEnergyModels.First(a => a.ModelId == specification.ModelContextId);

                    var localReport = GroupDataAdapter.InjectData(specification, groupModel);
                    report.Merge(localReport);
                }
            }
            catch (Exception e)
            {
                report.AddException(e);
            }
        }
    }
}