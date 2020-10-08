using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mocassin.UI.Data.Main
{
    /// <summary>
    ///     Serialization binder for <see cref="MocassinProject"/> to relay deprecated namespaces and types of old project files
    /// </summary>
    public class MocassinProjectSerializationBinder : ISerializationBinder
    {
        private static Dictionary<string, Type> TypeDictionary { get; } = new Dictionary<string, Type>();

        /// <inheritdoc />
        public Type BindToType(string assemblyName, string typeName)
        {
            if (TypeDictionary.TryGetValue(typeName, out var type)) return type;
            type = Type.GetType(typeName.Replace("UI.Xml", "UI.Data"));
            TypeDictionary.Add(typeName, type);
            return type;
        }

        /// <inheritdoc />
        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = null;
        }
    }
}