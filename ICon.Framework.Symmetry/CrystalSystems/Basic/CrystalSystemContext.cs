using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Abstract crystal system context class that handles settings for crystal system creation
    /// </summary>
    public abstract class CrystalSystemContext
    {
        /// <summary>
        ///     Access to the settings dictionary defined by the implementing class
        /// </summary>
        protected abstract Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> SettingsDictionary { get; set; }

        /// <summary>
        ///     Constructor, reconstructs the static dictionary on first call
        /// </summary>
        internal CrystalSystemContext()
        {
            SettingsDictionary = SettingsDictionary ?? CreateDictionary();
        }

        /// <summary>
        ///     Tries to find the requested setting in the dictionary, returns false if not found
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="variation"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool TryGetSetting(int systemId, string variation, out CrystalSystemSetting setting)
        {
            return SettingsDictionary.TryGetValue((systemId, variation), out setting);
        }

        /// <summary>
        ///     Searches the dictionary using the provided expression, returns null if nothing is found ore throws an exception if
        ///     more than one matches the condition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public CrystalSystemSetting FindSetting(Func<KeyValuePair<(int SystemID, string VariationName), CrystalSystemSetting>, bool> where)
        {
            return FindSettings(where).SingleOrDefault();
        }

        /// <summary>
        ///     Searches the dictionary using the provided expression, returns null if nothing is found ore throws an exception if
        ///     more than one matches the condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public KeyValuePair<(int SystemID, string VariationName), CrystalSystemSetting> FindSettingEntry(
            Func<KeyValuePair<(int SystemID, string VariationName), CrystalSystemSetting>, bool> predicate)
        {
            return FindSettingEntries(predicate).SingleOrDefault();
        }

        /// <summary>
        ///     Searches the dictionary using the provided expression, returns all matching the condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<CrystalSystemSetting> FindSettings(
            Func<KeyValuePair<(int SystemID, string VariationName), CrystalSystemSetting>, bool> predicate)
        {
            return SettingsDictionary.Where(predicate).Select(keyPair => keyPair.Value);
        }

        /// <summary>
        ///     Searches the dictionary using the provided expression, returns all matching the condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<(int, string), CrystalSystemSetting>> FindSettingEntries(
            Func<KeyValuePair<(int SystemID, string VariationName), CrystalSystemSetting>, bool> predicate)
        {
            return SettingsDictionary.Where(predicate);
        }


        /// <summary>
        ///     Creates the static dictionary based upon the settings defined in the implementing class
        /// </summary>
        /// <returns></returns>
        public Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> CreateDictionary()
        {
            var dictionary = new Dictionary<(int SystemID, string VariationName), CrystalSystemSetting>(new TupleComparer<int, string>());

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
        protected abstract void AddTriclinicSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        ///     Adds all monoclinic settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddMonoclinicSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        ///     Adds all orthorhombric settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddOrthorhombicSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        ///     Adds all tetragonal settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddTetragonalSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        ///     Adds all trigonal settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddTrigonalSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        ///     Adds all hexagonal settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddHexagonalSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        ///     Adds all cubic settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddCubicSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary);

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