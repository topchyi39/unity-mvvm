#if R3_SUPPORT

using R3;
using UnityEngine.UI;

namespace MVVM
{
    public class ToggleBinder : Binder<Toggle, ReactiveProperty<bool>>
    {
        private DisposableBag _disposable;

        public ToggleBinder(Toggle view, ReactiveProperty<bool> model) : base(view, model)
        {
        }

        public override void Bind()
        {
            View.OnValueChangedAsObservable().Subscribe(value => Model.Value = value).AddTo(ref _disposable);
            Model.Where(value => value != View.isOn).Subscribe(value => View.isOn = value).AddTo(ref _disposable);
        }

        public override void Unbind()
        {
            _disposable.Dispose();
        }

        private void InvokeFromToggle(bool toggle)
        {
            Model.Value = toggle;
        }
        
    }
}

#endif
