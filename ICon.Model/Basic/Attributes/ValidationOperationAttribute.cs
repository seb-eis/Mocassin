using System;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Enum that is used to specify the type of validation that is performed
    /// </summary>
    public enum ValidationType
    {
        /// <summary>
        ///     The validation handles an indexed model object
        /// </summary>
        Object,

        /// <summary>
        ///     The validation handles a unique model parameter
        /// </summary>
        Parameter
    }

    /// <summary>
    ///     Attribute to mark methods as validation methods for model data input
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidationOperationAttribute : Attribute
    {
        /// <summary>
        ///     The type of validation the method is used for
        /// </summary>
        public ValidationType ValidationType { get; }

        /// <summary>
        ///     Create new data validation method attribute with the specified validation type
        /// </summary>
        /// <param name="validationType"></param>
        public ValidationOperationAttribute(ValidationType validationType)
        {
            ValidationType = validationType;
        }
    }
}