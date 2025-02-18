#if R3_SUPPORT

using R3;

namespace MVVM
{
    public abstract class ObservableBinder<TV, TM> : Binder<TV, Observable<TM>>
    {
        public ObservableBinder(TV view, Observable<TM> model) : base(view, model) { }
    }
}

#endif
