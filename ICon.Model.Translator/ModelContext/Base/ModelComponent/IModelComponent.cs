using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a model component in the exended model context that can be indexed
    /// </summary>
    public interface IModelComponent
    {
        /// <summary>
        /// The index of the model component in the context
        /// </summary>
        int ModelId { get; set; }
    }
}
