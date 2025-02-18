using System.Collections.Generic;
using System.Reflection;

namespace MVVM.Internal
{
    internal class MemberScanner
    {
        private static BindingFlags Flags => BindingFlags.Instance | 
                                      BindingFlags.FlattenHierarchy | 
                                      BindingFlags.Public |
                                      BindingFlags.NonPublic;
        
        public static IReadOnlyDictionary<object, MemberInfo> ScanMembers(object target)
        {
            var members = new Dictionary<object, MemberInfo>();

            var type = target.GetType();

            var fields = type.GetFields(Flags);
            var methods = type.GetMethods(Flags);
            
            FillDictionary(members, fields);
            FillDictionary(members, methods);
            return members;
        }

        private static void FillDictionary<T>(Dictionary<object, MemberInfo> map, T[] infos) where T : MemberInfo
        {
            foreach (var memberInfo in infos)
            {
                var attribute = memberInfo.GetCustomAttribute<BindMemberAttribute>();
                if (attribute == null) continue;

                map[attribute.Id] = memberInfo;
            }
        }
        
        
    }
}