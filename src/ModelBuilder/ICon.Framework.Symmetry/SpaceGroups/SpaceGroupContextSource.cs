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
        /// <inheritdoc />
        public override string DefaultFilepath { get; } = MakeDefaultFilepath();

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
            return filepath;
        }
    }
}