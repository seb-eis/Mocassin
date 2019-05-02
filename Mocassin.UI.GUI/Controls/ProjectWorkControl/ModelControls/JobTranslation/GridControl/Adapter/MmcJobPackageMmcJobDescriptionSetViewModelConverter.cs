﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="IValueConverter" /> to wrap <see cref="MmcJobPackageDescriptionGraph" /> instances into
    ///     <see cref="MmcJobDescriptionSetControlViewModel" /> instances
    /// </summary>
    public class MmcJobPackageMmcJobDescriptionSetViewModelConverter : MarkupExtension, IValueConverter
    {
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MmcJobPackageDescriptionGraph jobPackage) return new MmcJobDescriptionSetControlViewModel(jobPackage);
            return null;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}