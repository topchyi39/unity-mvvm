using System;

namespace MVVM
{
    public interface IView
    {
        public Type ViewModelType { get; }
    }
}