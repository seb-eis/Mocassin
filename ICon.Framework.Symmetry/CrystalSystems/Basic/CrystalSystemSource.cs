using System;
using System.Collections.Generic;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Crystal system provider to enable creation of crystal systems with equal tolerance and range properties
    /// </summary>
    public class CrystalSystemSource : ICrystalSystemSource
    {
        /// <summary>
        ///     The crystal system context to find crystal system settings
        /// </summary>
        public CrystalSystemContext Context { get; set; }

        /// <inheritdoc />
        public double ToleranceRange { get; set; }

        /// <inheritdoc />
        public double ParameterMaxValue { get; set; }

        /// <summary>
        ///     Creates new provider with the specified context, base double tolerance and max parameter value
        /// </summary>
        /// <param name="toleranceRange"></param>
        /// <param name="context"></param>
        /// <param name="parameterMaxValue"></param>
        public CrystalSystemSource(CrystalSystemContext context, double parameterMaxValue, double toleranceRange)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            ToleranceRange = toleranceRange;
            ParameterMaxValue = parameterMaxValue;
        }

        /// <summary>
        ///     Creates a new crystal system with default parameters using the settings search expression (throws if more than one
        ///     settings matches the condition)
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public CrystalSystem Create(Func<KeyValuePair<(int SystemID, string VariationName), CrystalSystemSetting>, bool> where)
        {
            var settings = Context.FindSettingEntry(where);

            if (settings.Value == null)
                throw new ArgumentException("The context search function did not yield any valid settings", nameof(where));

            var system = settings.Value.DefaultConstruct();
            settings.Value.ApplySettings(system, ToleranceRange, ParameterMaxValue);
            if (!system.TrySetParameters(system.GetDefaultParameterSet()))
                throw new InvalidOperationException("Basic default parameter set not compatible with own constraints");

            return system;
        }

        /// <inheritdoc />
        public CrystalSystem Create(int systemIndex, string variationName)
        {
            return Create(pair => pair.Key.SystemID == systemIndex && pair.Key.VariationName == variationName);
        }

        /// <inheritdoc />
        public CrystalSystem Create(ISpaceGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            return Create(pair => pair.Key.SystemID == group.CrystalSystemIndex && group.Specifier == pair.Key.VariationName);
        }

        /// <summary>
        ///     Factory to create a new crystal system provider for soft systems with the specified max parameter and tolerance
        ///     value
        /// </summary>
        /// <param name="parameterMaxValue"></param>
        /// <param name="toleranceRange"></param>
        /// <returns></returns>
        public static CrystalSystemSource CreateSoft(double parameterMaxValue, double toleranceRange)
        {
            return new CrystalSystemSource(CrystalSystemContext.CreateSoftContext(), parameterMaxValue, toleranceRange);
        }
    }
}