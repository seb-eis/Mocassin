using Mocassin.Model.Basic;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Basic particle notification manager that provides push based notifications about changes in the particle manager
    ///     reference data
    /// </summary>
    internal class ParticleEventManager : ModelEventManager, IParticleEventPort
    {
    }
}