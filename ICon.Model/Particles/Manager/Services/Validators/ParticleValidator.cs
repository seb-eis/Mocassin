using System;
using System.Text.RegularExpressions;

using ICon.Mathematics.Constraints;
using ICon.Mathematics.Comparers;
using ICon.Framework.Messaging;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Particles.Validators
{
    /// <summary>
    /// Validator for new particle model objects that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class ParticleValidator : DataValidator<IParticle, BasicParticleSettings, IParticleDataPort>
    {
        /// <summary>
        /// Creates new validator t´hat uses the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public ParticleValidator(IProjectServices projectServices, BasicParticleSettings settings, IDataReader<IParticleDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new particle object in term of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IParticle obj)
        {
            var report = new ValidationReport();
            AddIndexOutOfRangeValidation(report);
            AddNameValidation(obj, report);
            AddSymbolValidation(obj, report);
            AddChargeValidation(obj, report);
            AddObjectUniquenessValidation(obj, report);
            return report;
        }

        /// <summary>
        /// Validates if the naming of a particle is allowed and adds the results to the report
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="report"></param>
        protected void AddNameValidation(IParticle particle, ValidationReport report)
        {
            if (new Regex(Settings.NameRegex).IsMatch(particle.Name) == false)
            {
                var detail0 = $"Particle naming is restriced by the following regular expression: {Settings.NameRegex}";
                report.AddWarning(ModelMessages.CreateNamingViolationWarning(this, detail0));
            }
        }

        /// <summary>
        /// Validates if the symbol naming of a particle is allowed and adds the results to the validation report
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="report"></param>
        protected void AddSymbolValidation(IParticle particle, ValidationReport report)
        {
            if (new Regex(Settings.SymbolRegex).IsMatch(particle.Symbol) == false)
            {
                var detail0 = $"Particle symbol naming is restriced by the following regular expression: {Settings.SymbolRegex}";
                report.AddWarning(ModelMessages.CreateNamingViolationWarning(this, detail0));
            }
        }

        /// <summary>
        /// Validates if a particle fulfills the charge restrictions and adds the reult to the validation report
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="report"></param>
        protected void AddChargeValidation(IParticle particle, ValidationReport report)
        {
            var constraint = new DoubleConstraint(true, -Settings.ChargeLimit, Settings.ChargeLimit, true, DoubleComparer.CreateRanged(Settings.ChargeTolerance));
            if (constraint.IsValid(particle.Charge) == false)
            {
                var detail0 = $"Particle charge values are restricted to {constraint.ToString()} with a tolerance of {Settings.ChargeTolerance}";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail0));
            }
        }

        /// <summary>
        /// Validates that a free slot for the particle exists in the model data and adds the result to the report
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="report"></param>
        protected void AddIndexOutOfRangeValidation(ValidationReport report)
        {
            if (DataReader.Access.GetValidParticleCount() >= Settings.ParticleLimit)
            {
                var detail0 = $"Particle manager limit for unique particles is ({Settings.ParticleLimit}) due to encoding for the simulation";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail0));
            }
        }

        /// <summary>
        /// Validates the uniqueness of the particle in the context of existing data (Ignores deprecated data objects in the existing data) and adds the result to the validation report
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="dataReader"></param>
        /// <param name="report"></param>
        protected void AddObjectUniquenessValidation(IParticle particle, ValidationReport report)
        {
            DoubleComparer comparer = DoubleComparer.CreateRanged(Settings.ChargeTolerance);
            foreach (var item in DataReader.Access.GetParticles())
            {
                if (item.IsDeprecated)
                {
                    continue;
                }
                if (particle.EqualsInModelProperties(item, comparer))
                {
                    var detail0 = $"Particle compares equal in properties to existing particle with index ({item.Index})";
                    report.AddWarning(ModelMessages.CreateModelDuplicateWarning(this, detail0));
                    break;
                }
                if (item.Name == particle.Name)
                {
                    var detail0 = $"Particle naming is identical to the particle with index ({item.Index})";
                    report.AddWarning(ModelMessages.CreateRedundantContentWarning(this, detail0));
                }
                if (item.Symbol == particle.Symbol)
                {
                    var detail0 = $"Particle symbol is identical to the particle with index ({item.Index})";
                    report.AddWarning(ModelMessages.CreateRedundantContentWarning(this, detail0));
                }
            }
        }
    }
}
