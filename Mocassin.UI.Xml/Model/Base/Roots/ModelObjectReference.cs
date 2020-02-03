using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Generic serializable class to store and provide key based references to specific <see cref="ModelObject" />
    ///     instances
    /// </summary>
    [XmlRoot]
    public sealed class ModelObjectReference<T> : PropertyChangeNotifier,
        IEquatable<ModelObjectReference<T>>,
        IComparable<ModelObjectReference<T>>,
        IComparable,
        IDuplicable<ModelObjectReference<T>> where T : ModelObject, new()
    {
        private ModelDataObject target;
        private string key;

        /// <summary>
        ///     Get or set the key of the reference
        /// </summary>
        [JsonIgnore]
        [XmlAttribute]
        public string Key
        {
            get => key;
            set => SetProperty(ref key, value);
        }

        /// <summary>
        ///     Get the name of the object reference
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public string Name => Target?.Name ?? "TargetNull";

        /// <summary>
        ///     Get or set the reference target <see cref="ModelDataObject" />. Only serialized in JSON mode with reference
        ///     handling
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public ModelDataObject Target
        {
            get => target;
            set
            {
                if (target != null) target.PropertyChanged -= RelayTargetNameChange;
                if (value != null) value.PropertyChanged += RelayTargetNameChange;
                SetProperty(ref target, value);
                Key = value?.Key;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <inheritdoc />
        public ModelObjectReference()
        {
        }

        /// <summary>
        ///     Creates new <see cref="ModelObjectReference{T}" /> that targets the passed graph
        /// </summary>
        /// <param name="targetGraph"></param>
        public ModelObjectReference(ModelDataObject targetGraph)
        {
            Target = targetGraph ?? throw new ArgumentNullException(nameof(targetGraph));
        }

        /// <summary>
        ///     Get the internal model object
        /// </summary>
        /// <returns></returns>
        public ModelObject GetInputObject()
        {
            return new T {Key = Target?.Key ?? Key};
        }

        /// <inheritdoc />
        public bool Equals(ModelObjectReference<T> other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key == other.Key || ReferenceEquals(Target, other.Target);
        }

        /// <inheritdoc />
        public ModelObjectReference<T> Duplicate()
        {
            return new ModelObjectReference<T>(Target);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as ModelObjectReference<T>);
        }

        /// <inheritdoc />
        public int CompareTo(ModelObjectReference<T> other)
        {
            if (other == null) return 1;
            if (target == null || other.Target == null) return string.Compare(Key, other.Key, StringComparison.Ordinal);
            return string.Compare(target.Name, other.Target.Name, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 1207054110;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Target?.Key);
            return hashCode;
        }

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            return CompareTo(obj as ModelObjectReference<T>);
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }

        /// <summary>
        ///     Relays a name change of the <see cref="ProjectDataObject"/> target to a name change of this reference
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void RelayTargetNameChange(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(target.Name))
            {
                OnPropertyChanged(nameof(Name));
            }
        }
    }
}