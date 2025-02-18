using System;
using UnityEngine;

namespace MVVM
{
    public abstract class View : MonoBehaviour, IView
    {
        public abstract Type ViewModelType { get; }
    }

    public abstract class View<T> : View, IView
    {
        public override Type ViewModelType => typeof(T);
    } 
}