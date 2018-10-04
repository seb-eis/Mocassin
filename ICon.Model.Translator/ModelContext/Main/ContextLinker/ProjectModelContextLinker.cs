using System.Threading.Tasks;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class ProjectModelContextLinker : IProjectModelContextLinker
    {
        /// <inheritdoc />
        public Task LinkContextComponents(IProjectModelContextBuilder contextBuilder)
        {
            return Task.CompletedTask;
        }
    }
}
