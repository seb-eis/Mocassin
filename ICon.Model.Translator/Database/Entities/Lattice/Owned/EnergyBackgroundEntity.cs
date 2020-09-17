namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Energy background alias class. Stores 5D energy background information for the simulation database
    /// </summary>
    public class EnergyBackgroundEntity : InteropArray<double>
    {
        /// <summary>
        ///     The empty energy background property
        /// </summary>
        public static readonly EnergyBackgroundEntity Empty = new EnergyBackgroundEntity(new double[0, 0, 0, 0, 0]);

        /// <inheritdoc />
        public EnergyBackgroundEntity()
        {
        }

        /// <inheritdoc />
        public EnergyBackgroundEntity(double[,,,,] array)
            : base(array)
        {
        }
    }
}