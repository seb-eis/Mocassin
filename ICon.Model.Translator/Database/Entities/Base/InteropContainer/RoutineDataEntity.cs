using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Routine data class that is used to encode a custom routine identification and parameters
    /// </summary>
    public abstract class RoutineDataEntity : BlobEntityBase
    {
        /// <summary>
        ///  Get the <see cref="IReadOnlyDictionary{TKey,TValue}"/> of known <see cref="RoutineDataEntity"/> types
        /// </summary>
        public static IReadOnlyDictionary<MocsimExtensionComponentAttribute, Func<RoutineDataEntity>> RoutineDataConstructors { get; }

        /// <summary>
        ///     Get the default <see cref="Regex"/> used to parse routine instructions from a job instruction string
        /// </summary>
        public static Regex DefaultInstructionRegex { get; } 
            = new Regex("routine:\\s*(?<alias>[^=\\s]+)\\s*@\\s*\\{\\s*(?<params>[^}]+)\\s*\\}", RegexOptions.Singleline);

        /// <summary>
        ///     Get the default <see cref="Regex"/> used to parse the routine parameters from a found routine instruction
        /// </summary>
        public static Regex DefaultParameterRegex { get; } = new Regex("(?<name>[a-zA-Z0-9]+)\\s*=\\s*\"(?<value>[^\"]+)\"");

        /// <summary>
        ///     Get the <see cref="Guid" /> that identifies the routine
        /// </summary>
        public abstract Guid RoutineGuid { get; }

        /// <summary>
        ///     Get or set the <see cref="InteropObject{T}" /> that contains the routine parameter data
        /// </summary>
        protected InteropObject InternalParameterObject { get; set; }

        /// <inheritdoc />
        public override int BlobByteCount => HeaderByteCount + InternalParameterObject?.ByteCount ?? 0;

        /// <inheritdoc />
        public override int HeaderByteCount => 16;

        /// <summary>
        ///     Static constructor that generates the known types lookup dictionary
        /// </summary>
        static RoutineDataEntity()
        {
            var dictionary = new Dictionary<MocsimExtensionComponentAttribute, Func<RoutineDataEntity>>();
            var searchAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic);
            foreach (var typeInfo in searchAssemblies.SelectMany(x => x.ExportedTypes).Where(x => typeof(RoutineDataEntity).IsAssignableFrom(x)))
            {
                var attribute = typeInfo.GetCustomAttribute<MocsimExtensionComponentAttribute>();
                if (attribute == null) continue;
                var constructor = typeInfo.GetConstructor(Type.EmptyTypes);
                if (constructor == null) continue;
                dictionary.Add(attribute, () => (RoutineDataEntity) constructor.Invoke(null));
            }

            RoutineDataConstructors = dictionary;
        }

        /// <inheritdoc />
        public override void ChangeStateToBinary(IMarshalService marshalService)
        {
            if (BinaryState != null) return;
            if (InternalParameterObject == null) throw new InvalidOperationException("Parameter object is null.");

            var buffer = new byte[BlobByteCount];
            Buffer.BlockCopy(RoutineGuid.ToByteArray(), 0, buffer, 0, 16);
            InternalParameterObject.ToBinary(buffer, 16, marshalService);
            BinaryState = buffer;

            InternalParameterObject = null;
        }

        /// <inheritdoc />
        public override void ChangeStateToObject(IMarshalService marshalService)
        {
            if (InternalParameterObject != null) return;
            if (BinaryState == null) throw new InvalidOperationException("Binary state is null.");
            InternalParameterObject = GetInteropObjectFromBinaryState(marshalService);
            BinaryState = null;
        }

        /// <summary>
        ///     Method to extract the actual <see cref="InteropObject" /> from the binary state using the provided
        ///     <see cref="IMarshalService" />
        /// </summary>
        /// <param name="marshalService"></param>
        /// <returns></returns>
        protected abstract InteropObject GetInteropObjectFromBinaryState(IMarshalService marshalService);

        /// <summary>
        ///     Tries to parse a parameter <see cref="string"/> of the routine, returns false on error. Format is implementation dependent
        /// </summary>
        /// <param name="str"></param>
        public virtual bool TryParseParameters(string str)
        {
            return true;
        }

        /// <summary>
        ///  Creates an empty <see cref="RoutineDataEntity"/> with an empty guid and one byte of data (17 total)
        /// </summary>
        /// <returns></returns>
        public static RoutineDataEntity CreateEmpty()
        {
            return new RoutineDataEntity<Unit> {ParameterObject = InteropObject.Create(Unit.Default)};
        }

        /// <summary>
        ///     Tries to parse a routine instruction from a <see cref="string"/> into the matching <see cref="RoutineDataEntity"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool TryParse(string str, out RoutineDataEntity entity)
        {
            entity = default;

            if (!DefaultInstructionRegex.IsMatch(str)) return false;
            var match = DefaultInstructionRegex.Match(str);

            entity = CreateRoutineObject(match.Groups["alias"].Value);
            if (entity == null) return false;

            if (entity.TryParseParameters(match.Groups["params"].Value)) return true;

            entity = default;
            return false;
        }

        /// <summary>
        ///     Creates a new <see cref="RoutineDataEntity"/> that matches the provided identification <see cref="string"/>
        /// </summary>
        /// <param name="identification"></param>
        /// <returns></returns>
        private static RoutineDataEntity CreateRoutineObject(string identification)
        {
            var obj = Guid.TryParse(identification, out var guid) 
                ? RoutineDataConstructors.SingleOrDefault(x => x.Key.ExtensionGuid == guid).Value?.Invoke()
                : RoutineDataConstructors.SingleOrDefault(x => x.Key.ExtensionAlias == identification).Value?.Invoke();
            return obj ?? CreateEmpty();
        }
    }

    /// <summary>
    ///     Generic routine data class that is used to encode a custom routine identification and parameters
    /// </summary>
    public class RoutineDataEntity<T> : RoutineDataEntity where T : struct
    {
        /// <inheritdoc />
        public override Guid RoutineGuid => GetGuidInternal();

        /// <summary>
        ///     Get or set the <see cref="InteropObject{T}" /> that stores the routine parameters
        /// </summary>
        public InteropObject<T> ParameterObject
        {
            get => (InteropObject<T>) InternalParameterObject;
            set => InternalParameterObject = value;
        }

        /// <inheritdoc />
        protected override InteropObject GetInteropObjectFromBinaryState(IMarshalService marshalService)
        {
            return InteropObject.Create(marshalService.GetStructure<T>(BinaryState, 16));
        }

        /// <summary>
        ///     Internal GUI lookup that uses the <see cref="MocsimExtensionComponentAttribute" /> marker, returns the empty
        ///     <see cref="Guid" /> if the marker is missing
        /// </summary>
        /// <returns></returns>
        protected virtual Guid GetGuidInternal()
        {
            return GetType().GetCustomAttribute<MocsimExtensionComponentAttribute>()?.ExtensionGuid ?? Guid.Empty;
        }

        /// <summary>
        ///  Tries to parse the passed parameters and uses the parameter names for value setting using reflection
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public override bool TryParseParameters(string str)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var matches = DefaultParameterRegex.Matches(str);
            if (matches.Count == 0) return true;

            var boxedStructure = (object) ParameterObject.Structure;
            foreach (Match match in matches)
            {
                var name = match.Groups["name"].Value;
                var value = match.Groups["value"].Value;
                var property = properties.FirstOrDefault(x => x.Name == name);
                if (property == null) return false;

                try
                {
                    var convertedValue = Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture);
                    property.SetValue(boxedStructure, convertedValue);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            ParameterObject = InteropObject.Create((T) boxedStructure);
            return true;
        }
    }
}