using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MVVM
{
    [Serializable]
    public class PlaceHolderListView<T> : ListView<T> where T : View
    {
        [SerializeField] private T placeholderPrefab;
        [SerializeField] private int placeholdersAmount;

        private List<T> _placeholders;

        public event Action<T, int> PlaceholderCreated;
        
        public override void Initialize()
        {
            _placeholders = new List<T>(placeholdersAmount);
            for (var i = 0; i < placeholdersAmount; i++)
            {
                var instance = Object.Instantiate(placeholderPrefab, container);
                _placeholders.Add(instance);
                PlaceholderCreated?.Invoke(instance, i);
            }
        }

        public override void Add(T item, int? index = null)
        {
            base.Add(item, index);
            
            if (Count <= placeholdersAmount)
                _placeholders[Count - 1].gameObject.SetActive(false);
        }

        public override void Remove(T item)
        {
            base.Remove(item);
            
            if (Count < placeholdersAmount)
                _placeholders[^Count].gameObject.SetActive(true);

        }
    }
}