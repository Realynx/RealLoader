using System.Collections;

using Microsoft.Extensions.DependencyInjection;

namespace PalworldManagedModFramework.Services.SandboxDI.ServiceResolution {
    public class SandboxServiceCollection : IServiceCollection {
        private readonly List<ServiceDescriptor> _descriptors = new List<ServiceDescriptor>();

        public ServiceDescriptor this[int index] {
            get {
                return _descriptors[index];
            }

            set {
                _descriptors[index] = value;
            }
        }

        public int Count {
            get {
                return _descriptors.Count;
            }
        }

        public bool IsReadOnly {
            get {
                return ((ICollection<ServiceDescriptor>)_descriptors).IsReadOnly;
            }
        }

        public void Add(ServiceDescriptor item) {
            ArgumentNullException.ThrowIfNull(item);

            if (item.Lifetime != ServiceLifetime.Singleton) {
                throw new NotSupportedException("Only singleton services are supported in this container.");
            }

            _descriptors.Add(item);
        }

        public void Clear() {
            _descriptors.Clear();
        }

        public bool Contains(ServiceDescriptor item) {
            return _descriptors.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex) {
            _descriptors.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator() {
            return _descriptors.GetEnumerator();
        }

        public int IndexOf(ServiceDescriptor item) {
            return _descriptors.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item) {
            _descriptors.Insert(index, item);
        }

        public bool Remove(ServiceDescriptor item) {
            return _descriptors.Remove(item);
        }

        public void RemoveAt(int index) {
            _descriptors.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
