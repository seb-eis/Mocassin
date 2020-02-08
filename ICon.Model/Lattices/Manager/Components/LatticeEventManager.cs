using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Basic lattice notification manager that handles distribution of push based update notifications about changes in
    ///     the lattice manager base data
    /// </summary>
    internal class LatticeEventManager : ModelEventManager, ILatticeEventPort
    {
    }
}