using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;

using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Represents a push notification port for the particle manager that provides update events about the current state of the particle manager model data
    /// </summary>
    public interface IParticleEventPort : IModelEventPort
    {

    }
}
