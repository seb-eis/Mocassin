using ICon.Framework.Operations;
using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for single conflict resolver logic implementations that handle a specific object change
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TDataObject"></typeparam>
    public abstract class ObjectConflictHandler<TObject, TDataObject> where TDataObject : ModelData
    {
        /// <summary>
        /// Resolves potential conflicts caused by the passed model objects
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        /// <returns></returns>
        public abstract ConflictReport Resolve(TObject obj, IDataAccessor<TDataObject> dataAccess, IProjectServices projectServices);
    }
}
