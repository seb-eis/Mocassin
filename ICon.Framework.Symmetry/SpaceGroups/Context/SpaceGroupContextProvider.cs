using Mocassin.Framework.SQLiteCore;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Provider for the space group database context
    /// </summary>
    public class SpaceGroupContextProvider : SqLiteContextProvider<SpaceGroupContext>
    {
        /// <inheritdoc />
        public override string DefaultFilepath { get; } =
            "C:/Users/hims-user/source/repos/ICon.Project/ICon.Framework.Symmetry/SpaceGroups/SpaceGroups.db";

        /// <inheritdoc />
        public SpaceGroupContextProvider()
        {
        }

        /// <inheritdoc />
        public SpaceGroupContextProvider(string filepath)
            : base(filepath)
        {
        }

        /// <summary>
        ///     Factory method to create a new space group context
        /// </summary>
        /// <returns></returns>
        public SpaceGroupContext NewContext()
        {
            return new SpaceGroupContext("Filename=" + DefaultFilepath);
        }
    }
}