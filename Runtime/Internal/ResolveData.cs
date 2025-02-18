using System.Reflection;

namespace MVVM.Internal
{
    internal struct ResolveData
    {
        public MemberInfo viewMember;
        public MemberInfo viewModelMember;
        public object view;
        public object viewModel;
    }
}