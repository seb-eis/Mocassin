using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Abstract base class for module settings objects
    /// </summary>
    [DataContract]
    public abstract class MocassinModuleSettings
    {
        /// <summary>
        ///     The <see cref="StringSetting" /> for general naming
        /// </summary>
        [DataMember]
        public StringSetting Naming { get; set; } = new StringSetting("Name", "[ -~]{1,100}", false);

        /// <summary>
        ///     Checks if the module settings is a valid settings object for the passed module type
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public bool IsValidForModule(Type moduleType)
        {
            if (GetType().GetCustomAttribute(typeof(ModuleSettingsAttribute)) is ModuleSettingsAttribute attribute)
                return attribute.ModuleType == moduleType;

            return false;
        }

        /// <summary>
        ///     Initializes the module settings object to ist default parameter set
        /// </summary>
        public abstract void InitAsDefault();
    }
}