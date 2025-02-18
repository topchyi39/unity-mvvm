#if EXTENJECT_SUPPORT

using UnityEngine;
using Zenject;

namespace MVVM
{
    public class InjectBinder : RuntimeBinder
    {
        private DiContainer _container;
        
        [Inject]
        private void Construct(DiContainer container)
        {
            _container = container;
        }

        protected override object GetViewModel()
        {
            return _container.Resolve(ViewModelType);
        }
    }
}

#endif
