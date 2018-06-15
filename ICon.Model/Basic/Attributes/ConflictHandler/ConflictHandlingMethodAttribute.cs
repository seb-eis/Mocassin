using System;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Attribute that marks a method as conflict handling method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ConflictHandlingMethodAttribute : Attribute
    {
        /// <summary>
        /// Get the type of the conflict the method is supposed to handle
        /// </summary>
        public DataOperationType DataOperationType { get; }

        /// <summary>
        /// Create new conflict resolver method attribute with the specified type of conflict source
        /// </summary>
        /// <param name="conflictSource"></param>
        public ConflictHandlingMethodAttribute(DataOperationType conflictSource)
        {
            DataOperationType = conflictSource;
        }
    }
}
