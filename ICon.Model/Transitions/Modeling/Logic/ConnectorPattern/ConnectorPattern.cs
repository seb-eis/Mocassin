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

    public enum ConnectorPatternType : int
    {
        Undefined, BasicKinetic, Metropolis, SingleVehicle, SplittedVehicle
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
        /// The name of the connector pattern
        /// </summary>
        public ConnectorPatternType PatternType { get; set; }

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
        public static string MakeConnectorCode(IEnumerable<ConnectorType> connectors)
        {
            var builder = new StringBuilder(10);
            foreach (var item in connectors)
            {
                builder.Append($"{item}{CodeSeparator}");
            }
            builder.PopBack(1);

            return builder.ToString();
        }

        /// <summary>
        /// Get the connector pattern for the bais kinetic transitions (D-{D}_1+)
        /// </summary>
        /// <returns></returns>
        public static ConnectorPattern GetBasicKineticPattern()
        {
            return new ConnectorPattern()
            {
                PatternRegex = new Regex($"^({ConnectorType.Dynamic}){{1}}({CodeSeparator}{ConnectorType.Dynamic}){{1,}}$"),
                PatternType = ConnectorPatternType.BasicKinetic
            };
        }

        /// <summary>
        /// Get the connector pattern for a metropolis transition (D)
        /// </summary>
        /// <returns></returns>
        public static ConnectorPattern GetMetropolisPattern()
        {
            return new ConnectorPattern()
            {
                PatternRegex = new Regex($"^({ConnectorType.Dynamic}){{1}}$"),
                PatternType = ConnectorPatternType.Metropolis
            };
        }

        /// <summary>
        /// Get the connector pattern for a splitted vehicle mechansim with multiple transition positions (D-D{-S-D-D}_1+)
        /// </summary>
        /// <returns></returns>
        public static ConnectorPattern GetSplittedVehiclePattern()
        {
            var head = $"{ConnectorType.Dynamic}{CodeSeparator}{ConnectorType.Dynamic}";
            var periodicBody = $"{CodeSeparator}{ConnectorType.Static}{CodeSeparator}{ConnectorType.Dynamic}{CodeSeparator}{ConnectorType.Dynamic}";
            return new ConnectorPattern()
            {
                PatternRegex = new Regex($"^({head}){{1}}({periodicBody}){{1,}}$"),
                PatternType = ConnectorPatternType.SplittedVehicle
            };
        }

        /// <summary>
        /// Get the connector pattern for a basic vehicle mechansim with one transition position ({D}_1+-S-S-{D}_1+))
        /// </summary>
        /// <returns></returns>
        public static ConnectorPattern GetBasicVehiclePattern()
        {
            var headOrTail = ConnectorType.Dynamic;
            var body = $"{ConnectorType.Static}{CodeSeparator}{ConnectorType.Static}";
            return new ConnectorPattern()
            {
                PatternRegex = new Regex($"^({headOrTail}{CodeSeparator}){{1,}}({body}){{1}}({CodeSeparator}{headOrTail}){{1,}}$"),
                PatternType = ConnectorPatternType.SingleVehicle
            };
        }

        /// <summary>
        ///  Get all curently supported transition connector patterns
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ConnectorPattern> GetAllSupportedPatterns()
        {
            yield return GetMetropolisPattern();
            yield return GetBasicKineticPattern();
            yield return GetBasicVehiclePattern();
            yield return GetSplittedVehiclePattern();
        }

        /// <summary>
        /// Determines the type of the pattern. If the pattern is not recognised the functions returns the undefined flag
        /// </summary>
        /// <param name="connectors"></param>
        /// <returns></returns>
        public static ConnectorPatternType DeterminePatternType(IEnumerable<ConnectorType> connectors)
        {
            var code = MakeConnectorCode(connectors);
            foreach (var pattern in GetAllSupportedPatterns())
            {
                if (pattern.PatternRegex.IsMatch(code))
                {
                    return pattern.PatternType;
                }
            }
            return ConnectorPatternType.Undefined;
        }
    }
}
