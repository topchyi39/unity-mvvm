using System.Collections.Generic;
using System.Linq;

namespace MVVM
{
    public sealed class CompositeBinder : IBinder
    {
        private readonly IBinder[] _binders;

        public CompositeBinder(IEnumerable<IBinder> binders)
        {
            _binders = binders.ToArray();
        }

        public void Bind()
        {
            foreach (var binder in _binders)
            {
                binder.Bind();
            }   
        }

        public void Unbind()
        {
            foreach (var binder in _binders)
            {
                binder.Unbind();
            }
        }
    }
}