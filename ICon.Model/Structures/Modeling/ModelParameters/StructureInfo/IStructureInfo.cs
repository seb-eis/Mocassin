using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Interface for all structure info objects that contain used defined misc structure infos
    /// </summary>
    public interface IStructureInfo : IModelParameter
    {
        /// <summary>
        /// The name of the structure
        /// </summary>
        String Name { get; }
    }
}
