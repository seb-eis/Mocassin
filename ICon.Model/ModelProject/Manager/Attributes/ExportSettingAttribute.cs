using System;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Setting export class that enables to mark a property as an exported setting for a specific purpose or value
    ///     by an identifier
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExportSettingAttribute : Attribute
    {
        /// <summary>
        ///     The export name that can be used to import the setting
        /// </summary>
        public string ExportName { get; }

        /// <summary>
        ///     Create a new export setting attribute that uses the provided export name
        /// </summary>
        /// <param name="exportName"></param>
        public ExportSettingAttribute(string exportName)
        {
            ExportName = exportName;
        }
    }
}