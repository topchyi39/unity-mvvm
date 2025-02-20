#if R3_SUPPORT

using UnityEngine;
using ObservableCollections;
using System.Collections.Generic;
using System.Collections.Specialized;
using R3;

namespace MVVM
{
    public class ObservableListBinder<TV, TM> : Binder<ObservableListView<TV>, ObservableList<TM>> where TV : MonoBehaviour, IView
    {
        private ISynchronizedView<TM, TV> _synchronizedView;

        private readonly Dictionary<TV, IBinder> _binderMap = new ();
        private readonly Dictionary<TV, TM> _modelMap = new ();
        
        private bool _enabled;
        
        public ObservableListBinder(ObservableListView<TV> view, ObservableList<TM> model) : base(view, model)
        {
        }

        public override void Bind()
        {
            _synchronizedView = Model.CreateView(Create);
            
            _synchronizedView.ObserveAdd().Subscribe(Add);
            _synchronizedView.ObserveRemove().Subscribe(Remove);
            _synchronizedView.ObserveClear().Subscribe(Clear);
        }

        public override void Unbind()
        {
            foreach (var pair in _binderMap)
            {
                pair.Value.Unbind();
            }
            
            Clear(default);
            
            _synchronizedView.Dispose();
            _synchronizedView = null;
        }

        private TV Create(TM model)
        {
            var view = View.Create();
            
            _modelMap[view] = model;
            _binderMap[view] = BinderFactory.Create(view, model);
            
            return view;
        }

        private void Add(CollectionAddEvent<(TM,TV)> addEvent)
        {
            var view = addEvent.Value.Item2;
            _binderMap[view].Bind();
            
            View.Add(view);
        }

        private void Remove(CollectionRemoveEvent<(TM, TV)> removeEvent)
        {
            var view = removeEvent.Value.Item2;
            if (!view) return;
            
            _binderMap[view].Unbind();
            
            _modelMap.Remove(view);
            _binderMap.Remove(view);
            View.Remove(view);
        }

        private void Clear(Unit unit)
        {
            _modelMap.Clear();
            _binderMap.Clear();
            View.Clear(); 
        }
    }
}

#endif