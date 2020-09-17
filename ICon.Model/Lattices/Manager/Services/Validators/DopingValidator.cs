using Mocassin.Framework.Messaging;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices.Validators
{
    /// <summary>
    ///     Validator for new Doping model objects that checks for consistency and compatibility with existing data and general
    ///     object constraints
    /// </summary>
    public class DopingValidator : DataValidator<IDoping, MocassinLatticeSettings, ILatticeDataPort>
    {
        /// <summary>
        ///     Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public DopingValidator(IModelProject projectServices, MocassinLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        ///     Validate a new Doping object in terms of consistency and compatibility with existing data
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IDoping obj)
        {
            var report = new ValidationReport();
            if (obj.UseCounterDoping) AddCounterChargeValidation(obj.PrimaryDoping, obj.CounterDoping, report);
            return report;
        }

        /// <summary>
        ///     Validate if a <see cref="IDopingCombination" /> pair forms a valid charge balance pair
        /// </summary>
        /// <param name="counterDoping"></param>
        /// <param name="report"></param>
        /// <param name="primaryDoping"></param>
        protected void AddCounterChargeValidation(IDopingCombination primaryDoping, IDopingCombination counterDoping,
            ValidationReport report)
        {
            var deltaDopingCharge = primaryDoping.Dopant.Charge - primaryDoping.Dopable.Charge;
            var deltaCounterDopingCharge = counterDoping.Dopant.Charge - counterDoping.Dopable.Charge;
            if (!(deltaDopingCharge * deltaCounterDopingCharge > 0.0)) return;

            const string detail0 = "The charge created by the doping has to be compensated by the counter doping.";
            report.AddWarning(WarningMessage.CreateCritical(this, detail0));
        }
    }
}