using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MVVM.Internal
{
    internal static class InternalBinderFactory
    {
        struct CreateInstanceData
        {
            private ParameterInfo[] _ctorParameters;
            
            public object View { get; }
            public object ViewModel { get; }
            public Type BinderType { get; }

            public CreateInstanceData(ParameterInfo[] ctorParameters, Type binderType, object view, object viewModel)
            {
                _ctorParameters = ctorParameters;
                View = view;
                ViewModel = viewModel;
                BinderType = binderType;
            }

            public bool IsValidParameters() => _ctorParameters is { Length: 2 };
            
            public bool IsTypeInstanceOfView(int parameterIndex) => _ctorParameters[parameterIndex].ParameterType.IsInstanceOfType(View);
            public bool IsTypeInstanceOfViewModel(int parameterIndex) => _ctorParameters[parameterIndex].ParameterType.IsInstanceOfType(ViewModel);
            
        }

        private static List<Type> _registeredBinders;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Init()
        {
            _registeredBinders = new List<Type>();
            
            FindBinders();
        }

        internal static CompositeBinder Create(object view, object viewModel)
        {
            var children = new List<IBinder>();
            if (view == null || viewModel == null)
            {
                throw new Exception("[MVVM] View Model or View is null");
            }
            if (TryCreateConcrete(view, viewModel, out var binder))
            {
                children.Add(binder);
            }

            var viewMemberMap = MemberScanner.ScanMembers(view);
            var viewModelMemberMap = MemberScanner.ScanMembers(viewModel);

            foreach (var (id, viewMember) in viewMemberMap)
            {
                if (viewModelMemberMap.TryGetValue(id, out var viewModelMember))
                {
                    var data = new ResolveData()
                    {
                        viewMember = viewMember,
                        view = view,
                        viewModelMember = viewModelMember,
                        viewModel = viewModel
                    };

                    if (TryResolve(data, out var childBinder))
                    {
                        children.Add(childBinder);
                    }
                }
            }
            
            return new CompositeBinder(children);
        }

        private static void FindBinders()
        {
            var binderType = typeof(IBinder);
            var binderTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => binderType.IsAssignableFrom(t)
                            && t.IsClass
                            && !t.IsAbstract);
            
            _registeredBinders.AddRange(binderTypes);
        }

        private static bool TryCreateConcrete(object view, object viewModel, out IBinder binder)
        {
            var vT = view.GetType();
            var vmT = viewModel.GetType();
            
            foreach (var type in _registeredBinders)
            {
                var binderType = type;
                if (binderType.IsGenericType && TryCreateGenericBinderType(binderType, vT, vmT, out var genericType))
                {
                    binderType = genericType;
                }
                
                var constructor = binderType.GetConstructors(
                        BindingFlags.Public 
                        | BindingFlags.Instance 
                        | BindingFlags.DeclaredOnly)[0];

                var createInstanceData =
                    new CreateInstanceData(constructor.GetParameters(), binderType, view, viewModel);
                
                if (!createInstanceData.IsValidParameters())
                {
                    continue;
                }

                
                if (TryCreateConcreteInstance(createInstanceData, out binder))
                {
                    return true;
                }
            }

            binder = null;
            return false;
        }

        private static bool TryCreateConcreteInstance(CreateInstanceData data, out IBinder binder)
        {
            binder = null;
            if (data.IsTypeInstanceOfView(0) && data.IsTypeInstanceOfViewModel(1))
            {
                binder = Activator.CreateInstance(data.BinderType, data.View, data.ViewModel) as IBinder;
                return true;
            }
                
            if (data.IsTypeInstanceOfViewModel(0) && data.IsTypeInstanceOfView(1))
            {
                binder = Activator.CreateInstance(data.BinderType, data.ViewModel, data.View) as IBinder;

                return true;
            }

            return false;
        }
        
        private static bool TryCreateGenericBinderType(Type binder, Type viewType, Type viewModelType, out Type genericBinder)
        {
            genericBinder = null;
            var isDefinition = binder.IsGenericTypeDefinition;
            
            if (!isDefinition) return false;
            
            var viewArguments = viewType.GenericTypeArguments;
            var viewModelArguments = viewModelType.GenericTypeArguments;

            if (viewArguments.Length > 1)
            {
                Debug.LogError("[MVVM] Generic Arguments more than 1");
                return false;
            }
            
            var isSameArguments = viewArguments.Length == viewModelArguments.Length;

            if (!isSameArguments)
            {
                if (viewModelArguments.Length == 1)
                {
                    try
                    {
                        genericBinder = binder.MakeGenericType(viewModelArguments[0]);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
            
                    return true;
                }
                
                return false;
            }

            for (var i = 0; i < viewArguments.Length; i++)
            {
                if (viewArguments[i] != viewModelArguments[i])
                {
                    isSameArguments = false;
                    break;
                };
            }

            try
            {
                genericBinder = isSameArguments 
                    ? binder.MakeGenericType(viewArguments) 
                    : binder.MakeGenericType(viewArguments[0], viewModelArguments[0]);
            }
            catch (Exception e)
            {
                return false;
            }
            
            return true;
        }

        private static bool TryResolve(ResolveData data, out IBinder binder)
        {
            if (!Resolver.TryResolve(data, out var childView, out var childViewModel))
            {
                Debug.LogError($"[MVVM] Can not find bind data for: {data.view} and ViewModel: {data.viewModel}");
                binder = null;
                return false;
            }

            if (childViewModel == null && data.viewModelMember is FieldInfo fieldInfo)
            {
                childViewModel = Activator.CreateInstance(fieldInfo.FieldType);
                fieldInfo.SetValue(data.viewModel, childViewModel);
                // throw new Exception($"[MVVM] ViewModel Element with id - <b>{data.viewModelMember.Name}</b> with type: {data.viewModelMember.DeclaringType.Name} is null");
            }

            if (childView == null)
            {
                throw new Exception($"[MVVM] View Element with id - <b>{data.viewMember.Name}</b> is null");
            }
            
            if (TryCreateConcrete(childView, childViewModel, out binder))
            {
                return true;
            }
            
            Debug.LogError($"[MVVM] Can not find a binder for View: {data.viewMember.Name}({childView.GetType().Name}) and ViewModel: {data.viewModelMember.Name}({childViewModel.GetType().Name})");
            return false;
        }
    }
}