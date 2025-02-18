// using UnityEngine;
//
// namespace MVVM.Binders
// {
//     public interface IViewBinder
//     {
//         
//     }
//     
//     public class ViewBinder : MonoBehaviour, IViewBinder
//     {
//         [SerializeField] private BindingType bindingType;
//         [SerializeField, ShowIf(nameof(BindFromInstance))] private MonoBehaviour viewModelInstance;
//         
//         private DiContainer _container;
//         private IBinder _binder;
//         private bool BindFromInstance => bindingType == BindingType.FromInstance;
//         
//         [Inject]
//         private void Construct(DiContainer container)
//         {
//             _container = container;
//         }
//
//         private void Awake()
//         {
//             _binder = CreateBinder();
//         }
//
//         private void OnEnable()
//         {
//             _binder.Bind();
//         }
//
//         private void OnDisable()
//         {
//             _binder.Unbind();   
//         }
//
//         private IBinder CreateBinder()
//         {
//             var view = GetComponent<IView>();
//
//             object viewModel;
//             if (BindFromInstance && viewModelInstance != null)
//             {
//                 viewModel = viewModelInstance;
//             }
//             else
//             {
//                 var viewModelType = ViewModelRepository.GetViewModelType(view.GetType());
//                 viewModel = _container.Resolve(viewModelType);
//             }
//             
//
//             return BinderFactory.CreateBinder(view, viewModel);
//         }
//
//         enum BindingType
//         {
//             FromInstance,
//             Inject,
//         }
//     }
// }