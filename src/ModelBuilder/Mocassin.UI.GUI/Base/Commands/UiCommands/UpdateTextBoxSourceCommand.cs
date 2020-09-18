using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Mocassin.UI.Base.Commands.UiCommands
{
    /// <summary>
    ///     The <see cref="Command{T}" /> to do an explicit update of the <see cref="TextBox" /> source property
    /// </summary>
    public class UpdateTextBoxSourceCommand : Command<TextBox>
    {
        /// <inheritdoc />
        public override void Execute(TextBox parameter)
        {
            if (parameter == null) return;
            BindingOperations.GetBindingExpression(parameter, TextBox.TextProperty)?.UpdateSource();
            Keyboard.ClearFocus();
        }
    }
}