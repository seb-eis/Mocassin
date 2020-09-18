using Mocassin.Model.Basic;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Represents a particle manager that handles input, output and distribution of model particles/species inside a
    ///     simulation project
    /// </summary>
    public interface IParticleManager : IModelManager<IParticleInputPort, IParticleEventPort, IParticleQueryPort>
    {
    }
}