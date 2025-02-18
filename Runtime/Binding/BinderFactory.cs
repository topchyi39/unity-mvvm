using MVVM.Internal;

namespace MVVM
{
    public static class BinderFactory
    {
        public static IBinder Create(object view, object viewModel) => InternalBinderFactory.Create(view, viewModel);
    }
}