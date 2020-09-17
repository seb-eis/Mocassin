namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Enumerates the possible SQLite db journal modes
    /// </summary>
    public enum DbJournalMode
    {
        /// <summary>
        ///     Delete mode
        /// </summary>
        Delete,

        /// <summary>
        ///     Truncate Mode
        /// </summary>
        Truncate,

        /// <summary>
        ///     Persist mode
        /// </summary>
        Persist,

        /// <summary>
        ///     Memory mode
        /// </summary>
        Memory,

        /// <summary>
        ///     Wal mode
        /// </summary>
        Wal,

        /// <summary>
        ///     No journal
        /// </summary>
        Off
    }
}