using System.Windows;
using System.Windows.Input;
using Mocassin.UI.Base.Commands;

namespace Mocassin.UI.GUI.Base.DataContext
{
    /// <summary>
    ///     General interface for acceptors of dropped <see cref="IDataObject"/>
    /// </summary>
    public interface IObjectDropAcceptor
    {
        /// <summary>
        ///     Gte the <see cref="Command{T}"/> that handles the dropped <see cref="IDataObject"/>
        /// </summary>
        Command<IDataObject> HandleDropAddCommand { get; }
    }
}