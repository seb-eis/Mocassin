using Mocassin.Framework.SQLiteCore;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Provider for the space group database context
    /// </summary>
    public class SpaceGroupContextSource : SqLiteContextSource<SpaceGroupContext>
    {
        /// <inheritdoc />
        public override string DefaultFilepath { get; } = ".\\";

        /// <inheritdoc />
        public SpaceGroupContextSource(string filepath)
            : base(filepath)
        {
        }
    }
}