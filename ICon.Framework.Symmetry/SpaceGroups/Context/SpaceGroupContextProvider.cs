using ICon.Framework.SQLiteCore;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Provider for the space group database context
    /// </summary>
    public class SpaceGroupContextProvider : SqLiteContextProvider<SpaceGroupContext>
    {
        /// <inheritdoc />
        public override string DefaultFilepath { get; } =
            "C:/Users/hims-user/source/repos/ICon.Project/ICon.Framework.Symmetry/SpaceGroups/SpaceGroups.db";

        /// <summary>
        ///     New context provider utilizing the default filepath
        /// </summary>
        public SpaceGroupContextProvider()
        {
        }

        /// <summary>
        ///     Creates a context provider with the specified filepath (Checks if filepath exists)
        /// </summary>
        /// <param name="filepath"></param>
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