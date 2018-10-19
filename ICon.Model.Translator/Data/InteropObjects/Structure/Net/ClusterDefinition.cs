namespace Mocassin.Model.Translator.Data
{
    /// <summary>
    ///     Cluster definition interop object. Boxes a marshal struct into a .NET object
    /// </summary>
    public class ClusterDefinition : InteropObject<CClusterDefinition>
    {
        public ClusterDefinition()
        {
        }

        public ClusterDefinition(CClusterDefinition structure)
            : base(structure)
        {
        }
    }
}