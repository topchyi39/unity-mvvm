using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MVVM.Internal
{
    internal class MemberScanner
    {
        private static BindingFlags Flags => BindingFlags.Instance | 
                                      BindingFlags.FlattenHierarchy | 
                                      BindingFlags.Public |
                                      BindingFlags.NonPublic |
                                      BindingFlags.Default;
        
        private static List<FieldInfo> _fields = new (100);
        private static List<MethodInfo> _methods = new (100);
        
        public static IReadOnlyDictionary<object, MemberInfo> ScanMembers(object target)
        {
            _fields.Clear();
            _methods.Clear();
            var members = new Dictionary<object, MemberInfo>();

            var type = target.GetType();

            GetFields(type, Flags, _fields);
            GetMethods(type, Flags, _methods);
            
            FillDictionary(members, _fields);
            FillDictionary(members, _methods);
            return members;
        }

        private static void FillDictionary<T>(Dictionary<object, MemberInfo> map, List<T> infos) where T : MemberInfo
        {
            foreach (var memberInfo in infos)
            {
                var attribute = memberInfo.GetCustomAttribute<BindMemberAttribute>();
                if (attribute == null) continue;

                map[attribute.Id] = memberInfo;
            }
        }

        private static void GetFields(Type type, BindingFlags flags, List<FieldInfo> fields)
        {
            if (type == typeof(object)) return;
            
            fields.AddRange(type.GetFields(flags));
            GetFields(type.BaseType, flags, fields);
        }
        
        private static void GetMethods(Type type, BindingFlags flags, List<MethodInfo> fields)
        {
            if (type == typeof(object)) return;
            
            fields.AddRange(type.GetMethods(flags));
            GetMethods(type.BaseType, flags, fields);
        }
        
        
    }
}