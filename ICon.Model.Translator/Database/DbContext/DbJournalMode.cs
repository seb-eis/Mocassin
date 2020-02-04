namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Enumerates the possible SQLite db journal modes
    /// </summary>
    public enum DbJournalMode
    {
        Delete,
        Truncate,
        Persist,
        Memory,
        Wal,
        Off
    }
}