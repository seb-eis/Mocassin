using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Particles.Validators
{
    /// <summary>
    ///     Validator for new particle model objects that checks for compatibility with existing data and general object
    ///     constraints
    /// </summary>
    public class ParticleValidator : DataValidator<IParticle, BasicParticleSettings, IParticleDataPort>
    {
        /// <inheritdoc />
        public ParticleValidator(IProjectServices projectServices, BasicParticleSettings settings,
            IDataReader<IParticleDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(IParticle obj)
        {
            var report = new ValidationReport();
            AddIndexOutOfRangeValidation(report);
            AddStringPropertyValidations(obj, report);
            AddChargeValidation(obj, report);
            AddObjectUniquenessValidation(obj, report);
            return report;
        }

        /// <summary>
        ///     Validates all string properties of the particle and adds the results to the validation report
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="report"></param>
        protected void AddStringPropertyValidations(IParticle particle, ValidationReport report)
        {
            if (!Settings.ParticleName.ParseValue(particle.Name, out var warnings)) 
                report.AddWarnings(warnings);

            if (!Settings.ParticleSymbol.ParseValue(particle.Symbol, out warnings)) 
                report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates if a particle fulfills the charge restrictions and adds the reult to the validation report
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="report"></param>
        protected void AddChargeValidation(IParticle particle, ValidationReport report)
        {
            if (Settings.ParticleCharge.ParseValue(particle.Charge, out var warnings) != 0) 
                report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates that a free slot for the particle exists in the model data and adds the result to the report
        /// </summary>
        /// <param name="report"></param>
        protected void AddIndexOutOfRangeValidation(ValidationReport report)
        {
            if (Settings.ParticleCount.ParseValue(DataReader.Access.GetValidParticleCount() + 1, out var warnings) != 0)
                report.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates the uniqueness of the particle in the context of existing data (Ignores deprecated data objects in the
        ///     existing data) and adds the result to the validation report
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="report"></param>
        protected void AddObjectUniquenessValidation(IParticle particle, ValidationReport report)
        {
            foreach (var item in DataReader.Access.GetParticles())
            {
                if (item.IsDeprecated) 
                    continue;

                if (particle.EqualsInModelProperties(item, ProjectServices.CommonNumeric.RangeComparer))
                {
                    var detail0 = $"Particle compares equal in properties to existing particle with index ({item.Index})";
                    report.AddWarning(ModelMessageSource.CreateModelDuplicateWarning(this, detail0));
                    break;
                }

                if (item.Name == particle.Name)
                {
                    var detail0 = $"Particle naming is identical to the particle with index ({item.Index})";
                    report.AddWarning(ModelMessageSource.CreateRedundantContentWarning(this, detail0));
                }

                if (item.Symbol != particle.Symbol) 
                    continue;

                var detail1 = $"Particle symbol is identical to the particle with index ({item.Index})";
                const string detail2 = "Transition rule generation will auto enforce matter conservation for this symbol";
                report.AddWarning(ModelMessageSource.CreateImplicitDependencyWarning(this, detail1, detail2));
            }
        }
    }
}