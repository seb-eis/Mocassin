using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents an extended model component that supports sorting by indexing and provides information for simulation generation
    /// </summary>
    public interface IModelComponent
    {
        /// <summary>
        /// The extended model object id
        /// </summary>
        int ModelId { get; set; }
    }
}
