using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Represents a translation unit for high level structure model data into encoded low level simulation data structures used by the C simulator
    /// </summary>
    public interface IStructureTranslator
    {
        IList<StructureModel> CreateStructureModels(ITranslationContext translationContext);
    }
}
