using System;

namespace Mocassin.UI.Data.Base
{
    /// <summary>
    ///     Attribute to mark a property as a model input data root that provides a model parameter and object sequence
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ModelInputRootAttribute : Attribute
    {
        /// <summary>
        ///     Get the input order of the root
        /// </summary>
        public int Order { get; }

        /// <summary>
        ///     Create new attribute with defined input order
        /// </summary>
        /// <param name="order"></param>
        public ModelInputRootAttribute(int order)
        {
            Order = order;
        }
    }
}