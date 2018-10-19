namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Job info interop object. Boxes a marshal struct into a .NET object
    /// </summary>
    public class JobInfo : InteropObject<CJobInfo>
    {
        /// <inheritdoc />
        public JobInfo()
        {
        }

        /// <inheritdoc />
        public JobInfo(CJobInfo structure)
            : base(structure)
        {
        }
    }
}