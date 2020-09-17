namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     The defect background table entity alias. Stores 2D lookup of unit cell defect energies
    /// </summary>
    public class DefectBackgroundEntity : InteropArray<double>
    {
        /// <inheritdoc />
        public DefectBackgroundEntity()
        {
        }

        /// <inheritdoc />
        public DefectBackgroundEntity(double[,] array)
            : base(array)
        {
        }
    }
}