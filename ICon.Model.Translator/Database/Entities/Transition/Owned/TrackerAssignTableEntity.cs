namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Tracker assign table entity alias class. Defines a 2D mapping for tracker indexing
    /// </summary>
    public class TrackerAssignTableEntity : InteropArray<int>
    {
        /// <inheritdoc />
        public TrackerAssignTableEntity()
        {
        }

        /// <inheritdoc />
        public TrackerAssignTableEntity(int[,] array)
            : base(array)
        {
        }
    }
}