using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Represents a query port of the particle manager that provides safe access to data and internal logic
    /// </summary>
    public interface IParticleQueryPort : IModelQueryPort<IParticleDataPort>, IModelQueryPort<IParticleCachePort>
    {

    }
}
