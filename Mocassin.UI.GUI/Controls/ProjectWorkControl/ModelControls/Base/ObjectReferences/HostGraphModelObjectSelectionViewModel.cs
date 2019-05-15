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
    ///     <see cref="ModelObjectReferenceGraph{T}" /> collection selections for a hosting <see cref="ModelObjectGraph" />
    /// </summary>
    /// <typeparam name="TModelObject"></typeparam>
    /// <typeparam name="TObjectGraph"></typeparam>
    public abstract class HostGraphModelObjectSelectionViewModel<TModelObject, TObjectGraph> :
        CollectionControlViewModel<ModelObjectReferenceGraph<TModelObject>>,
        IContentSupplier<MocassinProjectGraph>,
        IObjectDropAcceptor
        where TModelObject : ModelObject, new()
        where TObjectGraph : ModelObjectGraph
    {
        /// <summary>
        ///     Get or set the <see cref="IReadOnlyCollection{T}" /> of <see cref="MocassinProjectGraph" /> instances that can be
        ///     referenced
        /// </summary>
        public IReadOnlyCollection<ModelObjectGraph> ReferenceObjectGraphs { get; private set; }

        /// <summary>
        ///     Get the boolean flag if the key selection filters out duplicate key <see cref="string" /> values
        /// </summary>
        public bool IsDuplicateFiltered { get; }

        /// <summary>
        ///     Get the hosting <see cref="TObjectGraph" />
        /// </summary>
        public TObjectGraph HostObject { get; }

        /// <inheritdoc />
        public Command<IDataObject> HandleDropAddCommand { get; set; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of currently selectable <see cref="ModelObjectGraph" /> instances
        /// </summary>
        public virtual IEnumerable<ModelObjectGraph> SelectableReferences => FilterReferences(ReferenceObjectGraphs);

        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Creates new <see cref="HostGraphModelObjectSelectionViewModel{TModelObject,TObjectGraph}" /> with a boolean
        ///     value that decides
        ///     if the key selection can contain duplicates
        /// </summary>
        /// <param name="hostObject"></param>
        /// <param name="isDuplicateFiltered"></param>
        protected HostGraphModelObjectSelectionViewModel(TObjectGraph hostObject, bool isDuplicateFiltered)
        {
            IsDuplicateFiltered = isDuplicateFiltered;
            HostObject = hostObject;
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectGraph" /> values that remains after internal
        ///     filtering is applied
        /// </summary>
        /// <param name="baseCollection"></param>
        /// <returns></returns>
        public virtual IEnumerable<ModelObjectGraph> FilterReferences(IReadOnlyCollection<ModelObjectGraph> baseCollection)
        {
            if (!IsDuplicateFiltered || SelectedCollectionItem == null) return baseCollection;

            var targetCollection = GetTargetCollection(HostObject);
            return targetCollection == null
                ? baseCollection
                : baseCollection.Where(x => targetCollection.All(y => y.Key != x.Key));
        }

        /// <inheritdoc />
        public virtual void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            ReferenceObjectGraphs = GetSourceCollection(ContentSource);
            DataCollection = GetTargetCollection(HostObject);
        }

        /// <summary>
        ///     Extract the <see cref="IReadOnlyCollection{T}" /> of reference <see cref="ModelObjectGraph" /> from the passed
        ///     <see cref="MocassinProjectGraph" />
        /// </summary>
        /// <param name="projectGraph"></param>
        /// <returns></returns>
        protected abstract IReadOnlyCollection<ModelObjectGraph> GetSourceCollection(MocassinProjectGraph projectGraph);

        /// <summary>
        ///     Extract the <see cref="ICollection{T}" /> of already defined <see cref="ModelObjectReferenceGraph{T}" /> on the
        ///     passed source graph
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <returns></returns>
        protected abstract ICollection<ModelObjectReferenceGraph<TModelObject>> GetTargetCollection(TObjectGraph sourceObject);

        /// <summary>
        ///     Get a <see cref="Command{T}" /> to add a <see cref="TGraph" /> reference from a <see cref="IDataObject" /> drop to
        ///     the host collection
        /// </summary>
        /// <returns></returns>
        public Command<IDataObject> GetDropAddObjectCommand<TGraph>() where TGraph : ModelObjectGraph
        {
            void Execute(IDataObject obj)
            {
                if (!(obj.GetData(typeof(TGraph)) is TGraph graph)) return;
                DataCollection.Add(new ModelObjectReferenceGraph<TModelObject> {Key = graph.Key, TargetGraph = graph});
            }

            bool CanExecute(IDataObject obj)
            {
                if (!(obj.GetData(typeof(TGraph)) is TGraph graph)) return false;
                var result = !GetDropRejectPredicates().Any(predicate => predicate.Invoke(obj));
                result &=  !IsDuplicateFiltered || GetTargetCollection(HostObject).All(x => x.Key != graph.Key);
                return result;
            }

            return new RelayCommand<IDataObject>(Execute, CanExecute);
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}"/> of additional rejection <see cref="Predicate{T}"/> instances for a drop object
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Predicate<IDataObject>> GetDropRejectPredicates()
        {
            yield break;
        }
    }
}