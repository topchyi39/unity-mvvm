using System.Reflection;

namespace MVVM.Internal
{
    internal static class Resolver
    {
        public static bool TryResolve(ResolveData data, out object childView, out object childViewModel)
        {
            if (data.viewMember.IsDefined(typeof(BindAttribute)) &&
                data.viewModelMember.IsDefined(typeof(BindAttribute)))
                return TryResolveField(data, out childView, out childViewModel) ||
                       TryResolveMethod(data, out childView, out childViewModel);
            
            childView = default;
            childViewModel = default;
            return false;

        }
        
        private static bool TryResolveField(ResolveData data, out object childView, out object childViewModel)
        {
            if ( data.viewMember is FieldInfo viewField)
            {
                childView = viewField!.GetValue(data.view);
                
                if (data.viewModelMember is FieldInfo modelField)
                {
                    childViewModel = modelField!.GetValue(data.viewModel);
                    return true;
                }

                if (data.viewModelMember is MethodInfo modelMethod)
                {
                    childViewModel = Utils.CreateActionDelegate(modelMethod, data.viewModel);
                    return true;
                }
            }

            childView = default;
            childViewModel = default;
            return false;
        }

        private static bool TryResolveMethod(ResolveData data, out object childView, out object childViewModel)
        {
            if (data.viewMember.IsDefined(typeof(BindAttribute)) && data.viewMember is MethodInfo viewMethod)
            {
                childView = Utils.CreateActionDelegate(viewMethod, data.view);
                
                if (data.viewModelMember.IsDefined(typeof(BindAttribute)) && data.viewModelMember is FieldInfo modelField)
                {
                    childViewModel = modelField!.GetValue(data.viewModel);
                    return true;
                }
            }

            childView = default;
            childViewModel = default;
            return false;
        }
    }
}