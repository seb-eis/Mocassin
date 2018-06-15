using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Enum that is used to specify the type of validation performed
    /// </summary>
    public enum ValidationType : int
    {
        Object, Parameter
    }

    /// <summary>
    /// Attribute to mark methods as validation methods for model data input
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidationMethodAttribute : Attribute
    {
        /// <summary>
        /// The type of validation the method is used for
        /// </summary>
        public ValidationType ValidationType { get; }

        /// <summary>
        /// Create new data validation method attribute with the specififed validation type
        /// </summary>
        /// <param name="validationType"></param>
        public ValidationMethodAttribute(ValidationType validationType)
        {
            ValidationType = validationType;
        }
    }
}
