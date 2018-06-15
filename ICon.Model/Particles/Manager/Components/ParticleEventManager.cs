using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using ICon.Framework.Events;

using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Basic particle notification manager that provides push based notifications about changes in the particle manager reference data
    /// </summary>
    internal class ParticleEventManager : ModelEventManager, IParticleEventPort
    {

    }
}
