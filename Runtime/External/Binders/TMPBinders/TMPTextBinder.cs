#if R3_ENABLED && TMP_SUPPORT

using System;
using R3;
using TMPro;

namespace MVVM
{
    public class TMPTextBinder<T> : ObservableBinder<TMP_Text, T>
    {
        private IDisposable _disposable;
        
        public TMPTextBinder(TMP_Text view, Observable<T> model) : base(view, model)
        {
        }
    
        public override void Bind()
        {
            _disposable = Model.Subscribe(Invoke);
        }
    
        public override void Unbind()
        {
            _disposable?.Dispose();
        }

        private void Invoke(T data)
        {
            View.SetText(data?.ToString());
        }
    }
}

#endif
