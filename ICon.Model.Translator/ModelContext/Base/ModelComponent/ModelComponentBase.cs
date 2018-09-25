using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Abstract base class for model component implementations that form the model data context
    /// </summary>
    public abstract class ModelComponentBase : IModelComponent
    {
        /// <inheritdoc />
        public int ModelId { get; set; }
    }
}
