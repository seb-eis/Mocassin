using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

using ICon.Symmetry.SpaceGroups;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Crystal system provider to enable creation of crystal systems with equal tolerance and range properties
    /// </summary>
    public class CrystalSystemProvider : ICrystalSystemProvider
    {
        /// <summary>
        /// The cryystal system context to find cyrstal system settings
        /// </summary>
        public CrystalSystemContext Context { get; set; }

        /// <summary>
        /// The tolerance value for double camparisons within the crystal system
        /// </summary>
        public Double ToleranceRange { get; set; }

        /// <summary>
        /// The maximum value for the lattice parameters
        /// </summary>
        public Double ParameterMaxValue { get; set; }

        /// <summary>
        /// Creates new provider with the specified context, base double tolerance and max parameter value
        /// </summary>
        /// <param name="toleranceRange"></param>
        /// <param name="parameterMaxValue"></param>
        public CrystalSystemProvider(CrystalSystemContext context, Double parameterMaxValue, Double toleranceRange)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            ToleranceRange = toleranceRange;
            ParameterMaxValue = parameterMaxValue;
        }

        /// <summary>
        /// Creates a new crystal system with default parameters using the settings search expression (throws if more than one settings matches the condition)
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public CrystalSystem Create(Func<KeyValuePair<(Int32 SystemID, String VariationName), CrystalSystemSetting>, Boolean> @where)
        {
            var settings = Context.FindSettingEntry(@where);

            if (settings.Value == null)
            {
                throw new ArgumentException("The context search function did not yield any valid settings", nameof(@where));
            }

            CrystalSystem system = settings.Value.DefaultConstruct();
            settings.Value.ApplySettings(system, ToleranceRange, ParameterMaxValue);
            if (!system.TrySetParameters(system.GetDefaultParameterSet()))
            {
                throw new InvalidOperationException("Basic default parameter set not compatible with own constraints");
            }
            return system;
        }

        /// <summary>
        /// Creates a new crystal system by system index and variation name
        /// </summary>
        /// <param name="SystemIndex"></param>
        /// <param name="VariationName"></param>
        /// <returns></returns>
        public CrystalSystem Create(Int32 SystemIndex, String VariationName)
        {
            return Create(pair => pair.Key.SystemID == SystemIndex && pair.Key.VariationName == VariationName);
        }

        /// <summary>
        /// Creates the correct cyrstal system object for the provided space group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public CrystalSystem Create(ISpaceGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }
            return Create(pair => pair.Key.SystemID == group.CrystalSystemIndex && group.Specifier == pair.Key.VariationName);
        }

        /// <summary>
        /// Factory to create a new cyrstal system provider for soft systems with the specified max parameter and tolerance value
        /// </summary>
        /// <param name="parameterMaxValue"></param>
        /// <param name="toleranceRange"></param>
        /// <returns></returns>
        public static CrystalSystemProvider CreateSoft(Double parameterMaxValue, Double toleranceRange)
        {
            return new CrystalSystemProvider(CrystalSystemContext.CreateSoftContext(), parameterMaxValue, toleranceRange);
        }
    }
}
