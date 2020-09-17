using System;

namespace Mocassin.Framework.Exceptions
{
    /// <summary>
    ///     Exception thrown if a processing pipeline receives a non-processable type and no default handle case exists
    /// </summary>
    public class InvalidPipelineInputException : MocassinException
    {
        /// <summary>
        ///     The actual model object type that caused the exception
        /// </summary>
        public Type InputType { get; }

        /// <inheritdoc />
        public InvalidPipelineInputException(string message, Type inputType)
            : base(message)
        {
            InputType = inputType;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"Model input manager was provided with an unexpected type{Environment.NewLine} Type: {InputType}{Environment.NewLine} Details:{Environment.NewLine}{Message}";
    }
}