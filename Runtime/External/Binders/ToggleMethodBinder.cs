#if R3_SUPPORT

using R3;
using System;
using UnityEngine.UI;

namespace MVVM
{
    public class ToggleMethodBinder : Binder<Toggle, Action<bool>>
    {
        private IDisposable _disposable;
        
        public ToggleMethodBinder(Toggle view, Action<bool> model) : base(view, model)
        {
        }

        public override void Bind()
        {
            _disposable = View.OnValueChangedAsObservable().Subscribe(Model);
        }

        public override void Unbind()
        {
            _disposable?.Dispose();
        }
    }
}

#endif
