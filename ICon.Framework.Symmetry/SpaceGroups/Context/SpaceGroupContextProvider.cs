using Mocassin.Framework.SQLiteCore;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Provider for the space group database context
    /// </summary>
    public class SpaceGroupContextProvider : SqLiteContextProvider<SpaceGroupContext>
    {
        /// <inheritdoc />
        public override string DefaultFilepath { get; } = ".\\";

        /// <inheritdoc />
        public SpaceGroupContextProvider(string filepath)
            : base(filepath)
        {
        }
    }
}