using System;
using System.Collections;
using System.Collections.Generic;

namespace Singular.Grid.Model
{
    public class ModelManager
    {
        private readonly NamespaceCollection<IAdressable> Types = new NamespaceCollection<IAdressable>();

        public void AddDataType(Func<ComplexDataTypeBuilder.IStart, ComplexDataTypeBuilder.IBuild> datatype)
        {
            ComplexDataTypeBuilder.IBuild build = datatype(new ComplexDataTypeBuilder.Builder());
            Types.Add(build.Build());
        }

        public void AddEnumeration(Func<EnumerationBuilder.IStart, EnumerationBuilder.IBuild> datatype)
        {
            EnumerationBuilder.IBuild build = datatype(new EnumerationBuilder.Builder());
            Types.Add(build.Build());
        }

        public void AddAspect(Func<ModelAspectBuilder.IStart, ModelAspectBuilder.IAspect> datatype)
        {
            ModelAspectBuilder.IAspect build = datatype(new ModelAspectBuilder.Builder());
            Types.Add(build.Build());
        }
    }

    public class NamespaceCollection<T> : ICollection<T> where T : IAdressable
    {
        private readonly Dictionary<string, T> elements = new Dictionary<string, T>();

        public IEnumerator<T> GetEnumerator() => elements.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => elements.Add(item.Name.FullyQualifiedName, item);

        public void Clear() => elements.Clear();

        public bool Contains(T item) => elements.ContainsKey(item.Name.FullyQualifiedName);

        public void CopyTo(T[] array, int arrayIndex) => elements.Values.CopyTo(array, arrayIndex);

        public bool Remove(T item) => elements.Remove(item.Name.FullyQualifiedName);

        public int Count => elements.Count;

        public bool IsReadOnly => false;
    }
}