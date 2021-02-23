using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Mocassin.Framework.SQLiteCore;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Provider for the space group database context
    /// </summary>
    public class SpaceGroupContextSource : SqLiteContextSource<SpaceGroupContext>
    {
        /// <summary>
        ///     Sets the global default database path
        /// </summary>
        public static string DefaultDbPath { get; set; } = MakeDefaultFilepath();

        /// <inheritdoc />
        public override string DefaultFilepath { get; } = DefaultDbPath;

        /// <inheritdoc />
        public SpaceGroupContextSource(string filepath)
            : base(filepath)
        {
        }

        /// <summary>
        ///     Creates the default filepath where to look for the symmetry database
        /// </summary>
        /// <returns></returns>
        public static string MakeDefaultFilepath()
        {
            var assemblyDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            var filepath = assemblyDirectory + "/Data/Mocassin.Symmetry.db";
            if (File.Exists(filepath)) return filepath;

            // Try to relay based on nuget package structure
            var packageDirectory = assemblyDirectory!.Parent!.Parent;
            filepath = packageDirectory + "/contentFiles/any/any/Data/Mocassin.Symmetry.db";
            if (File.Exists(filepath)) return filepath;
            var message = $"No default symmetry db location is valid. The property '{nameof(SpaceGroupContextSource)}.${nameof(DefaultDbPath)}' should be be set manually.";
            Debug.Write(message);
            return null;
        }
    }
}