#if R3_ENABLED && TMP_SUPPORT

using System;
using R3;
using TMPro;

namespace MVVM
{
    public class TMPInputBinder : SubjectBinder<TMP_InputField, string>
    {
        private IDisposable _disposable;
        public TMPInputBinder(TMP_InputField view, ReactiveProperty<string> model) : base(view, model)
        {
        }

        public override void Bind()
        {
            _disposable = View.onValueChanged.AsObservable().Subscribe(Model.OnNext);
        }

        public override void Unbind()
        {
            _disposable.Dispose();
        }
    }
}

#endif