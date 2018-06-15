using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using ICon.Framework.Operations;
using ICon.Model.Basic;

namespace ICon.Model.Basic
{
    /// <summary>
    /// General base interface for all model object input port interfaces
    /// </summary>
    public interface IModelInputPort
    {
        /// <summary>
        /// Sets a unique model parameter of the manager if it passes validation
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        Task<IOperationReport> SetModelParameter<TParameter>(TParameter modelParameter) where TParameter : IModelParameter;

        /// <summary>
        /// Adds a new model object to the manager if the manager can cast the object to an accepted type and the new object passes validation
        /// </summary>
        /// <param name="particle"></param>
        /// <returns></returns>
        Task<IOperationReport> InputModelObject<TObject>(TObject modelObject) where TObject : IModelObject;

        /// <summary>
        /// ARemoves a model object from the manager if it supports the object by using the model object index
        /// </summary>
        /// <param name="particle"></param>
        /// <returns></returns>
        Task<IOperationReport> RemoveModelObject<TObject>(TObject modelObject) where TObject : IModelObject;

        /// <summary>
        /// Replaces a  new model object in the manager by a new one if the manager can cast the object to an accepted type and the new object passes validation
        /// </summary>
        /// <param name="particle"></param>
        /// <returns></returns>
        Task<IOperationReport> ReplaceModelObject<TObject>(TObject original, TObject replacement) where TObject : IModelObject;

        /// <summary>
        /// Clears all data of the manager. This action restores the mint condition where only 'Null-Particle' and 'Null-PartcleSet' set exist
        /// </summary>
        /// <returns></returns>
        Task<IOperationReport> ResetManager();

        /// <summary>
        /// Cleans the particle manager from deprecated data. This action causes a full ID reassignment and potentially causes a full recalculation of all model data
        /// </summary>
        /// <returns></returns>
        Task<IOperationReport> CleanupManager();

        /// <summary>
        /// Get an enumerable off all supported input model objects and parameters of this input manager
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetSupportedModelTypes();
    }
}
