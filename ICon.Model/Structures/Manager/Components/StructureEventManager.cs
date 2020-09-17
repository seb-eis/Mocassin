using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Basic structure notification manager that handles distribution of push based update notifications about changes in
    ///     the structure manager base data
    /// </summary>
    internal class StructureEventManager : ModelEventManager, IStructureEventPort
    {
    }
}