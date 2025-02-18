using System;
using JetBrains.Annotations;

namespace MVVM
{
    public abstract class BindMemberAttribute : Attribute
    {
        internal readonly object Id;

        public BindMemberAttribute(object id)
        {
            Id = id;
        }
    }

    public class BindAttribute : BindMemberAttribute
    {
        public BindAttribute(object id) : base(id)
        {
        }
    }
}