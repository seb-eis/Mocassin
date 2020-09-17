using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.ConflictHandling
{
    /// <summary>
    ///     Resolver provider for all structure conflict resolvers that handle internal data conflicts of the particle manager
    /// </summary>
    public class StructureDataConflictHandlerProvider : DataConflictHandlerProvider<StructureModelData>
    {
        /// <inheritdoc />
        public StructureDataConflictHandlerProvider(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <summary>
        ///     Creation method for the structure parameter change conflict handler
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.ParameterChange)]
        protected object CreateParameterHandler() => new StructureParameterConflictHandler(ModelProject);
    }
}