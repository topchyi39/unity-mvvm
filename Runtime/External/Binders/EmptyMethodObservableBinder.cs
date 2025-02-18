#if R3_SUPPORT

using R3;
using System;

namespace MVVM
{
    public class EmptyMethodObservableBinder<T> : ObservableBinder<Action, T>
    {
        private IDisposable _disposable;
        
        public EmptyMethodObservableBinder(Action view, Observable<T> model) : base(view, model) { }

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
            View();
        }
    }
}
#endif