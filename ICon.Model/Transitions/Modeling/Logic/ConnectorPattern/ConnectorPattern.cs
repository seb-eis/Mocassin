using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

using ICon.Framework.Extensions;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Enum for the connectors ()
    /// </summary>
    public enum ConnectorType : byte
    {
        Dynamic, Static
    }

    /// <summary>
    /// Defines a pattern for the way transition connectors can be connected into transition sequences
    /// </summary>
    public class ConnectorPattern
    {
        /// <summary>
        /// The separator for connector enums during code generation
        /// </summary>
        public static string CodeSeparator { get; } = "-";

        /// <summary>
        /// The regex that describes the patttern of the connector sequence that is allowed
        /// </summary>
        public Regex PatternRegex { get; set; }

        /// <summary>
        /// Checks if a sequence of connectors is valid in terms of the connector pattern
        /// </summary>
        /// <param name="connectors"></param>
        /// <returns></returns>
        public bool IsValid(IEnumerable<ConnectorType> connectors)
        {
            return PatternRegex.IsMatch(MakeConnectorCode(connectors));
        }

        /// <summary>
        /// Get a literal name of a connector enum
        /// </summary>
        /// <param name="connector"></param>
        /// <returns></returns>
        public string GetName(ConnectorType connector)
        {
            return Enum.GetName(typeof(ConnectorType), connector);
        }

        /// <summary>
        /// Creates a string code froma sequence of connectors that can be used for pattern matching
        /// </summary>
        /// <param name="connectors"></param>
        /// <returns></returns>
        public string MakeConnectorCode(IEnumerable<ConnectorType> connectors)
        {
            var builder = new StringBuilder(10);
            foreach (var item in connectors)
            {
                builder.Append(item.ToString() + CodeSeparator);
            }
            builder.PopBack(1);

            return builder.ToString();
        }

        /// <summary>
        /// Get the connector pattern for the bais ckinetic transitions (Only kinetic connectors)
        /// </summary>
        /// <returns></returns>
        public static ConnectorPattern GetBasicKineticPattern()
        {
            return new ConnectorPattern()
            {
                PatternRegex = new Regex($"^({ConnectorType.Dynamic.ToString()}){{1}}({CodeSeparator + ConnectorType.Dynamic.ToString()}){{0,}}$")
            };
        }

        /// <summary>
        /// Get the connector pattern for a metropolis transition
        /// </summary>
        /// <returns></returns>
        public static ConnectorPattern GetMetropolisPattern()
        {
            return new ConnectorPattern()
            {
                PatternRegex = new Regex($"^({ConnectorType.Dynamic.ToString()}){{1}}$")
            };
        }

        /// <summary>
        /// Get the connector pattern for a vehicle mechansim
        /// </summary>
        /// <returns></returns>
        public static ConnectorPattern GetKineticVehiclePattern()
        {
            var start = ConnectorType.Dynamic.ToString();
            var periodic = ConnectorType.Static.ToString() + CodeSeparator + ConnectorType.Dynamic.ToString();
            return new ConnectorPattern()
            {
                PatternRegex = new Regex($"^({start}){{1}}({CodeSeparator + periodic}){{1,}}$")
            };
        }
    }
}
