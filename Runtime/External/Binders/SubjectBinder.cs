#if R3_SUPPORT

using R3;

namespace MVVM
{
    public abstract class SubjectBinder<TV, TM> : Binder<TV, ISubject<TM>>
    {
        public SubjectBinder(TV view, ISubject<TM> model) : base(view, model) { }
    }
}

#endif