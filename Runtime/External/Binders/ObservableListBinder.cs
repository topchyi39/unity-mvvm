#if R3_SUPPORT

using UnityEngine;
using ObservableCollections;
using System.Collections.Generic;
using R3;

namespace MVVM
{
    public class ObservableListBinder<TV, TM> : Binder<ListView<TV>, ObservableList<TM>> where TV : View
    {
        private ISynchronizedView<TM, TV> _synchronizedView;

        private readonly Dictionary<TV, IBinder> _binderMap = new ();
        private readonly Dictionary<TV, TM> _modelMap = new ();
        
        private bool _enabled;
        
        public ObservableListBinder(ListView<TV> view, ObservableList<TM> model) : base(view, model)
        {
        }

        public override void Bind()
        {
            View.Initialize();
            _synchronizedView = Model.CreateView(Create);
            
            _synchronizedView.ObserveAdd().Subscribe(Add);
            _synchronizedView.ObserveRemove().Subscribe(Remove);
            _synchronizedView.ObserveClear().Subscribe(Clear);
            _synchronizedView.ObserveMove().Subscribe(Move);
            _synchronizedView.ObserveReplace().Subscribe(Replace);

            foreach (var view in _synchronizedView)
            {
                BindView(view);
            }
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
            BindView(addEvent.Value.Item2);
        }

        private void Remove(CollectionRemoveEvent<(TM, TV)> removeEvent)
        {
            var view = removeEvent.Value.Item2;
            if (!view) return;
            
            RemoveView(view);
        }

        private void RemoveView(TV view)
        {
            _binderMap[view].Unbind();
            
            _modelMap.Remove(view);
            _binderMap.Remove(view);
            View.Remove(view);
        }

        private void Move(CollectionMoveEvent<(TM, TV)> moveEvent)
        {
            View.Move(moveEvent.Value.Item2, moveEvent.NewIndex);
        }

        private void Replace(CollectionReplaceEvent<(TM, TV)> replaceEvent)
        {
            RemoveView(replaceEvent.OldValue.Item2);
            BindView(replaceEvent.NewValue.Item2, replaceEvent.Index);
        }

        private void Clear(Unit unit)
        {
            _modelMap.Clear();
            _binderMap.Clear();
            View.Clear(); 
        }
        

        private void BindView(TV view, int? index = null)
        {
            _binderMap[view].Bind();
            View.Add(view, index);
        }
    }
}

#endif