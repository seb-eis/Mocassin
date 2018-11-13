namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Jump sequence entity alias class. Describes a set of 4D encoded crystal vectors that describe a jump geometry for
    ///     the
    ///     simulation database
    /// </summary>
    public class JumpSequenceEntity : InteropBinaryList<CVector4>
    {
    }
}