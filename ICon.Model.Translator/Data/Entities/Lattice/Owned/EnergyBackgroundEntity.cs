namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Energy background alias class. Stores 5D energy background information for the simulation database
    /// </summary>
    public class EnergyBackgroundEntity : InteropBinaryArray<double>
    {
        /// <inheritdoc />
        public override string BlobTypeName => "LEB";
    }
}