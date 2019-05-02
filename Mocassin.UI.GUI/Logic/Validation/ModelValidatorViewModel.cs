﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mocassin.Framework.Events;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Model.ModelProject;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.GUI.Logic.Validation
{
    /// <summary>
    ///     Enumeration for model validation status
    /// </summary>
    [Flags]
    public enum ModelValidationStatus
    {
        NoErrorsDetected = 0,
        FatalException = 1,
        ModelNotReady = 1 << 1,
        ModelRejected = 1 << 2,
        CustomizationNotCreatable = 1 << 3
    }

    /// <summary>
    ///     A <see cref="PrimaryControlViewModel" /> live validator for <see cref="ProjectModelGraph" /> instances that
    ///     provides affiliated
    ///     update events and supports continuous validation cycles
    /// </summary>
    public class ModelValidatorViewModel : PrimaryControlViewModel
    {
        private readonly object lockObject = new object();
        private bool isIgnoreContentChange;

        /// <summary>
        ///     Get the <see cref="IModelProject"/> interface that provides the validation functionality
        /// </summary>
        private IModelProject ModelProject { get; }

        /// <summary>
        ///     Get or set a boolean value if the validator is disposed
        /// </summary>
        private bool IsDisposed { get; set; }

        /// <summary>
        ///     The <see cref="ReactiveEvent{TSubject}" /> for changes in the <see cref="IOperationReport" /> set
        /// </summary>
        private ReactiveEvent<IEnumerable<IOperationReport>> ReportSetChangedEvent { get; }

        /// <summary>
        ///     The <see cref="ReactiveEvent{TSubject}" /> for changes in
        /// </summary>
        private ReactiveEvent<ModelValidationStatus> ModelValidationStatusChangedEvent { get; }

        /// <summary>
        ///     Get or set the active validation routine <see cref="Task" />
        /// </summary>
        private Task ValidationTask { get; set; }

        /// <summary>
        ///     Get or set the <see cref="CancellationTokenSource" /> to stop the validation routine task
        /// </summary>
        private CancellationTokenSource CancellationSource { get; set; }

        /// <summary>
        ///     Get the <see cref="ProjectModelGraph" /> that the validator is using
        /// </summary>
        public ProjectModelGraph ModelGraph { get; }

        /// <summary>
        ///     Gee the <see cref="IObservable{T}" /> for changes in the set of <see cref="IOperationReport" /> instances
        /// </summary>
        public IObservable<IEnumerable<IOperationReport>> ReportsChangeNotification => ReportSetChangedEvent.AsObservable();

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> for notifications about changes in the <see cref="ModelValidationStatus" />
        /// </summary>
        public IObservable<ModelValidationStatus> StatusChangeNotification => ModelValidationStatusChangedEvent.AsObservable();

        /// <summary>
        ///     Get or set the <see cref="TimeSpan" /> between validation cycles
        /// </summary>
        public TimeSpan ValidationInterval { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        ///     Get the <see cref="AsyncRunValidationCommand"/> to start a single validation cycle
        /// </summary>
        public AsyncRunValidationCommand RunValidationCommand { get; }

        /// <summary>
        ///     Get or set a boolean flag if the validation system ignores received content change notifications
        /// </summary>
        public bool IsIgnoreContentChange
        {
            get => isIgnoreContentChange;
            set => SetProperty(ref isIgnoreContentChange, value);
        }

        /// <summary>
        ///     Creates new <see cref="ModelValidatorViewModel" /> for the passed <see cref="ProjectModelGraph" /> and
        ///     <see cref="IMocassinProjectControl" />
        /// </summary>
        /// <param name="modelGraph"></param>
        /// <param name="projectControl"></param>
        public ModelValidatorViewModel(ProjectModelGraph modelGraph, IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ModelGraph = modelGraph ?? throw new ArgumentNullException(nameof(modelGraph));
            ReportSetChangedEvent = new ReactiveEvent<IEnumerable<IOperationReport>>();
            ModelValidationStatusChangedEvent = new ReactiveEvent<ModelValidationStatus>();
            RunValidationCommand = new AsyncRunValidationCommand(this);
            ModelProject = ProjectControl.CreateModelProject();
        }

        /// <summary>
        ///     Starts the continues validation process. Returns false if the process could not be started or is already running
        /// </summary>
        /// <returns></returns>
        public bool StartContinuousValidation()
        {
            lock (lockObject)
            {
                if (ValidationTask != null || IsDisposed) return false;
                CancellationSource = new CancellationTokenSource();
                ValidationTask = Task.Run(() =>
                {
                    RunModelValidationCycle(CancellationSource.Token);
                    DisposeInternal();
                });
                return true;
            }
        }

        /// <summary>
        ///     Stops the continues validation process. Returns false if the process did not stop within an acceptable
        ///     <see cref="TimeSpan" />
        /// </summary>
        /// <returns></returns>
        public bool StopContinuousValidation()
        {
            lock (lockObject)
            {
                if (ValidationTask == null) return true;
                CancellationSource?.Cancel();
                ValidationTask.Wait(TimeSpan.FromSeconds(10));
                return ValidationTask.IsCompleted;
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            StopContinuousValidation();
            DisposeInternal();
            base.Dispose();
        }

        /// <summary>
        ///     Internal <see cref="Dispose" /> implementation
        /// </summary>
        private void DisposeInternal()
        {
            lock (lockObject)
            {
                ReportSetChangedEvent.OnCompleted();
                ModelValidationStatusChangedEvent.OnCompleted();
                IsDisposed = true;
            }
        }

        /// <summary>
        ///     Runs the validation cycle with the set interval settings until cancellation is requested
        /// </summary>
        /// <param name="cancellationToken"></param>
        private void RunModelValidationCycle(CancellationToken cancellationToken)
        {
            var lastCheck = DateTime.Now;
            while (!cancellationToken.IsCancellationRequested
                   && (ReportSetChangedEvent.HasObservers || ModelValidationStatusChangedEvent.HasObservers))
            {
                lastCheck = WaitForNextValidation(lastCheck);
                RunValidation();
                lastCheck = DateTime.Now;
            }
        }

        /// <summary>
        ///     Performs a single validation cycle run on the internally set <see cref="ProjectModelGraph"/>
        /// </summary>
        public void RunValidation()
        {
            ModelProject.ResetProject();
            if (!TryPrepareModelInput(out var inputObjects))
            {
                ModelValidationStatusChangedEvent.OnNext(ModelValidationStatus.ModelNotReady);
                return;
            }

            if (!TryPushModelInput(ModelProject, inputObjects, out var reports))
            {
                ModelValidationStatusChangedEvent.OnNext(ModelValidationStatus.FatalException);
                return;
            }

            ReportSetChangedEvent.OnNext(reports);
            if (reports.Any(x => !x.IsGood))
            {
                ModelValidationStatusChangedEvent.OnNext(ModelValidationStatus.ModelRejected);
                return;
            }

            if (!TryGenerateModelCustomization(ModelProject, out var customization))
            {
                ModelValidationStatusChangedEvent.OnNext(ModelValidationStatus.CustomizationNotCreatable);
                return;
            }

            ModelValidationStatusChangedEvent.OnNext(ModelValidationStatus.NoErrorsDetected);
        }

        /// <summary>
        ///     Tries to create a new <see cref="ProjectCustomizationGraph"/> for the current <see cref="ProjectModelGraph"/>
        /// </summary>
        /// <param name="customization"></param>
        /// <returns></returns>
        public ModelValidationStatus TryCreateCustomization(out ProjectCustomizationGraph customization)
        {
            ModelProject.ResetProject();
            customization = null;
            if (!TryPrepareModelInput(out var inputObjects))
            {
                return ModelValidationStatus.ModelNotReady;
            }

            if (!TryPushModelInput(ModelProject, inputObjects, out var reports))
            {
                return ModelValidationStatus.FatalException;
            }

            if (reports.Any(x => !x.IsGood))
            {
                return ModelValidationStatus.ModelRejected;
            }

            return !TryGenerateModelCustomization(ModelProject, out customization) 
                ? ModelValidationStatus.CustomizationNotCreatable 
                : ModelValidationStatus.NoErrorsDetected;
        }

        /// <summary>
        ///     Waits for the next validation time point and returns the time at which the method finished
        /// </summary>
        /// <param name="lastCheck"></param>
        /// <returns></returns>
        private DateTime WaitForNextValidation(DateTime lastCheck)
        {
            var delta = DateTime.Now - lastCheck;
            if (delta < ValidationInterval) Thread.Sleep(ValidationInterval - delta);
            return DateTime.Now;
        }

        /// <summary>
        ///     Tries to prepare a <see cref="IList{T}" /> of model input <see cref="object" /> instances from the internally set
        ///     <see cref="ProjectModelGraph" />. Returns false if the data is currently not convertible
        /// </summary>
        /// <param name="inputObjects"></param>
        /// <returns></returns>
        private bool TryPrepareModelInput(out IList<object> inputObjects)
        {
            try
            {
                inputObjects = ModelGraph.GetInputSequence().ToList();
                return true;
            }
            catch (Exception)
            {
                inputObjects = null;
                return false;
            }
        }

        /// <summary>
        ///     Tries to push the passed <see cref="IList{T}" /> of model <see cref="object" /> instances to the passed
        ///     <see cref="IModelProject" />. Returns false if the push is aborted for any reason
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="inputObjects"></param>
        /// <param name="reports"></param>
        /// <returns></returns>
        private bool TryPushModelInput(IModelProject modelProject, IList<object> inputObjects, out IList<IOperationReport> reports)
        {
            try
            {
                reports = modelProject.InputPipeline.PushToProject(inputObjects);
                return true;
            }
            catch (Exception)
            {
                reports = null;
                return false;
            }
        }

        /// <summary>
        ///     Tries to generate a <see cref="ProjectCustomizationGraph" /> from the provided <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="customization"></param>
        /// <returns></returns>
        private bool TryGenerateModelCustomization(IModelProject modelProject, out ProjectCustomizationGraph customization)
        {
            try
            {
                customization = ProjectCustomizationGraph.Create(modelProject, ModelGraph);
                return true;
            }
            catch (Exception)
            {
                customization = null;
                return false;
            }
        }

        /// <inheritdoc />
        protected override async void OnProjectContentChangedInternal()
        {
            if (IsIgnoreContentChange) return;
            await RunValidationCommand.ExecuteAsync(null);
        }
    }
}