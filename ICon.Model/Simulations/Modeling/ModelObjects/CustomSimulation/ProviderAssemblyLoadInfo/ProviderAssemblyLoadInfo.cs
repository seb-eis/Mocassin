using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Defines a string based provider assembly load info that enables an outside dll to be used as a source for specific values
    /// </summary>
    [DataContract]
    public class ProviderAssemblyLoadInfo : ModelObject
    {
        /// <summary>
        /// The path to the DLL that should be loaded for provision
        /// </summary>
        [DataMember]
        public string DllFilePath { get; set; }

        /// <summary>
        /// The name of the provider calss that will be created for provision
        /// </summary>
        [DataMember]
        public string ProviderClassName { get; set; }

        /// <summary>
        /// The name of the method on the provider that should be used to create a provder delegate
        /// </summary>
        [DataMember]
        public string ProviderMethodName { get; set; }

        /// <summary>
        /// Get a string representing the model object name
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Background Creator Dll Load Info'";
        }

        /// <summary>
        /// Populates this object from a model object interafec and returns this object. Retruns null if the population operation failed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            return null;
        }
    }
}
