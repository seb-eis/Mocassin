using System;
using ICon.Framework.Operations;
using ICon.Model.ProjectServices;
using ICon.Model.Basic;
using ICon.Model.Particles.Validators;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Particle validation service that provide validations for new particle related data
    /// </summary>
    public class ParticleValidationService : ValidationService<IParticleDataPort>
    {
        /// <summary>
        /// The particle settings used for validations
        /// </summary>
        protected BasicParticleSettings Settings { get; }

        /// <summary>
        /// Creates new particle validation service from settings data
        /// </summary>
        /// <param name="particleSettings"></param>
        public ParticleValidationService(BasicParticleSettings particleSettings, IProjectServices projectServices) : base(projectServices)
        {
            Settings = particleSettings ?? throw new ArgumentNullException(nameof(particleSettings));
        }

        /// <summary>
        /// Validates a particle interface and checks if the contents are not a duplicate using the provided data reader
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateParticle(IParticle particle, IDataReader<IParticleDataPort> dataReader)
        {
            return new ParticleValidator(ProjectServices, Settings, dataReader).Validate(particle);
        }

        /// <summary>
        /// Validates a particle set interface and checks if the contents are not a duplicate using the provided data reader
        /// </summary>
        /// <param name="particleSet"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport ValidateParticleSet(IParticleSet particleSet, IDataReader<IParticleDataPort> dataReader)
        {
            return new ParticleSetValidator(ProjectServices, Settings, dataReader).Validate(particleSet);
        }
    }
}
