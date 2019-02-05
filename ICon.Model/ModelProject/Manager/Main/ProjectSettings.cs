using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Data class for the model settings that stores general settings required throughout the entire model process (e.g.
    ///     floating point tolerances)
    /// </summary>
    [DataContract]
    public class ProjectSettings
    {
        /// <summary>
        /// The module settings collection
        /// </summary>
        [DataMember]
        private HashSet<MocassinModuleSettings> ModuleSettings { get; }

        /// <summary>
        ///     The numeric settings for common calculations and comparisons
        /// </summary>
        [DataMember]
        public MocassinNumericSettings CommonNumericSettings { get; set; }

        /// <summary>
        ///     The numeric settings for geometry calculations and comparisons
        /// </summary>
        [DataMember]
        public MocassinNumericSettings GeometryNumericSettings { get; set; }

        /// <summary>
        ///     The basic concurrency settings for timeout exceptions during parallel access to the model library
        /// </summary>
        [DataMember]
        public MocassinConcurrencySettings ConcurrencySettings { get; set; }

        /// <summary>
        ///     The basic constant settings that contain nature constants
        /// </summary>
        [DataMember]
        public MocassinConstantsSettings ConstantsSettings { get; set; }

        /// <summary>
        ///     The basic symmetry settings for space groups and crystal systems
        /// </summary>
        [DataMember]
        public MocassinSymmetrySettings SymmetrySettings { get; set; }

		/// <summary>
		///		The basic doping range tolerance
		/// </summary>
		public double DopingToleranceSetting { get; set; }

        /// <summary>
        /// Default construct project settings with empty module settings collection
        /// </summary>
        public ProjectSettings()
        {
            ModuleSettings = new HashSet<MocassinModuleSettings>();
        }

        /// <summary>
        /// Tries to get a module setting from the project settings collection
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <param name="settings"></param>
        /// <returns></returns>
        public bool TryGetModuleSettings<TSettings>(out TSettings settings)
            where TSettings : MocassinModuleSettings
        {
            settings = ModuleSettings.SingleOrDefault(a => a is TSettings) as TSettings;
            return settings != null;
        }

        /// <summary>
        /// Tries to get a module settings from the collection by module type
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public bool TryGetModuleSettings(Type moduleType, out MocassinModuleSettings settings)
        {
            settings = ModuleSettings.SingleOrDefault(a => a.IsValidForModule(moduleType));
            return settings != null;
        }

        /// <summary>
        /// Get the module settings of the specified type or null if they do not exist
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <returns></returns>
        public TSettings GetModuleSettings<TSettings>() where TSettings : MocassinModuleSettings
        {
            return ModuleSettings.SingleOrDefault(a => a is TSettings) as TSettings;
        }

        /// <summary>
        /// Looks-up all module settings objects that are marked with the module settings attribute in the passed assembly an adds one instance of each
        /// to the module settings collection
        /// </summary>
        /// <param name="sourceAssembly"></param>
        public void LookupAndAddModuleSettings(Assembly sourceAssembly)
        {
            if (sourceAssembly == null) 
                throw new ArgumentNullException(nameof(sourceAssembly));

            foreach (var type in sourceAssembly.ExportedTypes)
            {
                if (!typeof(MocassinModuleSettings).IsAssignableFrom(type) || type.GetCustomAttribute(typeof(ModuleSettingsAttribute)) == null) 
                    continue;

                var setting = (MocassinModuleSettings) Activator.CreateInstance(type);
                setting.InitAsDefault();
                ModuleSettings.Add(setting);
            }
        }

        /// <summary>
        ///     Creates a new default project services data object
        /// </summary>
        /// <returns></returns>
        public static ProjectSettings CreateDefault()
        {
            var settings = new ProjectSettings
            {
                CommonNumericSettings = new MocassinNumericSettings
                {
                    FactorValue = 1.0e-6,
                    RangeValue = 1.0e-10,
                    UlpValue = 10
                },
                GeometryNumericSettings = new MocassinNumericSettings
                {
                    FactorValue = 1.0e-2,
                    RangeValue = 1.0e-3,
                    UlpValue = 50
                },
                ConcurrencySettings = new MocassinConcurrencySettings
                {
                    AttemptInterval = TimeSpan.FromMilliseconds(100),
                    MaxAttempts = 20
                },
                ConstantsSettings = new MocassinConstantsSettings
                {
                    BoltzmannConstantSi = 1.38064852e-23,
                    UniversalGasConstantSi = 8.3144598,
                    VacuumPermittivitySi = 8.85418781762e-12,
                    ElementalChargeSi = 1.6021766208e-19
                },
                SymmetrySettings = new MocassinSymmetrySettings
                {
                    SpaceGroupDbPath = $"{Environment.GetEnvironmentVariable("USERPROFILE")}/source/repos/ICon.Program/ICon.Framework.Symmetry/SpaceGroups/SpaceGroups.db",
                    VectorTolerance = 1.0e-6,
                    ParameterTolerance = 1.0e-6
                },
	            DopingToleranceSetting = 1.0e-4
            };

            settings.LookupAndAddModuleSettings(Assembly.GetExecutingAssembly());
            return settings;
        }
    }
}