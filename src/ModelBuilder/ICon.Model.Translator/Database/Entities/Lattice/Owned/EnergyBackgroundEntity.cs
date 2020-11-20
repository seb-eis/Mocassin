namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Energy background alias class. Stores 5D energy background information for the simulation database
    /// </summary>
    public class EnergyBackgroundEntity : InteropArray<double>
    {
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