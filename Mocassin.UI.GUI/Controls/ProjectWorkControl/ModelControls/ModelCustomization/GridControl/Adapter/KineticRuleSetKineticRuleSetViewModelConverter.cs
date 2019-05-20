using System;
using System.Globalization;
using System.Windows.Data;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="IValueConverter" /> implementation to wrap<see cref="KineticRuleSetGraph" /> instances into
    ///     <see cref="KineticRuleSetControlViewModel" /> instances
    /// </summary>
    public class KineticRuleSetKineticRuleSetViewModelConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is KineticRuleSetGraph ruleSet) return new KineticRuleSetControlViewModel(ruleSet);
            return value;
        }
    }
}