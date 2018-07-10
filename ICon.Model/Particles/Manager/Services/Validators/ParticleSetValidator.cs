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
    /// Validator for new particle set model objects that cheks for compatibility with existing data and general object constraints
    /// </summary>
    public class ParticleSetValidator : DataValidator<IParticleSet, BasicParticleSettings, IParticleDataPort>
    {
        /// <summary>
        /// Creates new validator t´hat uses the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public ParticleSetValidator(IProjectServices projectServices, BasicParticleSettings settings, IDataReader<IParticleDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new particle set object in term of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IParticleSet obj)
        {
            var report = new ValidationReport();
            AddIndexOutOfRangeValidation(report);
            AddOccupantsValidation(obj, report);
            AddObjectUniquenessValidation(obj, report);
            return report;
        }
        /// <summary>
        /// Validates that a free slot for the particle set exists in the model data and adds the result to the validation report
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="result"></param>
        protected void AddIndexOutOfRangeValidation(ValidationReport result)
        {
            if (Settings.ParticleSetCount.ParseValue(DataReader.Access.GetValidParticleSetCount(), out var warnings) != 0)
            {             
                result.AddWarnings(warnings);
            }
        }

        /// <summary>
        /// Validates the uniqueness of a particle set in the context of existing data (Ignores deprecated data objects in the existing data) and adds the result to the validation report
        /// </summary>
        /// <param name="particleSet"></param>
        /// <param name="dataReader"></param>
        /// <param name="result"></param>
        protected void AddObjectUniquenessValidation(IParticleSet particleSet, ValidationReport result)
        {
            foreach (var item in DataReader.Access.GetParticleSets())
            {
                if (item.IsDeprecated)
                {
                    continue;
                }
                if (particleSet.EqualsInModelProperties(item))
                {
                    var warning = new WarningMessage(this, "Particle set duplicate detected") { IsCritical = true };
                    warning.Details.Add($"Particle set compares equal in properties to existing particle set with index ({item.Index})");
                    result.AddWarning(warning);
                    break;
                }
            }
        }

        /// <summary>
        /// Validate the occupants of a particle set, this functions throws if deprecated entries are found or the set contains particle indexes that do not exist and adds the result to the validation report
        /// </summary>
        /// <param name="particleSet"></param>
        /// <param name="result"></param>
        protected void AddOccupantsValidation(IParticleSet particleSet, ValidationReport result)
        {
            var encodedSet = particleSet.GetEncoded();
            if (encodedSet.Mask == 0)
            {
                var detail = $"Custom definition of the empty particle set is not allowed as is always exists";
                result.AddWarning(ModelMessages.CreateModelDuplicateWarning(this, detail));
            }
            if ((encodedSet.Mask & DataReader.Access.GetValidParticlesAsSet().GetEncoded().Mask) - encodedSet.Mask != 0)
            {
                throw new ArgumentException("Particle mask contains deprecated or out of range particles", nameof(particleSet));
            }
        }

    }
}
