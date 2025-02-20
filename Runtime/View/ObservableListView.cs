using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MVVM
{
    [Serializable]
    public class ObservableListView<T> : IEnumerable<T> where T : MonoBehaviour, IView
    {
        public T this[int i] => _items[i];
        
        [SerializeField] private T prefab;
        [SerializeField] private Transform container;

        private List<T> _items = new ();


        public virtual T Create()
        {
            return Object.Instantiate(prefab, container);
        }

        public virtual void Add(T item)
        {
            _items.Add(item);
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
    }
}