using System;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Attribute that marks a class to be a project setting source for a specific module
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleSettingsAttribute : Attribute
    {
        /// <summary>
        ///     The type of the module interface the setting is valid for
        /// </summary>
        public Type ModuleType { get; }

        /// <summary>
        ///     Create new module settings attribute with the passed module type
        /// </summary>
        /// <param name="moduleType"></param>
        public ModuleSettingsAttribute(Type moduleType)
        {
            ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
        }
    }
}