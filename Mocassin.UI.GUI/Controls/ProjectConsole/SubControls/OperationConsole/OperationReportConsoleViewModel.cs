using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Mocassin.Framework.Operations;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="OperationReportConsoleView" /> that displays
    ///     <see cref="IOperationReport" /> collections
    /// </summary>
    public class OperationReportConsoleViewModel : PrimaryControlViewModel
    {
        private readonly object lockObject = new object();
        private bool isErrorContentFiltered;
        private bool isSoftUpdateStop;
        private bool isHardUpdateStop;
        private string reportSourceName;
        private IOperationReport selectedReport;

        /// <summary>
        ///     Get or set the last received <see cref="IEnumerable{T}"/> of <see cref="IOperationReport"/> instances
        /// </summary>
        private IEnumerable<IOperationReport> LastReportSet { get; set; }

        /// <summary>
        ///     The <see cref="IDisposable" /> for an active report subscription
        /// </summary>
        private IDisposable ReportSourceSubscription { get; set; }

        /// <summary>
        ///     The <see cref="ObservableCollectionViewModel{T}" /> for the set of <see cref="IOperationReport" />
        /// </summary>
        public ObservableCollectionViewModel<IOperationReport> OperationCollectionViewModel { get; }

        /// <summary>
        ///     Get or set the currently selected <see cref="IOperationReport"/>
        /// </summary>
        public IOperationReport SelectedReport
        {
            get => selectedReport;
            set => SetProperty(ref selectedReport, value);
        }

        /// <summary>
        ///     Get a set a boolean flag that controls if display requests will filter <see cref="IOperationReport" /> instances
        ///     that have nor relevant content
        /// </summary>
        public bool IsErrorContentFiltered
        {
            get => isErrorContentFiltered;
            set => SetProperty(ref isErrorContentFiltered, value);
        }

        /// <summary>
        ///     Get or set a boolean flag that defines if updating of received report lists is temporarily stopped
        /// </summary>
        public bool IsSoftUpdateStop
        {
            get => isSoftUpdateStop;
            set => SetProperty(ref isSoftUpdateStop, value);
        }

        /// <summary>
        ///     Get or set a boolean flag that defines if updating of received report lists is hard stopped
        /// </summary>
        public bool IsHardUpdateStop
        {
            get => isHardUpdateStop;
            set => SetProperty(ref isHardUpdateStop, value);
        }

        /// <summary>
        ///     Get or set a <see cref="string" /> name of the last report display request source
        /// </summary>
        public string ReportSourceName
        {
            get => reportSourceName ?? "Unknown";
            protected set => SetProperty(ref reportSourceName, value);
        }

        /// <summary>
        ///     Get the <see cref="SetSoftUpdateStopCommand"/> to set/unset the affiliated flag
        /// </summary>
        public SetSoftUpdateStopCommand SoftUpdateStopCommand { get; }

        /// <inheritdoc />
        public OperationReportConsoleViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            OperationCollectionViewModel = new ObservableCollectionViewModel<IOperationReport>();
            SoftUpdateStopCommand = new SetSoftUpdateStopCommand(this);
            PropertyChanged += OnFilterChanged;
        }

        /// <summary>
        ///     Requests the displaying of the passed <see cref="IEnumerable{T}" /> sequence of
        /// </summary>
        /// <param name="reports"></param>
        /// <param name="reportSource"></param>
        public void DisplayReports(IEnumerable<IOperationReport> reports, [CallerMemberName] string reportSource = default)
        {
            lock (lockObject)
            {
                if (isSoftUpdateStop || isHardUpdateStop) return;

                LastReportSet = reports;
                ReportSourceName = reportSource;
                OperationCollectionViewModel.ClearCollection();
                OperationCollectionViewModel.AddCollectionItems(GetFilteredReports(LastReportSet));
            }
        }

        /// <summary>
        ///     Filters the passed <see cref="IEnumerable{T}" /> of <see cref="IOperationReport" /> instances using the internal
        ///     filter settings
        /// </summary>
        /// <param name="reports"></param>
        /// <returns></returns>
        public IEnumerable<IOperationReport> GetFilteredReports(IEnumerable<IOperationReport> reports)
        {
            return !IsErrorContentFiltered ? reports : reports?.Where(x => !x.IsGood);
        }

        /// <summary>
        ///     Subscribes the report console system to a <see cref="IObservable{T}" /> for <see cref="IOperationReport" />
        ///     sequences with the given source name. Active subscription is disposed if it exists
        /// </summary>
        /// <param name="reportSource"></param>
        /// <param name="sourceName"></param>
        public void ChangeReportSubscription(IObservable<IEnumerable<IOperationReport>> reportSource, string sourceName)
        {
            if (reportSource == null) throw new ArgumentNullException(nameof(reportSource));
            if (sourceName == null) throw new ArgumentNullException(nameof(sourceName));

            lock (lockObject)
            {
                ReportSourceSubscription?.Dispose();
                ReportSourceSubscription = reportSource.Subscribe(reports => DisplayReports(reports, $"{sourceName} [Live]"));
            }
        }

        /// <summary>
        ///     Action that is called if the <see cref="IsErrorContentFiltered"/> property changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFilterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsErrorContentFiltered))
            {
                DisplayReports(LastReportSet, ReportSourceName);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            ReportSourceSubscription?.Dispose();
            base.Dispose();
        }
    }
}