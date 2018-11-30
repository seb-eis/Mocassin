using System;

namespace Mocassin.Model.Mml.Descriptions
{
    /// <summary>
    ///     Attribute to mark a method as a description source method for the description source pipeline system
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DescriptionCreatorMethodAttribute : Attribute
    {
    }
}