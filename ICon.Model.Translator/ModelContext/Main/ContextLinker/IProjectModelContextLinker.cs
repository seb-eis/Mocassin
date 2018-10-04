using System.Threading.Tasks;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Model context linker that creates interconnection between raw build context elements
    /// </summary>
    public interface IProjectModelContextLinker
    {
        /// <summary>
        /// Awaits the build processes of the passed model context builder and links finished components
        /// </summary>
        /// <param name="contextBuilder"></param>
        /// <returns></returns>
        Task LinkContextComponents(IProjectModelContextBuilder contextBuilder);
    }
}