using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Mocassin.Model.Basic;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl
{
    /// <summary>
    ///     Base <see cref="ViewModelBase" /> for types that control grid data manipulation of
    ///     <see cref="ModelObjectReference{T}" /> collection selections for a hosting <see cref="ModelDataObject" />
    /// </summary>
    /// <typeparam name="TModelObject"></typeparam>
    /// <typeparam name="TDataObject"></typeparam>
    public abstract class HostGraphModelObjectSelectionViewModel<TModelObject, TDataObject> :
        CollectionControlViewModel<ModelObjectReference<TModelObject>>,
        IContentSupplier<MocassinProject>,
        IDataObjectAcceptor
        where TModelObject : ModelObject, new()
        where TDataObject : ModelDataObject
    {
        /// <summary>
        ///     Get or set the <see cref="IReadOnlyCollection{T}" /> of <see cref="MocassinProject" /> instances that can be
        ///     referenced
        /// </summary>
        public IReadOnlyCollection<ModelDataObject> ReferenceObjectGraphs { get; private set; }

        /// <summary>
        ///     Get the boolean flag if the key selection filters out duplicate key <see cref="string" /> values
        /// </summary>
        public bool IsDuplicateFiltered { get; }

        /// <summary>
        ///     Get the hosting <see cref="ModelDataObject"/>
        /// </summary>
        public TDataObject HostObject { get; }

        /// <inheritdoc />
        public Command<IDataObject> ProcessDataObjectCommand { get; set; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of currently selectable <see cref="ModelDataObject" /> instances
        /// </summary>
        public virtual IEnumerable<ModelDataObject> SelectableReferences => FilterReferences(ReferenceObjectGraphs);

        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <summary>
        ///     Creates new <see cref="HostGraphModelObjectSelectionViewModel{TModelObject,TObjectGraph}" /> with a boolean
        ///     value that decides
        ///     if the key selection can contain duplicates
        /// </summary>
        /// <param name="hostObject"></param>
        /// <param name="isDuplicateFiltered"></param>
        protected HostGraphModelObjectSelectionViewModel(TDataObject hostObject, bool isDuplicateFiltered)
        {
            IsDuplicateFiltered = isDuplicateFiltered;
            HostObject = hostObject;
        }

        /// <inheritdoc />
        public virtual void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            ReferenceObjectGraphs = GetSourceCollection(ContentSource);
            Items = GetTargetCollection(HostObject);
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelDataObject" /> values that remains after internal
        ///     filtering is applied
        /// </summary>
        /// <param name="baseCollection"></param>
        /// <returns></returns>
        public virtual IEnumerable<ModelDataObject> FilterReferences(IReadOnlyCollection<ModelDataObject> baseCollection)
        {
            if (!IsDuplicateFiltered || SelectedItem == null) return baseCollection;

            var targetCollection = GetTargetCollection(HostObject);
            return targetCollection == null
                ? baseCollection
                : baseCollection.Where(x => targetCollection.All(y => y.Key != x.Key));
        }

        /// <summary>
        ///     Extract the <see cref="IReadOnlyCollection{T}" /> of reference <see cref="ModelDataObject" /> from the passed
        ///     <see cref="MocassinProject" />
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        protected abstract IReadOnlyCollection<ModelDataObject> GetSourceCollection(MocassinProject project);

        /// <summary>
        ///     Extract the <see cref="ICollection{T}" /> of already defined <see cref="ModelObjectReference{T}" /> on the
        ///     passed source graph
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <returns></returns>
        protected abstract ICollection<ModelObjectReference<TModelObject>> GetTargetCollection(TDataObject sourceObject);

        /// <summary>
        ///     Get a <see cref="Command{T}" /> to add a reference from a <see cref="IDataObject" /> drop to the host collection
        /// </summary>
        /// <returns></returns>
        public Command<IDataObject> GetDropAddObjectCommand<TGraph>() where TGraph : ModelDataObject
        {
            void Execute(IDataObject obj)
            {
                if (!(obj.GetData(typeof(TGraph)) is TGraph graph)) return;
                Items.Add(new ModelObjectReference<TModelObject> {Key = graph.Key, Target = graph});
            }

            bool CanExecute(IDataObject obj)
            {
                if (!(obj.GetData(typeof(TGraph)) is TGraph graph)) return false;
                var result = !GetDropRejectPredicates().Any(predicate => predicate.Invoke(obj));
                result &= !IsDuplicateFiltered || GetTargetCollection(HostObject).All(x => x.Key != graph.Key);
                return result;
            }

            return new RelayCommand<IDataObject>(Execute, CanExecute);
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of additional rejection <see cref="Predicate{T}" /> instances for a drop
        ///     object
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Predicate<IDataObject>> GetDropRejectPredicates()
        {
            yield break;
        }
    }
}