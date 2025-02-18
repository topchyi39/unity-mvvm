using System;
using System.Linq;
using System.Reflection;

namespace MVVM.Internal
{
    internal static class Utils
    {
        public static Delegate CreateActionDelegate(MethodInfo method, object target)
        {
            Type[] paramTypes = method!
                .GetParameters()
                .Select(it => it.ParameterType)
                .ToArray();

            int paramsCount = paramTypes.Length;

            Type delegateType = paramsCount switch
            {
                0 => typeof(Action),
                1 => typeof(Action<>).MakeGenericType(paramTypes[0]),
                2 => typeof(Action<,>).MakeGenericType(paramTypes[0], paramTypes[1]),
                3 => typeof(Action<,,>).MakeGenericType(paramTypes[0], paramTypes[1], paramTypes[2]),
                _ => throw new Exception($"Can't create Action delegate with parameters count: {paramsCount}!")
            };

            return Delegate.CreateDelegate(delegateType, target, method);
        }
        
        public static Delegate CreateFunctionDelegate(MethodInfo method, object target)
        {
            Type returnType = method.ReturnType;
            
            Type[] paramTypes = method
                .GetParameters()
                .Select(it => it.ParameterType)
                .ToArray();

            int paramsCount = paramTypes.Length;

            Type delegateType = paramsCount switch
            {
                0 => typeof(Func<>).MakeGenericType(returnType),
                1 => typeof(Func<,>).MakeGenericType(paramTypes[0], returnType),
                2 => typeof(Func<,,>).MakeGenericType(paramTypes[0], paramTypes[1], returnType),
                3 => typeof(Func<,,,>).MakeGenericType(paramTypes[0], paramTypes[1], paramTypes[2], returnType),
                _ => throw new Exception($"Can't create Action delegate with parameters count: {paramsCount}!")
            };

            return Delegate.CreateDelegate(delegateType, target, method);
        }
    }
}