using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a transition mapping model without specializing the type of describe transition
    /// </summary>
    public interface ITransitionMappingModel
    {
        /// <summary>
        ///     Defines the start vector of the mapping in encoded 4D crystal coordinates
        /// </summary>
        CrystalVector4D StartVector4D { get; set; }
    }
}