using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Represents a manager for model energies and replated modelling parameters and objects
    /// </summary>
    public interface ILatticeManager : IModelManager<ILatticeInputPort, ILatticeEventPort, ILatticeQueryPort>
    {
    }
}