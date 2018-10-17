using System.Runtime.Serialization;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Settings data object for the energy managing module
    /// </summary>
    [DataContract]
    public class MocassinLatticeSettings : MocassinModuleSettings
    {
        /// <inheritdoc />
        public override void InitAsDefault()
        {
            
        }
    }
}