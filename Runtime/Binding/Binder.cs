namespace MVVM
{
    public abstract class Binder<TV, TM> : IBinder
    {
        protected readonly TV View;
        protected readonly TM Model;

        public Binder(TV view, TM model)
        {
            View = view;
            Model = model;
        }

        public abstract void Bind();

        public abstract void Unbind();
    }
}