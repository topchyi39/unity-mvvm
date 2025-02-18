using UnityEngine;

namespace MVVM
{
    public class MonoBehaviourBinder : RuntimeBinder
    {
        [SerializeField] private GameObject viewModelGameObject;
        
        protected override object GetViewModel()
        {
            if (viewModelGameObject == null)
            {
                Debug.LogError($"[MVVM] ViewModel GameObject not set");
                return null;
            }   
            
            if (viewModelGameObject.TryGetComponent(ViewModelType, out var viewModelComponent))
            {
                return viewModelComponent;
            }
            
            Debug.LogError($"[MVVM] ViewModel Component: <b>{ViewModelType.Name}</b>. not found in GameObject: {viewModelGameObject.name}");
            return null;
        }
    }
}