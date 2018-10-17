using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Mocassin.Framework.Collections;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    /// Describes a physical movement during a transition as a set of integer values
    /// </summary>
    [DataContract]
    public class MovementCode : ArrayCode<int>
    {
        /// <summary>
        /// Returns the movement code as an exchange pair instruction
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(int, int)> AsExchangePairs()
        {
            for (int i = 0; i < CodeValues.Length - 2; i = i + 2)
            {
                yield return (CodeValues[i], CodeValues[i + 1]);
            }
        }

        /// <summary>
        /// Get the type name of the object
        /// </summary>
        /// <returns></returns>
        public override string GetTypeName()
        {
            return "'Movement Code'";
        }
    }
}
