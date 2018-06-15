using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Collections;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Abstract crystal system context class that handles settings for crystal system creation
    /// </summary>
    public abstract class CrystalSystemContext
    {
        /// <summary>
        /// Access to the settings dictionary defined by the implementing class
        /// </summary>
        protected abstract Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> SettingsDictionary { get; set; }

        /// <summary>
        /// Constructor, reconstructs the static dictionary on first call
        /// </summary>
        internal CrystalSystemContext()
        {
            if (SettingsDictionary == null)
            {
                SettingsDictionary = CreateDictionary();
            }
        }

        /// <summary>
        /// Tries to find the requested setting in the dictionary, returns false if not found
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="variationName"></param>
        /// <returns></returns>
        public Boolean TryGetSetting(Int32 systemID, String variation, out CrystalSystemSetting setting)
        {
            return SettingsDictionary.TryGetValue((systemID, variation), out setting);
        }

        /// <summary>
        /// Searches the dictionary using the provided expression, returns null if nothing is found ore throws an exception if more than one matches the condition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public CrystalSystemSetting FindSetting(Func<KeyValuePair<(Int32 SystemID, String VariationName), CrystalSystemSetting>, Boolean> @where)
        {
            return FindSettings(@where).SingleOrDefault();
        }

        /// <summary>
        /// Searches the dictionary using the provided expression, returns null if nothing is found ore throws an exception if more than one matches the condition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public KeyValuePair<(Int32 SystemID, String VariationName), CrystalSystemSetting> FindSettingEntry(Func<KeyValuePair<(Int32 SystemID, String VariationName), CrystalSystemSetting>, Boolean> @where)
        {
            return FindSettingEntries(@where).SingleOrDefault();
        }

        /// <summary>
        /// Searches the dictionary using the provided expression, returns all matching the condition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IEnumerable<CrystalSystemSetting> FindSettings(Func<KeyValuePair<(Int32 SystemID, String VariationName), CrystalSystemSetting>, Boolean> @where)
        {
            return SettingsDictionary.Where(keyPair => @where(keyPair)).Select(keyPair => keyPair.Value);
        }

        /// <summary>
        /// Searches the dictionary using the provided expression, returns all matching the condition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<(Int32, String), CrystalSystemSetting>> FindSettingEntries(Func<KeyValuePair<(Int32 SystemID, String VariationName), CrystalSystemSetting>, Boolean> @where)
        {
            return SettingsDictionary.Where(keyPair => where(keyPair));
        }


        /// <summary>
        /// Creates the static dictionary based upon the settings defined in the implementing class
        /// </summary>
        /// <returns></returns>
        public Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> CreateDictionary()
        {
            var dictionary = new Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting>(new TupleContentComparer<Int32, String>());

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
        /// Creates a new soft crystal system context that allows low symmetry systems to be defined with higher symmetry parameters
        /// </summary>
        /// <returns></returns>
        public static CrystalSystemContext CreateSoftContext()
        {
            return new SoftCrystalSystemContext();
        }

        /// <summary>
        /// Adds all triclinic settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddTriclinicSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        /// Adds all monoclinic settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddMonoclinicSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        /// Adds all orthorhombric settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddOrthorhombicSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        /// Adds all tetragonal settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddTetragonalSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        /// Adds all trigonal settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddTrigonalSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        /// Adds all hexagonal settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddHexagonalSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        /// Adds all cubic settings defined in the implementing context
        /// </summary>
        /// <param name="dictionary"></param>
        protected abstract void AddCubicSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary);

        /// <summary>
        /// Factory function for a specific crystal system type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        protected CrystalSystem Create<T1>() where T1 : CrystalSystem, new()
        {
            return new T1();
        }
    }
}
