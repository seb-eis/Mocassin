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
        ///     The <see cref="CrystalSystemContext" /> used by the source
        /// </summary>
        public CrystalSystemContext Context { get; set; }

        /// <inheritdoc />
        public double Tolerance { get; set; }

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
            Tolerance = toleranceRange;
            ParameterMaxValue = parameterMaxValue;
        }

        /// <inheritdoc />
        public CrystalSystem GetSystem(CrystalSystemIdentification identification)
        {
            return Create(pair => pair.Key.Equals(identification));
        }

        /// <inheritdoc />
        public CrystalSystem GetSystem(ISpaceGroup group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            return Create(pair => pair.Key.CrystalType.Equals(group.CrystalType) && pair.Key.CrystalVariation.Equals(group.CrystalVariation));
        }

        /// <summary>
        ///     Creates a new crystal system with default parameters using the settings search expression (throws if more than one
        ///     settings matches the condition)
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public CrystalSystem Create(Func<KeyValuePair<CrystalSystemIdentification, CrystalSystemDefinition>, bool> where)
        {
            var settings = Context.FindSettingEntry(where);
            var system = settings.Value.Factory();
            settings.Value.ApplySettings(system, Tolerance, ParameterMaxValue);
            if (!system.TrySetParameterValues(system.GetDefaultParameterSet()))
                throw new InvalidOperationException("Basic default parameter set not compatible with own constraints");

            return system;
        }

        /// <summary>
        ///     Factory to create a new crystal system provider for soft systems with the specified max parameter and tolerance
        ///     value
        /// </summary>
        /// <param name="parameterMaxValue"></param>
        /// <param name="toleranceRange"></param>
        /// <returns></returns>
        public static CrystalSystemSource CreateSoft(double parameterMaxValue, double toleranceRange) =>
            new CrystalSystemSource(CrystalSystemContext.CreateSoftContext(), parameterMaxValue, toleranceRange);
    }
}