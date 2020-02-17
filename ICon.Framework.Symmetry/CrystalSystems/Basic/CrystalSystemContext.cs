using System;
using System.Collections.Generic;
using System.Linq;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Abstract crystal system context class that handles settings for crystal system creation
    /// </summary>
    public abstract class CrystalSystemContext
    {
        /// <summary>
        ///     Get the settings dictionary that maps <see cref="CrystalSystemIdentification" /> to
        ///     <see cref="CrystalSystemDefinition" /> instances
        /// </summary>
        protected abstract Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> SettingsDictionary { get; }

        /// <summary>
        ///     Tries to find a <see cref="CrystalSystemDefinition" /> by a <see cref="CrystalSystemIdentification" />
        /// </summary>
        /// <param name="identification"></param>
        /// <param name="definition"></param>
        /// <returns></returns>
        public bool TryGetSetting(CrystalSystemIdentification identification, out CrystalSystemDefinition definition)
        {
            return SettingsDictionary.TryGetValue(identification, out definition);
        }

        /// <summary>
        ///     Searches the dictionary using the provided expression, returns null if nothing is found ore throws an exception if
        ///     more than one matches the condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public CrystalSystemDefinition FindSetting(Func<KeyValuePair<CrystalSystemIdentification, CrystalSystemDefinition>, bool> predicate)
        {
            return SettingsDictionary.Single(predicate).Value;
        }

        /// <summary>
        ///     Searches the dictionary using the provided expression, returns null if nothing is found ore throws an exception if
        ///     more than one matches the condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public KeyValuePair<CrystalSystemIdentification, CrystalSystemDefinition> FindSettingEntry(
            Func<KeyValuePair<CrystalSystemIdentification, CrystalSystemDefinition>, bool> predicate)
        {
            return SettingsDictionary.Single(predicate);
        }

        /// <summary>
        ///     Searches the dictionary using the provided expression, returns all matching the condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<CrystalSystemDefinition> FindSettings(Func<KeyValuePair<CrystalSystemIdentification, CrystalSystemDefinition>, bool> predicate)
        {
            return SettingsDictionary.Where(predicate).Select(keyPair => keyPair.Value);
        }

        /// <summary>
        ///     Searches the dictionary using the provided expression, returns all matching the condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<CrystalSystemIdentification, CrystalSystemDefinition>> FindSettingEntries(
            Func<KeyValuePair<CrystalSystemIdentification, CrystalSystemDefinition>, bool> predicate)
        {
            return SettingsDictionary.Where(predicate);
        }


        /// <summary>
        ///     Creates the static dictionary based upon the settings defined in the implementing class
        /// </summary>
        /// <returns></returns>
        public Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> CreateSystemDictionary()
        {
            var dictionary = new Dictionary<CrystalSystemIdentification, CrystalSystemDefinition>();

            AddTriclinicSettings(dictionary);
            AddMonoclinicSettings(dictionary);
            AddOrthorhombicSettings(dictionary);
            AddTetragonalSettings(dictionary);
            AddTrigonalSettings(dictionary);
            AddHexagonalSettings(dictionary);
            AddCubicSettings(dictionary);

            return dictionary;
        }

        /// <summary>
        ///     Creates a new soft crystal system context that allows low symmetry systems to be defined with higher symmetry
        ///     parameters
        /// </summary>
        /// <returns></returns>
        public static CrystalSystemContext CreateSoftContext()
        {
            return new SoftCrystalSystemContext();
        }

        /// <summary>
        ///     Adds all triclinic settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddTriclinicSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary);

        /// <summary>
        ///     Adds all monoclinic settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddMonoclinicSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary);

        /// <summary>
        ///     Adds all orthorhombric settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddOrthorhombicSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary);

        /// <summary>
        ///     Adds all tetragonal settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddTetragonalSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary);

        /// <summary>
        ///     Adds all trigonal settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddTrigonalSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary);

        /// <summary>
        ///     Adds all hexagonal settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddHexagonalSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary);

        /// <summary>
        ///     Adds all cubic settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddCubicSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary);

        /// <summary>
        ///     Factory function for a specific crystal system type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        protected CrystalSystem Create<T1>() where T1 : CrystalSystem, new()
        {
            return new T1();
        }
    }
}