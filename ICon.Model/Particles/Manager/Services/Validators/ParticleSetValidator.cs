using System;
using Mocassin.Framework.Messaging;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Particles.Validators
{
    /// <summary>
    ///     Validator for new particle set model objects that checks for compatibility with existing data and general object
    ///     constraints
    /// </summary>
    public class ParticleSetValidator : DataValidator<IParticleSet, MocassinParticleSettings, IParticleDataPort>
    {
        /// <summary>
        ///     Creates new validator t´hat uses the provided project services, settings object and data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public ParticleSetValidator(IModelProject modelProject, MocassinParticleSettings settings,
            IDataReader<IParticleDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <inheritdoc />
        public override IValidationReport Validate(IParticleSet obj)
        {
            var report = new ValidationReport();
            AddIndexOutOfRangeValidation(report);
            AddOccupantsValidation(obj, report);
            AddObjectUniquenessValidation(obj, report);
            return report;
        }

        /// <summary>
        ///     Validates that a free slot for the particle set exists in the model data and adds the result to the validation
        ///     report
        /// </summary>
        /// <param name="result"></param>
        protected void AddIndexOutOfRangeValidation(ValidationReport result)
        {
            if (Settings.ParticleSetCount.ParseValue(DataReader.Access.GetValidParticleSetCount(), out var warnings) != 0)
                result.AddWarnings(warnings);
        }

        /// <summary>
        ///     Validates the uniqueness of a particle set in the context of existing data (Ignores deprecated data objects in the
        ///     existing data) and adds the result to the validation report
        /// </summary>
        /// <param name="particleSet"></param>
        /// <param name="result"></param>
        protected void AddObjectUniquenessValidation(IParticleSet particleSet, ValidationReport result)
        {
            foreach (var item in DataReader.Access.GetParticleSets())
            {
                if (item.IsDeprecated) 
                    continue;

                if (!particleSet.EqualsInModelProperties(item)) 
                    continue;

                var warning = new WarningMessage(this, "Particle set duplicate detected") {IsCritical = true};
                warning.Details.Add($"Particle set compares equal in properties to existing particle set with index ({item.Index})");
                result.AddWarning(warning);
                break;
            }
        }

        /// <summary>
        ///     Validate the occupants of a particle set, this functions throws if deprecated entries are found or the set contains
        ///     particle indexes that do not exist and adds the result to the validation report
        /// </summary>
        /// <param name="particleSet"></param>
        /// <param name="result"></param>
        protected void AddOccupantsValidation(IParticleSet particleSet, ValidationReport result)
        {
            var encodedSet = particleSet.GetEncoded();
            if (encodedSet.Mask == 0)
            {
                const string detail = "Custom definition of the empty particle set is not allowed as is always exists";
                result.AddWarning(ModelMessageSource.CreateModelDuplicateWarning(this, detail));
            }

            if ((encodedSet.Mask & DataReader.Access.GetValidParticlesAsSet().GetEncoded().Mask) - encodedSet.Mask != 0)
                throw new ArgumentException("Particle mask contains deprecated or out of range particles", nameof(particleSet));
        }
    }
}