#if R3_SUPPORT

using R3;
using System;
using UnityEngine.UI;

namespace MVVM
{
    public class ButtonObservableBinder : SubjectBinder<Button, Unit>
    {
        private IDisposable _disposable;


        public ButtonObservableBinder(Button view, ISubject<Unit> model) : base(view, model)
        {
        }

        public override void Bind()
        {
            _disposable = View.OnClickAsObservable().Subscribe(Model.OnNext);
        }

        public override void Unbind()
        {
            _disposable?.Dispose();
        }
    }
}
#endif