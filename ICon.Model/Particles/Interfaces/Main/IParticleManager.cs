using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Represents a particle manager that handles input, output and distribution of model particles/species inside a simulation project
    /// </summary>
    public interface IParticleManager : IModelManager
    {
        /// <summary>
        /// The particle manager query port that offers particle related functionality
        /// </summary>
        IParticleQueryPort QueryPort { get; }

        /// <summary>
        /// The particle manager notification port that offers 'hot' notifications about changes in the manage data
        /// </summary>
        new IParticleEventPort EventPort { get; }
    }
}
