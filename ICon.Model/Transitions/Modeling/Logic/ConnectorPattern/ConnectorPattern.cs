using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Enum for the connectors types
    /// </summary>
    public enum ConnectorType : byte
    {
        /// <summary>
        ///     The connection step is dynamic and describes an exchange step
        /// </summary>
        Dynamic,

        /// <summary>
        ///     The connection is static and describes a simple offset to the next start
        /// </summary>
        Static
    }

    /// <summary>
    ///     Enum for connector pattern types that are supported and recognized by the system
    /// </summary>
    public enum ConnectorPatternType
    {
        /// <summary>
        ///     Pattern is undefined or not supported
        /// </summary>
        Undefined,

        /// <summary>
        ///     Patterns follows basic kinetic transition definition or a basic chained kinetic type
        /// </summary>
        BasicKinetic,

        /// <summary>
        ///     Patterns follows basic metropolis transition definition
        /// </summary>
        Metropolis,

        /// <summary>
        ///     Pattern follows normal vehicle transition with one transition position
        /// </summary>
        NormalVehicle,

        /// <summary>
        ///     Patterns follows split vehicle transition with separated transition positions
        /// </summary>
        SplitTransitionVehicle
    }

    /// <summary>
    ///     Defines a pattern for the way transition connectors can be connected into transition sequences
    /// </summary>
    public class ConnectorPattern
    {
        /// <summary>
        ///     The separator for connector enums during code generation
        /// </summary>
        public static string CodeSeparator { get; } = "-";

        /// <summary>
        ///     The regex that describes the pattern of the connector sequence that is allowed
        /// </summary>
        public Regex PatternRegex { get; set; }

        /// <summary>
        ///     The name of the connector pattern
        /// </summary>
        public ConnectorPatternType PatternType { get; set; }

        /// <summary>
        ///     Checks if a sequence of connectors is valid in terms of the connector pattern
        /// </summary>
        /// <param name="connectors"></param>
        /// <returns></returns>
        public bool IsValid(IEnumerable<ConnectorType> connectors) => PatternRegex.IsMatch(MakeConnectorCode(connectors));

        /// <summary>
        ///     Get a literal name of a connector enum
        /// </summary>
        /// <param name="connector"></param>
        /// <returns></returns>
        public string GetName(ConnectorType connector) => Enum.GetName(typeof(ConnectorType), connector);

        /// <summary>
        ///     Creates a string code from a sequence of connectors that can be used for pattern matching
        /// </summary>
        /// <param name="connectors"></param>
        /// <returns></returns>
        public static string MakeConnectorCode(IEnumerable<ConnectorType> connectors)
        {
            var builder = new StringBuilder(10);
            foreach (var item in connectors)
                builder.Append($"{item}{CodeSeparator}");

            builder.PopBack(1);

            return builder.ToString();
        }

        /// <summary>
        ///     Get the connector pattern for the basic kinetic transitions (D-{D}_1+)
        /// </summary>
        /// <returns></returns>
        public static ConnectorPattern GetBasicKineticPattern() =>
            new ConnectorPattern
            {
                PatternRegex = new Regex($"^({ConnectorType.Dynamic}){{1}}({CodeSeparator}{ConnectorType.Dynamic}){{1,}}$"),
                PatternType = ConnectorPatternType.BasicKinetic
            };

        /// <summary>
        ///     Get the connector pattern for a metropolis transition (D)
        /// </summary>
        /// <returns></returns>
        public static ConnectorPattern GetMetropolisPattern() =>
            new ConnectorPattern
            {
                PatternRegex = new Regex($"^({ConnectorType.Dynamic}){{1}}$"),
                PatternType = ConnectorPatternType.Metropolis
            };

        /// <summary>
        ///     Get the connector pattern for a split vehicle mechanism with multiple transition positions (D-D{-S-D-D}_1+)
        /// </summary>
        /// <returns></returns>
        public static ConnectorPattern GetSplitVehiclePattern()
        {
            var head = $"{ConnectorType.Dynamic}{CodeSeparator}{ConnectorType.Dynamic}";
            var periodicBody =
                $"{CodeSeparator}{ConnectorType.Static}{CodeSeparator}{ConnectorType.Dynamic}{CodeSeparator}{ConnectorType.Dynamic}";
            return new ConnectorPattern
            {
                PatternRegex = new Regex($"^({head}){{1}}({periodicBody}){{1,}}$"),
                PatternType = ConnectorPatternType.SplitTransitionVehicle
            };
        }

        /// <summary>
        ///     Get the connector pattern for a basic vehicle mechanism with one transition position ({D}_1+-S-S-{D}_1+))
        /// </summary>
        /// <returns></returns>
        public static ConnectorPattern GetBasicVehiclePattern()
        {
            const ConnectorType headOrTail = ConnectorType.Dynamic;
            var body = $"{ConnectorType.Static}{CodeSeparator}{ConnectorType.Static}";
            return new ConnectorPattern
            {
                PatternRegex = new Regex($"^({headOrTail}{CodeSeparator}){{1,}}({body}){{1}}({CodeSeparator}{headOrTail}){{1,}}$"),
                PatternType = ConnectorPatternType.NormalVehicle
            };
        }

        /// <summary>
        ///     Get all currently supported transition connector patterns
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ConnectorPattern> GetAllSupportedPatterns()
        {
            yield return GetMetropolisPattern();
            yield return GetBasicKineticPattern();
            yield return GetBasicVehiclePattern();
            yield return GetSplitVehiclePattern();
        }

        /// <summary>
        ///     Determines the type of the pattern. If the pattern is not recognized the functions returns the undefined flag
        /// </summary>
        /// <param name="connectors"></param>
        /// <returns></returns>
        public static ConnectorPatternType DeterminePatternType(IEnumerable<ConnectorType> connectors)
        {
            var code = MakeConnectorCode(connectors);
            return (from pattern in GetAllSupportedPatterns()
                    where pattern.PatternRegex.IsMatch(code)
                    select pattern.PatternType).FirstOrDefault();
        }
    }
}