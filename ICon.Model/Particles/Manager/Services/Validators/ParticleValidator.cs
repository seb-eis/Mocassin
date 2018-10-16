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
            AddStringPropertyValidations(obj, report);
            AddChargeValidation(obj, report);
            AddObjectUniquenessValidation(obj, report);
            return report;
        }

        /// <summary>
        /// Validates all string properties of the particle and adds the results to the validation report
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="report"></param>
        protected void AddStringPropertyValidations(IParticle particle, ValidationReport report)
        {
            if (!Settings.ParticleName.ParseValue(particle.Name, out var warnings))
            {
                report.AddWarnings(warnings);
            }

            if (!Settings.ParticleSymbol.ParseValue(particle.Symbol, out warnings))
            {
                report.AddWarnings(warnings);
            }
        }

        /// <summary>
        /// Validates if a particle fulfills the charge restrictions and adds the reult to the validation report
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="report"></param>
        protected void AddChargeValidation(IParticle particle, ValidationReport report)
        {
            if (Settings.ParticleCharge.ParseValue(particle.Charge, out var warnings) != 0)
            {
                report.AddWarnings(warnings);
            }
        }

        /// <summary>
        /// Validates that a free slot for the particle exists in the model data and adds the result to the report
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="report"></param>
        protected void AddIndexOutOfRangeValidation(ValidationReport report)
        {
            if (Settings.ParticleCount.ParseValue(DataReader.Access.GetValidParticleCount() + 1, out var warnings) != 0)
            {
                report.AddWarnings(warnings);
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
            foreach (var item in DataReader.Access.GetParticles())
            {
                if (item.IsDeprecated)
                {
                    continue;
                }
                if (particle.EqualsInModelProperties(item, ProjectServices.CommonNumerics.RangeComparer))
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
                if (item.Symbol == particle.Symbol)
                {
                    var detail0 = $"Particle symbol is identical to the particle with index ({item.Index})";
                    var detail1 = $"Transition rule generation will auto enforce matter conservation for this symbol";
                    report.AddWarning(ModelMessageSource.CreateImplicitDependencyWarning(this, detail0, detail1));
                }
            }
        }
    }
}
