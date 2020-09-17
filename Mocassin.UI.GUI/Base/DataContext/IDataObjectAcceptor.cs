using System.Windows;
using Mocassin.UI.Base.Commands;

namespace Mocassin.UI.GUI.Base.DataContext
{
    /// <summary>
    ///     General interface for acceptors of <see cref="IDataObject" /> through a <see cref="Command{T}" />
    /// </summary>
    public interface IDataObjectAcceptor
    {
        /// <summary>
        ///     Get the <see cref="Command{T}" /> that processes an incoming <see cref="IDataObject" />
        /// </summary>
        Command<IDataObject> ProcessDataObjectCommand { get; }
    }
}