using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles.Validators;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Particle validation service that provide validations for new particle related data
    /// </summary>
    public class ParticleValidationService : ValidationService<IParticleDataPort>
    {
        /// <summary>
        ///     The particle settings used for validations
        /// </summary>
        protected MocassinParticleSettings Settings { get; }

        /// <inheritdoc />
        public ParticleValidationService(MocassinParticleSettings particleSettings, IModelProject modelProject)
            : base(modelProject)
        {
            Settings = particleSettings ?? throw new ArgumentNullException(nameof(particleSettings));
        }

        /// <summary>
        ///     Validates a particle interface and checks if the contents are not a duplicate using the provided data reader
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateParticle(IParticle particle, IDataReader<IParticleDataPort> dataReader)
        {
            return new ParticleValidator(ModelProject, Settings, dataReader).Validate(particle);
        }

        /// <summary>
        ///     Validates a particle set interface and checks if the contents are not a duplicate using the provided data reader
        /// </summary>
        /// <param name="particleSet"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateParticleSet(IParticleSet particleSet, IDataReader<IParticleDataPort> dataReader)
        {
            return new ParticleSetValidator(ModelProject, Settings, dataReader).Validate(particleSet);
        }
    }
}