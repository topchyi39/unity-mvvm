#if R3_SUPPORT

using R3;
using System;

namespace MVVM
{
    public class GenericMethodObservableBinder<T> : ObservableBinder<Action<T>, T>
    {
        private IDisposable _disposable;
        
        public GenericMethodObservableBinder(Action<T> view, Observable<T> model) : base(view, model)
        {
        }

        public override void Bind()
        {
            _disposable = Model.Subscribe(Invoke);
        }

        public override void Unbind()
        {
            _disposable.Dispose();
        }

        private void Invoke(T value)
        {
            View(value);
        }
    }
}

#endif