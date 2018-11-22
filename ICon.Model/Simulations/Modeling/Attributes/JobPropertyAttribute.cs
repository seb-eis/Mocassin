using System;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Attribute class that marks a property as a job property that can be manipulated without invalidating the simulation
    ///     data context making it open for job series creation
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JobPropertyAttribute : Attribute
    {
        /// <summary>
        ///     The settings export name that is used to limit the value range of the property
        /// </summary>
        public string ImportSettings { get; set; }

        /// <summary>
        ///     The job property base name that is usually recommended to use
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Create new job property attribute with the passed base name and settings binding
        /// </summary>
        /// <param name="name"></param>
        public JobPropertyAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}