using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MVVM
{
    [Serializable]
    public class ListView<T> : IEnumerable<T> where T : View
    {
        public T this[int i] => _items[i];
        public int Count => _items.Count;
        
        [SerializeField] private T prefab;
        [SerializeField] protected Transform container;
        
        private List<T> _items = new ();

        public virtual void Initialize() { }
        
        public virtual T Create()
        {
            return Object.Instantiate(prefab, container);
        }

        public virtual void Add(T item, int? index = null)
        {
            _items.Add(item);

            index = index ?? _items.Count - 1;
            item.transform.SetSiblingIndex(index.Value);
        }

        public virtual void Remove(T item)
        {
            _items.Remove(item);
            Object.Destroy(item.gameObject);
        }

        public virtual void Clear()
        {
            foreach (var item in _items)
            {
                Object.Destroy(item.gameObject);
            }
            
            _items.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }


        public void Move(T view, int index) 
        {
            view.transform.SetSiblingIndex(index);
        }
    }
}