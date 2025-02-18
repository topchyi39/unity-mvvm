using System;
using UnityEngine;

namespace MVVM
{
    public abstract class RuntimeBinder : MonoBehaviour
    {
        [SerializeField] private View viewComponent;
        
        private IBinder _binder;

        protected Type ViewModelType => viewComponent.ViewModelType;
        
        private void Awake()
        {
            _binder = GetBinder();
        }

        private void OnEnable()
        {
            _binder?.Bind();
        }

        private void OnDisable()
        {
            _binder?.Unbind();
        }

        private IBinder GetBinder() => BinderFactory.Create(viewComponent, GetViewModel());
        
        protected abstract object GetViewModel(); 
    }
}