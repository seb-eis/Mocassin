using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
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
