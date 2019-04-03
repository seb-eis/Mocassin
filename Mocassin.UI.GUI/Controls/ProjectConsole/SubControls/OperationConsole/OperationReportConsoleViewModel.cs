using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Mocassin.Framework.Operations;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole.Commands;
using Mocassin.UI.GUI.Logic.Validation;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.Model;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="OperationReportConsoleView" /> that displays
    ///     and updates <see cref="IOperationReport" /> collections for automated model live checking
    /// </summary>
    public class OperationReportConsoleViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get a dummy <see cref="MocassinProjectGraph" /> that serves as a stop dummy for the live validation
        /// </summary>
        private static readonly MocassinProjectGraph DummyProjectGraph = new MocassinProjectGraph {ProjectName = "[None]"};

        private readonly object lockObject = new object();
        private bool isErrorContentFiltered;
        private bool isSoftUpdateStop;
        private bool isHardUpdateStop;
        private string reportSourceName;
        private IOperationReport selectedReport;
        private TimeSpan validationInterval = TimeSpan.FromSeconds(5);
        private MocassinProjectGraph selectedProjectGraph;

        /// <summary>
        ///     Get or set the last received <see cref="IEnumerable{T}" /> of <see cref="IOperationReport" /> instances
        /// </summary>
        private IEnumerable<IOperationReport> LastReportSet { get; set; }

        /// <summary>
        ///     Get or set a <see cref="ModelLiveValidator" /> instance that periodically checks its linked model definition
        /// </summary>
        private ModelLiveValidator LiveValidator { get; set; }

        /// <summary>
        ///     The <see cref="IDisposable" /> for an active report subscription
        /// </summary>
        private IDisposable LiveReportSubscription { get; set; }

        /// <summary>
        ///     The <see cref="ObservableCollectionViewModel{T}" /> for the set of <see cref="IOperationReport" />
        /// </summary>
        public ObservableCollectionViewModel<IOperationReport> OperationCollectionViewModel { get; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of selectable project graphs
        /// </summary>
        public IEnumerable<MocassinProjectGraph> ProjectGraphs => GetProjectGraphs();

        /// <summary>
        ///     Get or set the currently selected <see cref="IOperationReport" />
        /// </summary>
        public IOperationReport SelectedReport
        {
            get => selectedReport;
            set => SetProperty(ref selectedReport, value);
        }

        /// <summary>
        ///     Get or set the selected <see cref="MocassinProjectGraph" /> that is live validated
        /// </summary>
        public MocassinProjectGraph SelectedProjectGraph
        {
            get => selectedProjectGraph;
            set => SetProperty(ref selectedProjectGraph, value);
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
        ///     Get or set the live validation interval <see cref="TimeSpan" />
        /// </summary>
        public TimeSpan ValidationInterval
        {
            get => validationInterval;
            set
            {
                SetProperty(ref validationInterval, value);
                if (LiveValidator == null) return;
                LiveValidator.ValidationInterval = value;
            }
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
        ///     Get the <see cref="SetSoftUpdateStopCommand" /> to set/unset the affiliated flag
        /// </summary>
        public SetSoftUpdateStopCommand SoftUpdateStopCommand { get; }

        /// <inheritdoc />
        public OperationReportConsoleViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            OperationCollectionViewModel = new ObservableCollectionViewModel<IOperationReport>();
            SoftUpdateStopCommand = new SetSoftUpdateStopCommand(this);
            PropertyChanged += OnFilterChanged;
            PropertyChanged += OnProjectGraphChanged;
        }

        /// <summary>
        ///     Requests the displaying of the passed <see cref="IEnumerable{T}" /> sequence of
        /// </summary>
        /// <param name="reports"></param>
        public void DisplayReports(IEnumerable<IOperationReport> reports)
        {
            lock (lockObject)
            {
                if (isSoftUpdateStop || isHardUpdateStop) return;

                LastReportSet = reports;
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
        ///     sequences. Active subscription is disposed if it exists
        /// </summary>
        /// <param name="reportSource"></param>
        public void ChangeReportSubscription(IObservable<IEnumerable<IOperationReport>> reportSource)
        {
            lock (lockObject)
            {
                AsyncDisposeLiveValidation();
                DisplayReports(null);
                if (reportSource == null) return;

                LiveReportSubscription = reportSource.Subscribe(DisplayReports);
            }
        }

        /// <summary>
        ///     Starts a <see cref="Task" /> that awaits disposal of the current <see cref="ModelLiveValidator" /> and affiliated
        ///     subscription
        /// </summary>
        private void AsyncDisposeLiveValidation()
        {
            var subscription = LiveReportSubscription;
            var validator = LiveValidator;
            Task.Run(() =>
            {
                subscription?.Dispose();
                validator?.Dispose();
                SendCallInfoMessage($"A live validation for [{validator?.ModelGraph.Parent.ProjectName}] was deactivated!");
            });
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            LiveReportSubscription?.Dispose();
            base.Dispose();
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of selectable <see cref="MocassinProjectGraph" /> instances for live
        ///     validation
        /// </summary>
        /// <returns></returns>
        private IEnumerable<MocassinProjectGraph> GetProjectGraphs()
        {
            yield return DummyProjectGraph;
            if (ProjectControl.ProjectGraphs == null) yield break;
            foreach (var projectGraph in ProjectControl.ProjectGraphs) yield return projectGraph;
        }

        /// <summary>
        ///     Action that is called if the <see cref="IsErrorContentFiltered" /> property changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFilterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsErrorContentFiltered)) DisplayReports(LastReportSet);
        }

        /// <summary>
        ///     Action that is called if the <see cref="SelectedProjectGraph" /> property changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnProjectGraphChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectedProjectGraph)) return;
            await Task.Run(() => SwitchLiveValidationTarget(SelectedProjectGraph?.ProjectModelGraph));
        }

        /// <summary>
        ///     Switches the <see cref="ModelLiveValidator" /> system to target the passed <see cref="ProjectModelGraph" /> or
        ///     stops the system if the argument is null
        /// </summary>
        /// <param name="targetModelGraph"></param>
        public void SwitchLiveValidationTarget(ProjectModelGraph targetModelGraph)
        {
            if (targetModelGraph == null || ReferenceEquals(targetModelGraph, DummyProjectGraph.ProjectModelGraph))
            {
                ChangeReportSubscription(null);
                return;
            }

            var liveValidator = new ModelLiveValidator(targetModelGraph, ProjectControl);
            ChangeReportSubscription(liveValidator.ReportsChangeNotification);
            liveValidator.ValidationInterval = ValidationInterval;
            liveValidator.StartContinuousValidation();
            LiveValidator = liveValidator;
            SendCallInfoMessage($"A live validation for [{targetModelGraph.Parent.ProjectName}] is now active!");
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            OnPropertyChanged(nameof(ProjectGraphs));
        }

        /// <inheritdoc />
        protected override void OnProjectContentChangedInternal()
        {
            OnPropertyChanged(nameof(ProjectGraphs));
        }
    }
}