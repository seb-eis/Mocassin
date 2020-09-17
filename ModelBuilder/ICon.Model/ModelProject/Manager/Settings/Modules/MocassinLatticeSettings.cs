using System.Runtime.Serialization;
using Mocassin.Model.Lattices;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Settings data object for the energy managing module
    /// </summary>
    [DataContract, ModuleSettings(typeof(ILatticeManager))]
    public class MocassinLatticeSettings : MocassinModuleSettings
    {
        /// <inheritdoc />
        public override void InitAsDefault()
        {
        }
    }
}