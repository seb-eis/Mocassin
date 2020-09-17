using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Describes a physical movement during a transition as a set of integer values
    /// </summary>
    public class MovementCode : CodeArray<int>
    {
        /// <summary>
        ///     Returns the movement code as an exchange pair instruction
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(int, int)> AsExchangePairs()
        {
            for (var i = 0; i < CodeValues.Length - 1; i = i + 2)
                yield return (CodeValues[i], CodeValues[i + 1]);
        }

        /// <inheritdoc />
        public override string GetTypeName() => "Movement Code";

        /// <summary>
        ///     Get the inverted movement code
        /// </summary>
        /// <returns></returns>
        public MovementCode GetInverse() =>
            new MovementCode
            {
                CodeValues = CodeValues.Reverse().ToArray()
            };
    }
}